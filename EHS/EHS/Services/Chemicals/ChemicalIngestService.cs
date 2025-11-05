using EHS.Data;
using EHS.Models.IH;
using EHS.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        // inside ChemicalIngestService
        public async Task<List<IhChemHistoryRow>> GetRecentAsync(int take = 25, CancellationToken ct = default)
        {
            // If you don't have navigation props, swap Count() calls for subqueries as in comment below
            return await _db.ih_chemical
                .OrderByDescending(c => /*(DateTime?)*/c.PreferredName /*UpdatedUtc ?? (DateTime?)c.CreatedUtc ?? DateTime.UtcNow*/)
                .Take(take)
                .Select(c => new IhChemHistoryRow
                {
                    Id = c.Id,
                    CasNumber = c.CasNumber,
                    PreferredName = c.PreferredName,
                    PubChemCid = c.PubChemCid,
                    SynonymCount = c.Synonyms.Count(),          // or: _db.ih_chemical_synonym.Count(s => s.ChemicalId == c.Id)
                    PropertyCount = c.Properties.Count(),        // or: _db.ih_chemical_property.Count(p => p.ChemicalId == c.Id)
                    HazardCount = c.Hazards.Count(),           // or: _db.ih_chemical_hazard.Count(h => h.ChemicalId == c.Id)
                    OelCount = c.OELs.Count(),              // or: _db.ih_chemical_oel.Count(o => o.ChemicalId == c.Id)
                    SamplingCount = c.SamplingMethods.Count()    // or: _db.ih_chemical_sampling_method.Count(m => m.ChemicalId == c.Id)
                })
                .ToListAsync(ct);
        }


        public async Task<(ChemicalCoreDto Dto, List<string> Unavailable)> FetchByCasWithStatusAsync(string cas, CancellationToken ct = default)
        {
            var unavailable = new List<string>();
            cas = cas.Trim();

            int? cid = await _pubchem.ResolveCidByCasAsync(cas, unavailable: unavailable, ct: ct);
            var props = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            string? preferredName = null;
            var synonyms = new List<string>();
            var hazards = new List<IhChemicalHazard>();
            var oels = new List<IhChemicalOel>();
            var methods = new List<IhChemicalSamplingMethod>();

            if (cid is int c)
            {
                props = await _pubchem.GetCorePropertiesAsync(c, unavailable: unavailable, ct: ct);

                var names = await _pubchem.GetPugViewAsync(c, heading: "Names and Identifiers", unavailable: unavailable, ct: ct);
                var (casFound, prefName, syns) = _parser.ExtractNames(names);
                if (!string.IsNullOrWhiteSpace(prefName)) preferredName = prefName;
                if (!string.IsNullOrWhiteSpace(casFound) && !cas.Equals(casFound, StringComparison.OrdinalIgnoreCase))
                    synonyms.Add(casFound);
                synonyms.AddRange(syns.Take(2000));

                try
                {
                    var extraSyns = await _pubchem.GetSynonymsAsync(c, unavailable: unavailable, ct: ct);
                    if (extraSyns != null)
                    {
                        foreach (var s in extraSyns)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                                synonyms.Add(s);
                            if (synonyms.Count >= 5000) break;
                        }
                    }
                }
                catch { }

                if (string.IsNullOrWhiteSpace(preferredName))
                {
                    if (props.TryGetValue("IUPACName", out var iup) && !string.IsNullOrWhiteSpace(iup))
                        preferredName = iup;
                    else if (synonyms.Count > 0)
                        preferredName = synonyms[0];
                }

                var hazardsRoot = await _pubchem.GetPugViewAsync(c, heading: null, unavailable: unavailable, ct: ct);
                var hazardsRaw = _parser.ExtractHazards(hazardsRoot);

                // ---- DEDUPE + DROP NO-CODE HAZARDS (before deriving any flags) ----
                var rawCount = hazardsRaw.Count;
                hazards = CleanHazards(hazardsRaw);
                var distinctCount = hazards.Count;
                // Optional: trace counts
                System.Diagnostics.Debug.WriteLine($"[Hazards] raw={rawCount} distinct_with_code={distinctCount}");
                // -------------------------------------------------------------------

                var expRoot = await _pubchem.GetPugViewAsync(c, heading: "Experimental Properties", unavailable: unavailable, ct: ct);
                var safRoot = await _pubchem.GetPugViewAsync(c, heading: "Safety and Hazards", unavailable: unavailable, ct: ct);
                var ext = _parser.ExtractExtendedProps(expRoot, safRoot);
                foreach (var kv in ext)
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        props[kv.Key] = kv.Value;

                var derived = _parser.DeriveFlagsFromHazards(hazards);
                foreach (var kv in derived)
                    props[kv.Key] = kv.Value;

                if (props.TryGetValue("MolecularWeight", out var mwStr) &&
                    double.TryParse(mwStr?.Replace(",", ""), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var mw) && mw > 0)
                {
                    var mg3PerPpm = mw / 24.45;
                    var ppmPerMg3 = 24.45 / mw;
                    props["Conversion mg/m3 per ppm @25C"] = mg3PerPpm.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                    props["Conversion ppm per mg/m3 @25C"] = ppmPerMg3.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            var nameCandidates = BuildHighSignalNameCandidates(preferredName, synonyms);

            try
            {
                oels = await _oel.GetOelsFromNpgByCasOrNamesAsync(cas, nameCandidates, unavailable: null, ct: ct);
            }
            catch
            {
                oels = new List<IhChemicalOel>();
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
                    unavailable?.Add("OSHA OELs (blocked 403) — using NIOSH Pocket Guide OELs instead.");
                }
                catch
                {
                    unavailable?.Add("OSHA OELs: fallback fetch error");
                }
            }

            oels = NormalizeAndDedupeOels(oels);

            if (oels.Count == 0 && unavailable != null &&
                !unavailable.Any(u => u.StartsWith("OELs fetched", StringComparison.OrdinalIgnoreCase)))
            {
                unavailable.Add("OELs fetched but no numeric limits parsed (page format or source gap).");
            }

            // PPE — FLAT first, then structured merge
            try
            {
                var ppeFlat = await _oel.GetPpeFlatFromNpgByCasOrNamesAsync(cas, nameCandidates, unavailable: null, ct: ct);
                var ppeStructured = await _oel.GetPpeFromNpgByCasOrNamesAsync(cas, nameCandidates, unavailable: null, ct: ct);

                var allPpe = ppeFlat.Concat(ppeStructured)
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
                    .GroupBy(kv => kv.Key.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(kv => kv.Value?.Length ?? 0).First().Value?.Trim() ?? ""
                    );

                foreach (var kv in allPpe)
                {
                    props[kv.Key] = kv.Value;
                }

                if (!ppeFlat.Any() && !ppeStructured.Any())
                    unavailable?.Add("No PPE section found in NPG for this substance.");
            }
            catch (Exception ex)
            {
                unavailable?.Add($"NIOSH NPG PPE: parse/merge error — {ex.Message}");
            }

            try
            {
                //methods = await _oel.GetSamplingMethodsByCasAsync(cas, unavailable: unavailable, ct: ct);
                methods = await _oel.GetSamplingMethodsByCasAsync(cas, preferredName, synonyms, unavailable, ct);
            }
            catch
            {
                unavailable?.Add("NIOSH/OSHA methods (error)");
            }

            try
            {
                var byName = await _oel.GetNmamByChemicalNameAsync(preferredName, nameCandidates, unavailable, ct);
                var map = new Dictionary<string, IhChemicalSamplingMethod>(StringComparer.OrdinalIgnoreCase);
                foreach (var m in methods.Concat(byName))
                {
                    var key = $"{m.Source}\n{m.MethodId}".ToLowerInvariant();
                    if (!map.ContainsKey(key)) map[key] = m;
                    else if (string.IsNullOrWhiteSpace(map[key].Url) && !string.IsNullOrWhiteSpace(m.Url)) map[key].Url = m.Url;
                }
                methods = map.Values.OrderBy(x => x.Source).ThenBy(x => x.MethodId).ToList();
            }
            catch
            {
                unavailable?.Add("NIOSH NMAM: name-based method lookup error");
            }

            var dto = new ChemicalCoreDto(
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
            // matchNames must be IEnumerable<string>
            var nameCandidates = matchNames?.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
                                ?? new List<string>();

            //List<IhChemicalOel> oels;
            try
            {
                // 1) NPG by CAS; if not found, try the provided names/synonyms
                oels = await _oel.GetOelsFromNpgByCasOrNamesAsync(cas, nameCandidates, unavailable: null, ct: ct);
            }
            catch
            {
                oels = new();
            }

            // 2) If still empty, try OSHA per-chemical page by CAS
            if (oels.Count == 0)
            {
                try { oels = await _oel.GetOelsByCasAsync(cas, unavailable: null, ct: ct); }
                catch { /* ignore */ }
            }
            UpsertOels(chem, oels);

            // Sampling methods by CAS (NMAM) with name fallback
            List<IhChemicalSamplingMethod> methods = new();
            try
            {
                var synonymsList = chem.Synonyms.Select(x => x.Synonym).ToList();
                methods = await _oel.GetSamplingMethodsByCasAsync(
                    cas,
                    chem.PreferredName,
                    synonymsList,
                    unavailable: null,
                    ct: ct
                );
            }
            catch
            {
                methods = new();
            }
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


        // ChemicalIngestService.cs (or a repository)
        public async Task<EHS.ViewModels.ChemicalCoreDto?> BuildDtoFromDbAsync(string cas,CancellationToken ct = default)
        {
            // Read only the parent row first (fast, no Includes)
            var head = await _db.ih_chemical
                .AsNoTracking()
                .Where(c => c.CasNumber == cas)
                .Select(c => new
                {
                    c.Id,
                    c.CasNumber,
                    c.PreferredName,
                    c.PubChemCid
                })
                .SingleOrDefaultAsync(ct);

            if (head == null) return null;

            var chemId = head.Id;

            // Load each child collection in its own query (sequential; same DbContext)
            var synonyms = await _db.Set<IhChemicalSynonym>()
                .AsNoTracking()
                .Where(s => s.IhChemicalId == chemId)
                .OrderBy(s => s.Synonym)
                .Select(s => s.Synonym)
                .ToArrayAsync(ct);

            var propsList = await _db.Set<IhChemicalProperty>()
                .AsNoTracking()
                .Where(p => p.IhChemicalId == chemId)
                .ToListAsync(ct);

            var hazards = await _db.Set<IhChemicalHazard>()
                .AsNoTracking()
                .Where(h => h.IhChemicalId == chemId)
                .ToListAsync(ct);

            var oels = await _db.Set<IhChemicalOel>()
                .AsNoTracking()
                .Where(o => o.IhChemicalId == chemId)
                .OrderBy(o => o.Source).ThenBy(o => o.Type)
                .ToListAsync(ct);

            var methods = await _db.Set<IhChemicalSamplingMethod>()
                .AsNoTracking()
                .Where(m => m.IhChemicalId == chemId)
                .OrderBy(m => m.Source).ThenBy(m => m.MethodId)
                .ToListAsync(ct);

            // Dictionary for properties
            var props = propsList.Count == 0
                ? new Dictionary<string, string?>()
                : propsList.ToDictionary(p => p.Key, p => p.Value);

            // Build the DTO the view expects
            return new EHS.ViewModels.ChemicalCoreDto(
                head.CasNumber,
                head.PreferredName,
                head.PubChemCid,
                synonyms,
                props,
                hazards,
                oels,
                methods
            );
        }

        public async Task UpsertFromDtoAsync(EHS.ViewModels.ChemicalCoreDto dto, CancellationToken ct = default)
        {
            // 1) Get or create parent (no Includes)
            var chem = await _db.ih_chemical
                .SingleOrDefaultAsync(c => c.CasNumber == dto.CasNumber, ct);

            if (chem == null)
            {
                chem = new IhChemical
                {
                    CasNumber = dto.CasNumber,
                    PreferredName = dto.PreferredName,
                    PubChemCid = dto.PubChemCid
                };
                _db.ih_chemical.Add(chem);
                await _db.SaveChangesAsync(ct); // ensures chem.Id
            }
            else
            {
                chem.PreferredName = dto.PreferredName;
                chem.PubChemCid = dto.PubChemCid;
                await _db.SaveChangesAsync(ct);
            }

            // 2) Wipe children with set-based deletes (FAST)
#if NET7_0_OR_GREATER
            await _db.Set<IhChemicalSynonym>().Where(x => x.IhChemicalId == chem.Id).ExecuteDeleteAsync(ct);
            await _db.Set<IhChemicalProperty>().Where(x => x.IhChemicalId == chem.Id).ExecuteDeleteAsync(ct);
            await _db.Set<IhChemicalHazard>().Where(x => x.IhChemicalId == chem.Id).ExecuteDeleteAsync(ct);
            await _db.Set<IhChemicalOel>().Where(x => x.IhChemicalId == chem.Id).ExecuteDeleteAsync(ct);
            await _db.Set<IhChemicalSamplingMethod>().Where(x => x.IhChemicalId == chem.Id).ExecuteDeleteAsync(ct);
#else
    // EF Core 6 or earlier: raw SQL fallbacks
    await _db.Database.ExecuteSqlRawAsync("DELETE FROM ih_chemical_synonym WHERE ih_chemical_id = {0}", chem.Id, ct);
    await _db.Database.ExecuteSqlRawAsync("DELETE FROM ih_chemical_property WHERE ih_chemical_id = {0}", chem.Id, ct);
    await _db.Database.ExecuteSqlRawAsync("DELETE FROM ih_chemical_hazard WHERE ih_chemical_id = {0}", chem.Id, ct);
    await _db.Database.ExecuteSqlRawAsync("DELETE FROM ih_chemical_oel WHERE ih_chemical_id = {0}", chem.Id, ct);
    await _db.Database.ExecuteSqlRawAsync("DELETE FROM ih_chemical_sampling_method WHERE ih_chemical_id = {0}", chem.Id, ct);
#endif

            // 3) Reinsert children
            if (dto.Synonyms != null && dto.Synonyms.Length > 0)
            {
                var rows = dto.Synonyms
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(s => new IhChemicalSynonym { IhChemicalId = chem.Id, Synonym = s.Trim() });
                await _db.Set<IhChemicalSynonym>().AddRangeAsync(rows, ct);
            }

            if (dto.Properties != null && dto.Properties.Count > 0)
            {
                var rows = dto.Properties
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Key))
                    .Select(kv => new IhChemicalProperty
                    {
                        IhChemicalId = chem.Id,
                        Key = kv.Key.Trim(),
                        Value = kv.Value
                    });
                await _db.Set<IhChemicalProperty>().AddRangeAsync(rows, ct);
            }

            if (dto.Hazards != null && dto.Hazards.Count > 0)
            {
                // If IhChemicalHazard is your entity, map it and set FK
                foreach (var h in dto.Hazards)
                    h.IhChemicalId = chem.Id; // ensure FK
                await _db.Set<IhChemicalHazard>().AddRangeAsync(dto.Hazards, ct);
            }

            if (dto.OELs != null && dto.OELs.Count > 0)
            {
                foreach (var o in dto.OELs)
                    o.IhChemicalId = chem.Id;
                await _db.Set<IhChemicalOel>().AddRangeAsync(dto.OELs, ct);
            }

            if (dto.SamplingMethods != null && dto.SamplingMethods.Count > 0)
            {
                foreach (var m in dto.SamplingMethods)
                    m.IhChemicalId = chem.Id;
                await _db.Set<IhChemicalSamplingMethod>().AddRangeAsync(dto.SamplingMethods, ct);
            }

            await _db.SaveChangesAsync(ct);
        }


        // Put this inside ChemicalIngestService (private helper)
        private static List<IhChemicalOel> NormalizeAndDedupeOels(List<IhChemicalOel> input)
        {
            if (input == null || input.Count == 0) return input ?? new();

            string NormType(string? t)
            {
                t = (t ?? "").Trim();
                if (string.IsNullOrEmpty(t)) return "LIMIT";
                var x = t.ToLowerInvariant();
                if (x is "st" or "stel") return "STEL";
                if (x is "c" or "ceil" or "ceiling") return "Ceiling";
                if (x.Contains("twa") || x.Contains("time-weighted")) return "TWA";
                if (x.Contains("action")) return "Action Level";
                return char.ToUpperInvariant(t[0]) + t[1..];
            }

            string NormSource(string? s)
            {
                s = (s ?? "").Trim();
                if (string.IsNullOrEmpty(s)) return "";
                var x = s.ToLowerInvariant();
                if (x.Contains("cal/osha") || x.Contains("california")) return "Cal/OSHA";
                if (x.Contains("osha")) return "OSHA";
                if (x.Contains("acgih")) return "ACGIH";
                if (x.Contains("niosh")) return "NIOSH";
                return s;
            }

            // number/unit parser + canon to a unit "family" (ppm vs mg/m3) and a base numeric for dedupe
            static bool TryParseNumberAndUnit(string? value, out double num, out string unit, out string family)
            {
                num = 0; unit = ""; family = "";
                if (string.IsNullOrWhiteSpace(value)) return false;

                var m = System.Text.RegularExpressions.Regex.Match(
                    value,
                    @"(?<n>\d+(?:\.\d+)?)\s*(?<u>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (!m.Success) return false;

                if (!double.TryParse(
                        m.Groups["n"].Value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var n)) return false;

                var u = m.Groups["u"].Value.Trim().ToLowerInvariant()
                    .Replace(" ", "")
                    .Replace("m³", "m3")
                    .Replace("m^3", "m3")
                    .Replace("㎥", "m3")
                    .Replace("μg", "ug")
                    .Replace("µg", "ug");

                switch (u)
                {
                    case "ppm":
                        num = n; unit = "ppm"; family = "ppm"; return true;
                    case "ppb":
                        num = n / 1000.0; unit = "ppb"; family = "ppm"; return true; // base=ppm
                    case "mg/m3":
                        num = n; unit = "mg/m3"; family = "mgm3"; return true;
                    case "ug/m3":
                        num = n / 1000.0; unit = "µg/m3"; family = "mgm3"; return true; // base=mg/m3, pretty print µg
                    default:
                        return false;
                }
            }

            static string EnsureNote(string note, string mustContain)
            {
                note ??= "";
                if (note.IndexOf(mustContain, StringComparison.OrdinalIgnoreCase) >= 0) return note;
                return string.IsNullOrWhiteSpace(note) ? mustContain : (note + "; " + mustContain);
            }

            static string DropNoteToken(string note, string tokenLike)
            {
                note ??= "";
                if (string.IsNullOrWhiteSpace(note)) return note;
                // remove the token + any leading/trailing separators
                var rx = new System.Text.RegularExpressions.Regex(@"\s*;?\s*" + System.Text.RegularExpressions.Regex.Escape(tokenLike), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var cleaned = rx.Replace(note, " ").Trim();
                cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s*;\s*;", ";");
                cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"^\s*;\s*|\s*;\s*$", "");
                return cleaned.Trim();
            }

            // 1) normalize source/type strings first
            var normalized = new List<IhChemicalOel>(input.Count);
            foreach (var o in input)
            {
                if (o == null) continue;
                normalized.Add(new IhChemicalOel
                {
                    Source = NormSource(o.Source),
                    Type = NormType(o.Type),
                    Value = (o.Value ?? "").Trim(),
                    Notes = (o.Notes ?? "").Trim()
                });
            }

            double? NumKey(IhChemicalOel o)
                => TryParseNumberAndUnit(o.Value, out var n, out _, out _) ? n : null;

            string Fam(IhChemicalOel o)
                => TryParseNumberAndUnit(o.Value, out _, out _, out var fam) ? fam : "";

            bool IsPpm(IhChemicalOel o) => Fam(o) == "ppm";

            // 2) Heuristic corrections (general — not hard-coded to a CAS)
            // NIOSH: small ppm pair → min=TWA (8-hour only), max=Ceiling (15-minute only)
            {
                var nioshPpm = normalized
                    .Where(o => string.Equals(o.Source, "NIOSH", StringComparison.OrdinalIgnoreCase))
                    .Where(IsPpm)
                    .OrderBy(o => NumKey(o) ?? double.MaxValue)
                    .ToList();

                var small = nioshPpm.Where(o => NumKey(o) is double v && v <= 0.2).ToList();
                if (small.Count >= 2)
                {
                    var min = small.First();
                    var max = small.Last();

                    min.Type = "TWA";
                    min.Notes = EnsureNote(DropNoteToken(min.Notes, "15-minute"), "8-hour");

                    max.Type = "Ceiling";
                    max.Notes = EnsureNote(DropNoteToken(max.Notes, "8-hour"), "15-minute");
                }
            }

            // OSHA: ppm pair → min=TWA, max=STEL (drop mislabels)
            {
                var oshaPpm = normalized
                    .Where(o => string.Equals(o.Source, "OSHA", StringComparison.OrdinalIgnoreCase))
                    .Where(IsPpm)
                    .OrderBy(o => NumKey(o) ?? double.MaxValue)
                    .ToList();

                if (oshaPpm.Count >= 2)
                {
                    var min = oshaPpm.First();
                    var max = oshaPpm.Last();
                    min.Type = "TWA";
                    max.Type = "STEL";
                }
            }

            // 3) Collapse conflicts: keep ONE row per (Source, family, numeric value)
            //    and pick the best type by rank (TWA > STEL > Ceiling > Action Level), then richer notes.
            int TypeRank(string t) => t switch
            {
                "TWA" => 0,
                "STEL" => 1,
                "Ceiling" => 2,
                "Action Level" => 3,
                _ => 9
            };

            var bestBySrcFamNum = new Dictionary<string, IhChemicalOel>(StringComparer.OrdinalIgnoreCase);

            foreach (var o in normalized)
            {
                if (!TryParseNumberAndUnit(o.Value, out var num, out _, out var fam)) continue;
                var key = $"{o.Source}|{fam}|{num.ToString("0.######", System.Globalization.CultureInfo.InvariantCulture)}";

                if (!bestBySrcFamNum.TryGetValue(key, out var existing))
                {
                    bestBySrcFamNum[key] = o;
                }
                else
                {
                    // prefer better type; if tie, richer notes
                    var cmp = TypeRank(o.Type ?? "LIMIT").CompareTo(TypeRank(existing.Type ?? "LIMIT"));
                    if (cmp < 0) { bestBySrcFamNum[key] = o; continue; }
                    if (cmp == 0)
                    {
                        var scoreNew = (o.Notes ?? "").Length * 2 + (o.Value ?? "").Length;
                        var scoreOld = (existing.Notes ?? "").Length * 2 + (existing.Value ?? "").Length;
                        if (scoreNew > scoreOld) bestBySrcFamNum[key] = o;
                    }
                }
            }

            // add any non-numeric (rare), dedup by (Source|Type|Value)
            var nonNumeric = normalized.Where(o => !TryParseNumberAndUnit(o.Value, out _, out _, out _)).ToList();
            var stringDedup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var o in nonNumeric)
            {
                var k = $"{o.Source}|{o.Type}|{o.Value}";
                if (stringDedup.Add(k)) bestBySrcFamNum[k] = o;
            }

            // 4) Final sort
            int SourceRank(string s) => s switch
            {
                "NIOSH" => 0,
                "OSHA" => 1,
                "Cal/OSHA" => 2,
                "ACGIH" => 3,
                _ => 9
            };

            var result = bestBySrcFamNum.Values
                .OrderBy(o => SourceRank(o.Source ?? ""))
                .ThenBy(o => TypeRank(o.Type ?? ""))
                .ThenBy(o => NumKey(o) ?? double.MaxValue)
                .ThenBy(o => (o.Value ?? ""))
                .ToList();

            return result;
        }

        private static List<string> BuildHighSignalNameCandidates(string? preferredName, IEnumerable<string> synonyms)
        {
            bool LooksLikeRegistryCode(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return true;
                var t = s.Trim();
                return Regex.IsMatch(t, @"^(AKOS|MFCD|NSC|SKU|BRN|RTECS|UNII|EC|EINECS|ENCS|DTXSID|DTXCID|CHEMBL|CAS|CHEBI|VANDF|HSDB|PubChem)\b", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(t, @"^[A-Z]{2,6}([- ]?\d.*)?$", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(t, @"^\w{2,6}[- ]?\d{2,}$");
            }

            string Clean(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = Regex.Replace(s, @"\([^)]*\)", " ");
                s = Regex.Replace(s, @"\b(anhydrous|monohydrate|dihydrate|trihydrate|hydrate|pentahydrate|usp|nf|bp|fcc|tech(?:nical)?|solution|reagent|grade|tn)\b", " ", RegexOptions.IgnoreCase);
                s = Regex.Replace(s, @"[^A-Za-z \-]", " ");
                s = Regex.Replace(s, @"\s+", " ").Trim();
                if (Regex.IsMatch(s, @"\bformalin\b", RegexOptions.IgnoreCase)) s = "formaldehyde";
                return s;
            }

            IEnumerable<string> Expand(string cleaned)
            {
                if (string.IsNullOrWhiteSpace(cleaned)) yield break;
                yield return cleaned;
                var tokens = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    var lastTwo = $"{tokens[^2]} {tokens[^1]}".Trim();
                    if (lastTwo.Length >= 4) yield return lastTwo;
                }
            }

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            void AddOne(string? raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return;
                if (LooksLikeRegistryCode(raw)) return;
                var cleaned = Clean(raw);
                foreach (var form in Expand(cleaned))
                {
                    if (string.IsNullOrWhiteSpace(form)) continue;
                    if (form.Length < 4) continue;
                    if (LooksLikeRegistryCode(form)) continue;
                    set.Add(form);
                }
            }

            AddOne(preferredName);
            foreach (var s in (synonyms ?? Enumerable.Empty<string>()).Take(5000))
                AddOne(s);

            var list = set.OrderBy(s => s.Length).ThenBy(s => s).Take(25).ToList();

            if (!string.IsNullOrWhiteSpace(preferredName))
            {
                var prefClean = Clean(preferredName);
                var i = list.FindIndex(x => x.Equals(prefClean, StringComparison.OrdinalIgnoreCase));
                if (i > 0)
                {
                    var v = list[i];
                    list.RemoveAt(i);
                    list.Insert(0, v);
                }
                else if (i < 0 && !string.IsNullOrWhiteSpace(prefClean))
                {
                    list.Insert(0, prefClean);
                }
            }

            System.Diagnostics.Debug.WriteLine("[NPG NAME CANDIDATES] " + string.Join("\n", list.Take(15)));
            return list;
        }

        // ---- Local helpers (scoped to this method) --------------------------
        static string? NormalizeHazardCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            // Trim + canonicalize whitespace + uppercase; keep internal spaces (e.g., "Skin Irrit. 2")
            var c = Regex.Replace(code.Trim(), @"\s+", " ");
            return c.ToUpperInvariant();
        }

        static List<IhChemicalHazard> CleanHazards(IEnumerable<IhChemicalHazard> raw)
        {
            var output = new List<IhChemicalHazard>();
            if (raw == null) return output;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var h in raw)
            {
                var norm = NormalizeHazardCode(h?.Code);
                if (string.IsNullOrWhiteSpace(norm))
                    continue; // <-- drop hazards without a usable code

                if (seen.Add(norm)) // <-- dedupe by normalized code
                {
                    output.Add(new IhChemicalHazard
                    {
                        IhChemicalId = h.IhChemicalId,                 // keep whatever identity you already set upstream
                        Code = norm,                          // normalized (trimmed/uppercased)
                        Description = h?.Description?.Trim(),
                        Source = h?.Source?.Trim()
                    });
                }
            }

            output = output.OrderBy(o => o.Code).ToList();

            return output;
        }


    }
}