using EHS.Data;
using EHS.Models.IH;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;

namespace EHS.Services.Chemicals
{
    public class ChemicalIngestService
    {
        private readonly EHSContext _db;
        private readonly PubChemClient _pubchem;
        private readonly OshaNioshClient _oel;
        private readonly PugViewParser _parser;


        public ChemicalIngestService(EHSContext db, PubChemClient pubchem, OshaNioshClient oel, PugViewParser parser)
        { 
            _db = db; _pubchem = pubchem; _oel = oel; _parser = parser; 
        }

        public async Task<(EHS.ViewModels.ChemicalCoreDto Dto, List<string> Unavailable)> FetchByCasWithStatusAsync(
    string cas,
    CancellationToken ct = default)
        {
            var unavailable = new List<string>();
            cas = cas.Trim();

            int? cid = await _pubchem.ResolveCidByCasAsync(cas, unavailable: unavailable, ct: ct);

            var props = new Dictionary<string, string?>();
            string? preferredName = null;
            var synonyms = new List<string>();
            var hazards = new List<IhChemicalHazard>();
            var oels = new List<IhChemicalOel>();
            var methods = new List<IhChemicalSamplingMethod>();

            if (cid is int c)
            {
                // Core properties
                props = await _pubchem.GetCorePropertiesAsync(c, unavailable: unavailable, ct: ct);

                // Names / synonyms
                var names = await _pubchem.GetPugViewAsync(c, heading: "Names and Identifiers", unavailable: unavailable, ct: ct);
                var (casFound, prefName, syns) = _parser.ExtractNames(names);
                if (!string.IsNullOrWhiteSpace(prefName)) preferredName = prefName;
                if (!string.IsNullOrWhiteSpace(casFound) && !cas.Equals(casFound, StringComparison.OrdinalIgnoreCase))
                    synonyms.Add(casFound);
                synonyms.AddRange(syns.Take(1000));

                // Fallback synonyms if sparse
                if (synonyms.Count < 3)
                {
                    var extraSyns = await _pubchem.GetSynonymsAsync(c, unavailable: unavailable, ct: ct);
                    synonyms.AddRange(extraSyns.Take(1000));
                }

                // Preferred-name fallback
                if (string.IsNullOrWhiteSpace(preferredName))
                {
                    if (props.TryGetValue("IUPACName", out var iup) && !string.IsNullOrWhiteSpace(iup))
                        preferredName = iup;
                    else if (synonyms.Count > 0)
                        preferredName = synonyms[0];
                }

                // Hazards (GHS/NFPA/HMIS/Toxicity)
                var hazardsRoot = await _pubchem.GetPugViewAsync(c, heading: null, unavailable: unavailable, ct: ct);
                hazards = _parser.ExtractHazards(hazardsRoot);

                // Experimental + Safety props (physicals, LEL/UEL, appearance)
                var expRoot = await _pubchem.GetPugViewAsync(c, heading: "Experimental Properties", unavailable: unavailable, ct: ct);
                var safRoot = await _pubchem.GetPugViewAsync(c, heading: "Safety and Hazards", unavailable: unavailable, ct: ct);
                var ext = _parser.ExtractExtendedProps(expRoot, safRoot);
                foreach (var kv in ext)
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        props[kv.Key] = kv.Value;

                // Derived flags from hazards
                var derived = _parser.DeriveFlagsFromHazards(hazards);
                foreach (var kv in derived)
                    props[kv.Key] = kv.Value;

                // Conversion factors using MolecularWeight
                if (props.TryGetValue("MolecularWeight", out var mwStr) &&
                    double.TryParse(mwStr?.Replace(",", ""), System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var mw) &&
                    mw > 0)
                {
                    var mg3PerPpm = mw / 24.45; // mg/m3 per ppm @25 °C
                    var ppmPerMg3 = 24.45 / mw;
                    props["Conversion mg/m3 per ppm @25C"] = mg3PerPpm.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                    props["Conversion ppm per mg/m3 @25C"] = ppmPerMg3.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            // ---------- Occupational Exposure Limits (NPG first; OSHA fallback) ----------
            try
            {
                oels = await _oel.GetOelsFromNpgByCasAsync(cas, unavailable: unavailable, ct: ct);
            }
            catch
            {
                unavailable?.Add("NIOSH NPG: OEL fetch error");
            }

            if (oels.Count == 0)
            {
                try
                {
                    var oshaOels = await _oel.GetOelsByCasAsync(cas, unavailable: unavailable, ct: ct);
                    if (oshaOels?.Count > 0) oels = oshaOels;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // precise message when OSHA is blocked in your environment
                    unavailable?.Add("OSHA OELs (blocked 403) — using NIOSH Pocket Guide OELs instead.");
                }
                catch
                {
                    unavailable?.Add("OSHA OELs: fallback fetch error");
                }
            }

            // ----- after the new OEL retrieval block (NPG first; optional OSHA fallback) -----
            oels = CleanAndNormalizeOels(oels);

            // Only add the warning if nothing survived AND we haven't already added it
            if (oels.Count == 0 && unavailable != null
                && !unavailable.Any(u => u.StartsWith("OELs fetched but no numeric limits parsed", StringComparison.OrdinalIgnoreCase)))
            {
                unavailable.Add("OELs fetched but no numeric limits parsed (page format or source gap).");
            }


            // ---------- Sampling methods (CAS + name-based NMAM merge) ----------
            try { methods = await _oel.GetSamplingMethodsByCasAsync(cas, unavailable: unavailable, ct: ct); }
            catch { unavailable.Add("NIOSH/OSHA methods (error)"); }

            try
            {
                var synonymsNorm = synonyms
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var nameMethods = await _oel.GetNmamByChemicalNameAsync(preferredName, synonymsNorm, unavailable, ct);

                string Key(IhChemicalSamplingMethod m) => $"{m.Source}|{m.MethodId}".ToLowerInvariant();
                var map = new Dictionary<string, IhChemicalSamplingMethod>(StringComparer.OrdinalIgnoreCase);

                foreach (var m in methods)
                {
                    var k = Key(m);
                    if (!map.TryGetValue(k, out var existing)) map[k] = m;
                    else if (string.IsNullOrWhiteSpace(existing.Url) && !string.IsNullOrWhiteSpace(m.Url)) existing.Url = m.Url;
                }
                foreach (var m in nameMethods)
                {
                    var k = Key(m);
                    if (!map.TryGetValue(k, out var existing)) map[k] = m;
                    else if (string.IsNullOrWhiteSpace(existing.Url) && !string.IsNullOrWhiteSpace(m.Url)) existing.Url = m.Url;
                }

                methods = map.Values.OrderBy(x => x.Source).ThenBy(x => x.MethodId).ToList();
            }
            catch { unavailable?.Add("NIOSH NMAM: name-based method lookup error"); }

            var dto = new EHS.ViewModels.ChemicalCoreDto(
                cas,
                preferredName,
                cid,
                synonyms.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                props,
                hazards,
                oels,
                methods
            );

            return (dto, unavailable);
        }


        public async Task<EHS.ViewModels.ChemicalCoreDto> FetchByCasAsync(string cas, CancellationToken ct = default)
        {
            var (dto, _) = await FetchByCasWithStatusAsync(cas, ct);
            return dto;
        }

        // SAVE (persists to DB); re-fetches and upserts
        public async Task<EHS.ViewModels.ChemicalCoreDto> IngestByCasAsync(string cas, CancellationToken ct = default)
        {
            cas = cas.Trim();
            var chem = await _db.ih_chemical
                .Include(x => x.Synonyms)
                .Include(x => x.Properties)
                .Include(x => x.Hazards)
                .Include(x => x.OELs)
                .Include(x => x.SamplingMethods)
                .FirstOrDefaultAsync(x => x.CasNumber == cas, ct)
                ?? new IhChemical { CasNumber = cas };

            var cid = await _pubchem.ResolveCidByCasAsync(cas, ct);
            chem.PubChemCid = cid;

            if (cid is int c)
            {
                var props = await _pubchem.GetCorePropertiesAsync(c, ct);
                UpsertProperties(chem, props);

                var names = await _pubchem.GetPugViewAsync(c, "Names and Identifiers", ct);
                var (casFound, prefName, syns) = _parser.ExtractNames(names);
                if (!string.IsNullOrWhiteSpace(prefName)) chem.PreferredName = prefName;
                if (!string.IsNullOrWhiteSpace(casFound) && !string.Equals(chem.CasNumber, casFound, StringComparison.OrdinalIgnoreCase))
                    chem.Synonyms.Add(new IhChemicalSynonym { Synonym = casFound });
                foreach (var s in syns.Take(500))
                    if (!chem.Synonyms.Any(x => x.Synonym.Equals(s, StringComparison.OrdinalIgnoreCase)))
                        chem.Synonyms.Add(new IhChemicalSynonym { Synonym = s });

                // After ExtractNames(...) and adding synonyms...
                if (string.IsNullOrWhiteSpace(chem.PreferredName))
                {
                    if (chem.Properties.FirstOrDefault(p => p.Key == "IUPACName")?.Value is string iup && !string.IsNullOrWhiteSpace(iup))
                        chem.PreferredName = iup;
                    else if (chem.Synonyms.Count > 0)
                        chem.PreferredName = chem.Synonyms.First().Synonym;
                }

                var hazardsView = await _pubchem.GetPugViewAsync(c, null, ct);
                var hazards = _parser.ExtractHazards(hazardsView);
                UpsertHazards(chem, hazards);

                // Experimental + Safety props → upsert into property table
                var expRoot = await _pubchem.GetPugViewAsync(c, heading: "Experimental Properties", ct: ct);
                var safRoot = await _pubchem.GetPugViewAsync(c, heading: "Safety and Hazards", ct: ct);

                var ext = _parser.ExtractExtendedProps(expRoot, safRoot);
                UpsertProperties(chem, ext);

                // Derived flags as properties
                var derived = _parser.DeriveFlagsFromHazards(chem.Hazards);
                UpsertProperties(chem, derived);

                // Conversion factors using MolecularWeight
                var mwProp = chem.Properties.FirstOrDefault(p => p.Key == "MolecularWeight")?.Value;
                if (double.TryParse(mwProp?.Replace(",", ""), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var mw) && mw > 0)
                {
                    var mg3PerPpm = mw / 24.45;
                    var ppmPerMg3 = 24.45 / mw;
                    UpsertProperties(chem, new Dictionary<string, string?>
                    {
                        ["Conversion mg/m3 per ppm @25C"] = mg3PerPpm.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture),
                        ["Conversion ppm per mg/m3 @25C"] = ppmPerMg3.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                    });
                }
            }

            // NEW — build compact match-name set for OSHA tables
            var matchNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(chem.PreferredName)) matchNames.Add(chem.PreferredName);
            matchNames.Add(cas);
            // add a handful of synonyms to improve hit rate
            foreach (var s in chem.Synonyms.Select(x => x.Synonym).Where(s => !string.IsNullOrWhiteSpace(s)).Take(50))
                matchNames.Add(s);

            // OELs by NAME
            List<IhChemicalOel> oels = new();
            try { oels = await _oel.GetOelsAsync(matchNames, ct: ct); } catch { oels = new(); }
            UpsertOels(chem, oels);

            // Sampling methods by CAS (NMAM)
            List<IhChemicalSamplingMethod> methods = new();
            try { methods = await _oel.GetSamplingMethodsByCasAsync(cas, ct: ct); } catch { methods = new(); }
            UpsertSampling(chem, methods);

            // Guard: don't save if there's effectively no data and it's a new record
            bool hasAnyData = (chem.PubChemCid.HasValue)
                || chem.Properties.Any() || chem.Synonyms.Any()
                || chem.Hazards.Any() || chem.OELs.Any() || chem.SamplingMethods.Any();

            if (chem.Id == 0 && !hasAnyData)
            {
                return new EHS.ViewModels.ChemicalCoreDto(
                    chem.CasNumber, chem.PreferredName, chem.PubChemCid,
                    chem.Synonyms.Select(s => s.Synonym).ToArray(),
                    chem.Properties.ToDictionary(p => p.Key, p => p.Value),
                    chem.Hazards.ToList(), chem.OELs.ToList(), chem.SamplingMethods.ToList()
                );
            }

            if (chem.Id == 0) _db.ih_chemical.Add(chem);
            await _db.SaveChangesAsync(ct);

            return new EHS.ViewModels.ChemicalCoreDto(
                chem.CasNumber,
                chem.PreferredName,
                chem.PubChemCid,
                chem.Synonyms.Select(s => s.Synonym).ToArray(),
                chem.Properties.ToDictionary(p => p.Key, p => p.Value),
                chem.Hazards.ToList(),
                chem.OELs.ToList(),
                chem.SamplingMethods.ToList()
            );
        }

        private static void UpsertProperties(IhChemical chem, Dictionary<string, string?> props)
        {
            foreach (var (k, v) in props)
            {
                var existing = chem.Properties.FirstOrDefault(p => p.Key == k);
                if (existing is null) chem.Properties.Add(new IhChemicalProperty { Key = k, Value = v });
                else existing.Value = v;
            }
        }
        private static void UpsertHazards(IhChemical chem, List<IhChemicalHazard> hazards)
        {
            foreach (var h in hazards)
                if (!chem.Hazards.Any(x => x.Source == h.Source && x.Code == h.Code && x.Description == h.Description))
                    chem.Hazards.Add(h);
        }
        private static void UpsertOels(IhChemical chem, List<IhChemicalOel> oels)
        {
            foreach (var o in oels)
                if (!chem.OELs.Any(x => x.Source == o.Source && x.Type == o.Type && x.Value == o.Value))
                    chem.OELs.Add(o);
        }
        private static void UpsertSampling(IhChemical chem, List<IhChemicalSamplingMethod> methods)
        {
            foreach (var m in methods)
                if (!chem.SamplingMethods.Any(x => x.Source == m.Source && x.MethodId == m.MethodId))
                    chem.SamplingMethods.Add(m);
        }

        // Keep only meaningful, numeric OELs and normalize Type/Source.
        private static List<IhChemicalOel> CleanAndNormalizeOels(IEnumerable<IhChemicalOel> raw)
        {
            if (raw == null) return new List<IhChemicalOel>();

            var allowedTypes = new[] { "TWA", "STEL", "Ceiling", "C", "Peak", "IDLH" };
            var hasNumber = new Regex(@"\d", RegexOptions.Compiled);

            string NormalizeType(string? t, string? v)
            {
                var type = (t ?? "").Trim();
                foreach (var ok in allowedTypes)
                    if (type.Contains(ok, StringComparison.OrdinalIgnoreCase))
                        return ok.Equals("C", StringComparison.OrdinalIgnoreCase) ? "Ceiling" : ok;

                var val = v ?? "";
                if (Regex.IsMatch(val, @"\bSTEL\b", RegexOptions.IgnoreCase)) return "STEL";
                if (Regex.IsMatch(val, @"\b(CEILING|C[^\w]|^C\s)", RegexOptions.IgnoreCase)) return "Ceiling";
                if (Regex.IsMatch(val, @"\bPEAK\b", RegexOptions.IgnoreCase)) return "Peak";
                if (Regex.IsMatch(val, @"\bIDLH\b", RegexOptions.IgnoreCase)) return "IDLH";
                if (Regex.IsMatch(val, @"\bTWA\b", RegexOptions.IgnoreCase)) return "TWA";
                return "";
            }

            string NormalizeSource(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return s ?? "";
                if (s.Contains("NIOSH", StringComparison.OrdinalIgnoreCase)) return "NIOSH";
                if (s.Contains("OSHA", StringComparison.OrdinalIgnoreCase)) return "OSHA";
                if (s.Contains("ACGIH", StringComparison.OrdinalIgnoreCase)) return "ACGIH";
                if (s.Contains("CAL", StringComparison.OrdinalIgnoreCase) || s.Contains("Cal/OSHA", StringComparison.OrdinalIgnoreCase))
                    return "Cal/OSHA";
                return s.Trim();
            }

            bool LooksLikeLimit(string? value, string? type)
            {
                if (string.IsNullOrWhiteSpace(value)) return false;
                if (!hasNumber.IsMatch(value)) return false;
                if (Regex.IsMatch(value, @"\bDescription\b", RegexOptions.IgnoreCase)) return false;

                // ✅ broadened units (ppm, ppb, mg/m3 variants, micrograms with different symbols)
                if (Regex.IsMatch(value, @"\b(ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)\b", RegexOptions.IgnoreCase))
                    return true;

                if ("IDLH".Equals(type, StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }

            var cleaned = new List<IhChemicalOel>();
            foreach (var o in raw)
            {
                if (o == null) continue;

                var src = NormalizeSource(o.Source);
                var typ = NormalizeType(o.Type, o.Value);
                var val = o.Value?.Trim();

                if (string.IsNullOrWhiteSpace(typ))
                {
                    if (!Regex.IsMatch(val ?? "", @"\bIDLH\b", RegexOptions.IgnoreCase))
                        continue;
                    typ = "IDLH";
                }

                if (!LooksLikeLimit(val, typ))
                    continue;

                val = Regex.Replace(val!, @"\s+", " ").Trim();
                var notes = o.Notes;
                if (!string.IsNullOrWhiteSpace(notes))
                    notes = Regex.Replace(notes, @"\s+", " ").Trim();

                cleaned.Add(new IhChemicalOel
                {
                    Source = src,
                    Type = typ,
                    Value = val,
                    Notes = notes
                });
            }

            // Dedup using a lowercase concatenated key
            var deduped = cleaned
                .GroupBy(x =>
                    $"{(x.Source ?? "").ToLowerInvariant()}|" +
                    $"{(x.Type ?? "").ToLowerInvariant()}|" +
                    $"{(x.Value ?? "").ToLowerInvariant()}")
                .Select(g => g.OrderBy(x => (x.Notes ?? "").Length).First())
                .OrderBy(x => x.Source)
                .ThenBy(x => x.Type)
                .ToList();

            return deduped;
        }

        // ChemicalIngestService.cs (or a repository)
        public async Task<EHS.ViewModels.ChemicalCoreDto?> BuildDtoFromDbAsync(
            string cas, CancellationToken ct = default)
        {
            cas = cas.Trim();

            // Replace EhsDbContext / DbSets with your real ones
            var chem = await _db.IhChemicals
                .AsNoTracking()
                .Include(x => x.Synonyms)
                .Include(x => x.Properties)
                .Include(x => x.Hazards)
                .Include(x => x.Oels)
                .Include(x => x.Methods)
                .SingleOrDefaultAsync(x => x.CasNumber == cas, ct);

            if (chem == null) return null;

            var props = (chem.Properties ?? new List<IhChemicalProperty>())
                .GroupBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Value, StringComparer.OrdinalIgnoreCase);

            return new EHS.ViewModels.ChemicalCoreDto(
                cas: chem.CasNumber,
                preferredName: chem.PreferredName,
                pubChemCid: chem.PubChemCid,
                synonyms: (chem.Synonyms ?? new List<IhChemicalSynonym>()).Select(s => s.Name).ToArray(),
                properties: props,
                hazards: (chem.Hazards ?? new List<IhChemicalHazard>()).Select(h => new IhChemicalHazard
                {
                    Source = h.Source,
                    Code = h.Code,
                    Description = h.Description
                }).ToList(),
                oELs: (chem.Oels ?? new List<IhChemicalOel>()).Select(o => new IhChemicalOel
                {
                    Source = o.Source,
                    Type = o.Type,
                    Value = o.Value,
                    Notes = o.Notes
                }).ToList(),
                samplingMethods: (chem.Methods ?? new List<IhChemicalSamplingMethod>()).Select(m => new IhChemicalSamplingMethod
                {
                    Source = m.Source,
                    MethodId = m.MethodId,
                    Url = m.Url
                }).ToList()
            );
        }

        public async Task UpsertFromDtoAsync(EHS.ViewModels.ChemicalCoreDto dto, CancellationToken ct = default)
        {
            var chem = await _db.IhChemicals
                .Include(x => x.Synonyms)
                .Include(x => x.Properties)
                .Include(x => x.Hazards)
                .Include(x => x.Oels)
                .Include(x => x.Methods)
                .SingleOrDefaultAsync(x => x.CasNumber == dto.CasNumber, ct);

            if (chem == null)
            {
                chem = new IhChemical
                {
                    CasNumber = dto.CasNumber,
                    PreferredName = dto.PreferredName,
                    PubChemCid = dto.PubChemCid
                };
                _db.IhChemicals.Add(chem);
            }
            else
            {
                chem.PreferredName = dto.PreferredName;
                chem.PubChemCid = dto.PubChemCid;

                // wipe children for simplicity (fine for admin ingest)
                _db.IhChemicalSynonyms.RemoveRange(chem.Synonyms);
                _db.IhChemicalProperties.RemoveRange(chem.Properties);
                _db.IhChemicalHazards.RemoveRange(chem.Hazards);
                _db.IhChemicalOels.RemoveRange(chem.Oels);
                _db.IhChemicalSamplingMethods.RemoveRange(chem.Methods);
            }

            chem.Synonyms = (dto.Synonyms ?? Array.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(s => new IhChemicalSynonym { Name = s }).ToList();

            chem.Properties = (dto.Properties ?? new Dictionary<string, string?>())
                .Select(kv => new IhChemicalProperty { Key = kv.Key, Value = kv.Value })
                .ToList();

            chem.Hazards = (dto.Hazards ?? new List<IhChemicalHazard>())
                .Select(h => new IhChemicalHazard { Source = h.Source, Code = h.Code, Description = h.Description })
                .ToList();

            chem.Oels = (dto.OELs ?? new List<IhChemicalOel>())
                .Select(o => new IhChemicalOel { Source = o.Source, Type = o.Type, Value = o.Value, Notes = o.Notes })
                .ToList();

            chem.Methods = (dto.SamplingMethods ?? new List<IhChemicalSamplingMethod>())
                .Select(m => new IhChemicalSamplingMethod { Source = m.Source, MethodId = m.MethodId, Url = m.Url })
                .ToList();

            await _db.SaveChangesAsync(ct);
        }

    }
}