using EHS.Models.IH;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace EHS.Services.Chemicals
{
    public class OshaNioshClient
    {
        private readonly HttpClient _http;
        public OshaNioshClient(HttpClient http) { _http = http; }


        [Conditional("DEBUG")]
        private void DebugDump(string tag, string? text, int take = 500, string? fileHint = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.WriteLine($"[{tag}] <null>");
                return;
            }

            var flat = text.Replace("\r", " ").Replace("\n", " ").Trim();
            var peek = flat.Length <= take ? flat : flat.Substring(0, take);
            Debug.WriteLine($"[{tag}] len={flat.Length} peek=\"{peek}\"");

            if (!string.IsNullOrWhiteSpace(fileHint))
            {
                try
                {
                    var path = Path.Combine(Path.GetTempPath(), $"ehs_{fileHint}.txt");
                    File.WriteAllText(path, flat);
                    Debug.WriteLine($"[{tag}] wrote full text to: {path}");
                }
                catch { /* ignore */ }
            }
        }




        // Pulls an OSHA method code like "ID165SG" from a URL or text.
        private static string DeriveOshaMethodCode(string? url, string? textFallback)
        {
            // Try from URL first (…/id165sg/id165sg.pdf → ID165SG)
            if (!string.IsNullOrWhiteSpace(url))
            {
                var m = System.Text.RegularExpressions.Regex.Match(url, @"\b(id[\-_]?\d+[a-z]*)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();
            }

            // Fallback to any ID#### token in link text
            if (!string.IsNullOrWhiteSpace(textFallback))
            {
                var m = System.Text.RegularExpressions.Regex.Match(textFallback, @"\b(id[\-_]?\d+[a-z]*)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();
            }

            return string.IsNullOrWhiteSpace(textFallback) ? "OSHA METHOD" : textFallback;
        }

        // Browser-like fetch to avoid 403s
        private async Task<string?> GetHtmlOrNullAsync(string url, CancellationToken ct)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124 Safari/537.36");
                req.Headers.TryAddWithoutValidation("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                req.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadAsStringAsync(ct);
            }
            catch { return null; }
        }

        // ----- OELs: look up rows by NAME (not CAS) across Z-1/Z-2/Z-3 -----
        // names: include preferred name + a handful of synonyms (shortish)
        public async Task<List<IhChemicalOel>> GetOelsAsync(
            IEnumerable<string> names,
            List<string>? unavailable = null,
            CancellationToken ct = default)
        {
            var list = new List<IhChemicalOel>();
            var nameSet = names.Where(s => !string.IsNullOrWhiteSpace(s))
                               .Select(s => s.Trim())
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToArray();
            if (nameSet.Length == 0) return list;

            var pages = new[]
            {
                "https://www.osha.gov/annotated-pels/table-z-1",
                "https://www.osha.gov/annotated-pels/table-z-2",
                "https://www.osha.gov/annotated-pels/table-z-3"
            };

            int matches = 0;
            foreach (var url in pages)
            {
                var html = await GetHtmlOrNullAsync(url, ct);
                if (string.IsNullOrEmpty(html))
                {
                    unavailable?.Add($"OSHA: {url.Split('/').Last()} unavailable/blocked");
                    continue;
                }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Find tables that have a "Substance" header (Annotated tables)
                var tables = doc.DocumentNode.SelectNodes("//table") ?? new HtmlNodeCollection(null);
                foreach (var table in tables)
                {
                    // Build header index
                    var headerCells = table.SelectNodes(".//thead//th") ?? table.SelectNodes(".//tr[1]/th") ?? new HtmlNodeCollection(null);
                    var idx = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < headerCells.Count; i++)
                    {
                        var h = HtmlEntity.DeEntitize(headerCells[i].InnerText).Trim();
                        if (!idx.ContainsKey(h)) idx[h] = i;
                    }
                    if (!idx.Keys.Any(k => k.Contains("Substance", StringComparison.OrdinalIgnoreCase))) continue;

                    // Candidate columns we care about
                    int colSub = idx.FirstOrDefault(kv => kv.Key.Contains("Substance", StringComparison.OrdinalIgnoreCase)).Value;
                    int colOsha = idx.FirstOrDefault(kv => kv.Key.Contains("OSHA PEL", StringComparison.OrdinalIgnoreCase)).Value;
                    int colNiosh = idx.FirstOrDefault(kv => kv.Key.Contains("NIOSH REL", StringComparison.OrdinalIgnoreCase)).Value;
                    int colCal = idx.FirstOrDefault(kv => kv.Key.Contains("Cal/OSHA", StringComparison.OrdinalIgnoreCase)).Value;
                    int colAcgih = idx.FirstOrDefault(kv => kv.Key.Contains("ACGIH", StringComparison.OrdinalIgnoreCase)).Value;

                    foreach (var row in table.SelectNodes(".//tbody//tr") ?? table.SelectNodes(".//tr[position()>1]") ?? new HtmlNodeCollection(null))
                    {
                        var cells = row.SelectNodes("./td")?.ToList();
                        if (cells is null || cells.Count == 0) continue;

                        string sub = GetCell(cells, colSub);
                        if (!MatchesAny(sub, nameSet)) continue;  // name-based match

                        matches++;

                        // Collect any present columns
                        if (colOsha >= 0)
                        {
                            var v = GetCell(cells, colOsha);
                            if (!string.IsNullOrWhiteSpace(v))
                                list.Add(new IhChemicalOel { Source = "OSHA", Type = "PEL", Value = v, Notes = sub });
                        }
                        if (colNiosh >= 0)
                        {
                            var v = GetCell(cells, colNiosh);
                            if (!string.IsNullOrWhiteSpace(v))
                                list.Add(new IhChemicalOel { Source = "NIOSH", Type = "REL", Value = v, Notes = sub });
                        }
                        if (colCal >= 0)
                        {
                            var v = GetCell(cells, colCal);
                            if (!string.IsNullOrWhiteSpace(v))
                                list.Add(new IhChemicalOel { Source = "Cal/OSHA", Type = "PEL", Value = v, Notes = sub });
                        }
                        if (colAcgih >= 0)
                        {
                            var v = GetCell(cells, colAcgih);
                            if (!string.IsNullOrWhiteSpace(v))
                                list.Add(new IhChemicalOel { Source = "ACGIH", Type = "TLV", Value = v, Notes = sub });
                        }
                    }
                }
            }

            if (matches == 0)
                unavailable?.Add("OSHA: No OEL row matched by name/synonym");

            // De-dup identical rows
            return list.GroupBy(o => (o.Source, o.Type, o.Value, o.Notes ?? ""))
                       .Select(g => g.First()).ToList();
        }

        private static bool MatchesAny(string hay, IEnumerable<string> needles)
        {
            var h = (hay ?? "").Trim();
            foreach (var n in needles)
            {
                if (string.IsNullOrWhiteSpace(n)) continue;
                if (h.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            }
            return false;
        }
        private static string GetCell(List<HtmlNode> cells, int idx)
        {
            if (idx < 0 || idx >= cells.Count) return "";
            return HtmlEntity.DeEntitize(cells[idx].InnerText).Trim().Replace("\u00A0", " ");
        }

        // ----- Sampling Methods: parse NMAM "Methods by CAS Number" table properly -----
        public async Task<List<IhChemicalSamplingMethod>> GetSamplingMethodsByCasAsync(string cas, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var byKey = new Dictionary<(string source, string methodNum), IhChemicalSamplingMethod>(new TupleIgnoreCaseComparer());

            // ===== NIOSH NMAM CAS table(s) =====
            var nmamPages = new[]
            {
                "https://www.cdc.gov/niosh/nmam/cas.html",
                "https://www.cdc.gov/niosh/docs/2003-154/method-casall.html"
            };

            int hits = 0;
            foreach (var pageUrl in nmamPages)
            {
                var html = await GetHtmlOrNullAsync(pageUrl, ct);
                if (string.IsNullOrEmpty(html))
                {
                    unavailable?.Add($"NIOSH: NMAM index unavailable/blocked ({pageUrl.Split('/').Last()})");
                    continue;
                }

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var table = doc.DocumentNode.SelectSingleNode("//table");
                if (table == null) continue;

                foreach (var row in table.SelectNodes(".//tr[td]") ?? new HtmlAgilityPack.HtmlNodeCollection(null))
                {
                    var tds = row.SelectNodes("./td")?.ToList();
                    if (tds is null || tds.Count == 0) continue;

                    var casCell = HtmlEntity.DeEntitize(tds[0].InnerText).Trim();
                    if (!StringEqualsCas(casCell, cas)) continue;

                    hits++;

                    for (int i = 1; i < tds.Count; i++)
                    {
                        var cell = tds[i];
                        var text = HtmlEntity.DeEntitize(cell.InnerText).Trim();

                        // Pull a 3–4 digit number from text (candidate method)
                        var mTxt = System.Text.RegularExpressions.Regex.Match(text, @"\b(\d{3,4})\b");
                        string? methodNumFromText = mTxt.Success ? mTxt.Groups[1].Value : null;

                        var links = cell.SelectNodes(".//a") ?? new HtmlAgilityPack.HtmlNodeCollection(null);

                        // (1) Merge any links in this cell
                        foreach (var a in links)
                        {
                            var hrefRaw = a.GetAttributeValue("href", string.Empty);
                            var hrefAbs = ResolveHref(hrefRaw, pageUrl);

                            // If we don't have a number yet, try to derive it from the URL (.../pdfs/7908.pdf)
                            var methodNum = methodNumFromText;
                            if (methodNum is null && !string.IsNullOrWhiteSpace(hrefAbs))
                            {
                                var mHref = System.Text.RegularExpressions.Regex.Match(hrefAbs, @"\b(\d{3,4})(?=\.pdf|/|$)");
                                if (mHref.Success) methodNum = mHref.Groups[1].Value;
                            }

                            if (methodNum != null || hrefAbs != null)
                            {
                                var key = ("NIOSH", methodNum ?? "NMAM");
                                if (!byKey.TryGetValue(key, out var existing))
                                {
                                    byKey[key] = new IhChemicalSamplingMethod
                                    {
                                        Source = "NIOSH",
                                        MethodId = methodNum is null ? "NMAM" : $"NIOSH {methodNum}",
                                        Url = hrefAbs
                                    };
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(existing.Url) && !string.IsNullOrWhiteSpace(hrefAbs))
                                        existing.Url = hrefAbs;
                                    if (existing.MethodId == "NMAM" && methodNum is not null)
                                        existing.MethodId = $"NIOSH {methodNum}";
                                }
                            }
                        }

                        // (2) Add number from text ONLY if it's a real NMAM method and not the CAS fragment
                        if (methodNumFromText != null &&
                            await IsValidNioshMethodAsync(methodNumFromText, ct) &&
                            !IsCasFragment(methodNumFromText, cas))
                        {
                            var key = ("NIOSH", methodNumFromText);
                            if (!byKey.ContainsKey(key))
                            {
                                byKey[key] = new IhChemicalSamplingMethod
                                {
                                    Source = "NIOSH",
                                    MethodId = $"NIOSH {methodNumFromText}",
                                    Url = null
                                };
                            }
                        }
                    }
                }
            }

            if (hits == 0)
                unavailable?.Add("NIOSH: No NMAM methods matched this CAS");

            // ===== OSHA SAM index (row-based parse; prefer canonical PDF) =====
            var oshaIndexUrl = "https://www.osha.gov/chemicaldata/sampling-analytical-methods";
            var oshaHtml = await GetHtmlOrNullAsync(oshaIndexUrl, ct);
            if (string.IsNullOrEmpty(oshaHtml))
            {
                unavailable?.Add("OSHA: SAM index unavailable/blocked");
            }
            else
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(oshaHtml);

                // Look for any table row that mentions the CAS anywhere in the row text
                var rows = doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlAgilityPack.HtmlNodeCollection(null);
                foreach (var tr in rows)
                {
                    var rowText = HtmlEntity.DeEntitize(tr.InnerText ?? string.Empty).Replace("\u00A0", " ").Trim();
                    if (rowText.IndexOf(cas, StringComparison.OrdinalIgnoreCase) < 0) continue;

                    // In this row, extract method links
                    foreach (var a in tr.SelectNodes(".//a") ?? new HtmlAgilityPack.HtmlNodeCollection(null))
                    {
                        var hrefAbs = ResolveHref(a.GetAttributeValue("href", string.Empty), oshaIndexUrl);
                        hrefAbs = NormalizeOshaMethodUrl(hrefAbs); // maps old /dts/... to /sites/default/files/methods/osha-{code}.pdf
                        if (hrefAbs == null) continue;             // skip generic index or unusable links

                        var code = DeriveOshaMethodCode(hrefAbs, HtmlEntity.DeEntitize(a.InnerText ?? string.Empty));
                        if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("ID", StringComparison.OrdinalIgnoreCase))
                            continue; // keep only OSHA ID#### methods

                        var key = ("OSHA", code);
                        if (!byKey.TryGetValue(key, out var existing))
                        {
                            byKey[key] = new IhChemicalSamplingMethod
                            {
                                Source = "OSHA",
                                MethodId = code,   // e.g., ID165SG
                                Url = hrefAbs      // e.g., https://www.osha.gov/sites/default/files/methods/osha-id165sg.pdf
                            };
                        }
                        else if (string.IsNullOrWhiteSpace(existing.Url))
                        {
                            existing.Url = hrefAbs;
                        }
                    }
                }
            }

            // ===== Merge OSHA methods from the per-chemical database page (authoritative) =====
            try
            {
                var viaDb = await GetOshaFromChemicalDbByCasAsync(cas, ct);
                foreach (var m in viaDb)
                {
                    var key = (source: "OSHA", methodNum: m.MethodId ?? "OSHA");
                    if (!byKey.TryGetValue(key, out var existing))
                    {
                        byKey[key] = m;
                    }
                    else if (string.IsNullOrWhiteSpace(existing.Url) && !string.IsNullOrWhiteSpace(m.Url))
                    {
                        existing.Url = m.Url;
                    }
                }

                // Optional: if sulfuric/nitric/phosphoric acid, also include ID165SG (Acid Mist) if not present.
                // This method applies to multiple inorganic acids and is commonly referenced with H2SO4 sampling.
                // You can comment this block out if you prefer to show only what OSHA lists per-chemical.
                var alreadyHas165 = byKey.Keys.Any(k => k.source == "OSHA" && k.methodNum.Equals("ID165SG", StringComparison.OrdinalIgnoreCase));
                if (!alreadyHas165)
                {
                    // Heuristic: add for strong inorganic acids that commonly use acid mist sampling
                    if (cas.Equals("7664-93-9", StringComparison.OrdinalIgnoreCase) // sulfuric acid
                        || cas.Equals("7697-37-2", StringComparison.OrdinalIgnoreCase) // nitric acid
                        || cas.Equals("7664-38-2", StringComparison.OrdinalIgnoreCase)) // phosphoric acid
                    {
                        byKey[("OSHA", "ID165SG")] = new IhChemicalSamplingMethod
                        {
                            Source = "OSHA",
                            MethodId = "ID165SG",
                            Url = "https://www.osha.gov/sites/default/files/methods/osha-id165sg.pdf"
                        };
                    }
                }
            }
            catch
            {
                unavailable?.Add("OSHA: chemical database page parse error");
            }

            return byKey.Values
                        .OrderBy(v => v.Source)
                        .ThenBy(v => v.MethodId)
                        .ToList();
        }


        // -------- Back-compat: old method names --------
        public Task<List<IhChemicalOel>> GetOelsByCasAsync(string cas, CancellationToken ct = default)
            => GetOelsAsync(new[] { cas }, unavailable: null, ct: ct);

        public async Task<List<IhChemicalOel>> GetOelsByCasAsync(
            string cas,
            List<string>? unavailable = null,
            CancellationToken ct = default)
        {
            var oels = new List<IhChemicalOel>();

            // A) Find page (logs show if/when we found it)
            var chemUrl = await FindOshaChemicalUrlByCasAsync(cas, ct);
            System.Diagnostics.Debug.WriteLine($"[OSHA CHEM URL] CAS={cas} => {chemUrl}");
            if (string.IsNullOrWhiteSpace(chemUrl))
            {
                unavailable?.Add("OSHA OELs: per-chemical page not found");
                return oels;
            }

            // B) Fetch the detail page
            var chemHtml = await GetHtmlOrNullAsync(chemUrl!, ct);
            if (string.IsNullOrWhiteSpace(chemHtml))
            {
                unavailable?.Add("OSHA OELs: chemical page blocked/unavailable");
                return oels;
            }

            var cdoc = new HtmlAgilityPack.HtmlDocument();
            cdoc.LoadHtml(chemHtml);

            // C) Try to isolate "Exposure Limits" content
            string expoText = "";
            var heading = cdoc.DocumentNode.SelectSingleNode("//*[self::h1 or self::h2 or self::h3][contains(., 'Exposure Limits')]");
            if (heading != null)
            {
                var table = heading.SelectSingleNode("following::table[1]");
                if (table != null)
                    expoText = HtmlAgilityPack.HtmlEntity.DeEntitize(table.InnerText ?? string.Empty);
            }

            // FIX #3: fallback to the first table that contains OEL tokens if heading/table not found
            if (string.IsNullOrWhiteSpace(expoText))
            {
                var anyOelTable = cdoc.DocumentNode
                    .SelectNodes("//table")?
                    .FirstOrDefault(t =>
                    {
                        var txt = t.InnerText ?? "";
                        return txt.IndexOf("PEL", StringComparison.OrdinalIgnoreCase) >= 0
                            || txt.IndexOf("REL", StringComparison.OrdinalIgnoreCase) >= 0
                            || txt.IndexOf("TLV", StringComparison.OrdinalIgnoreCase) >= 0
                            || txt.IndexOf("Action Level", StringComparison.OrdinalIgnoreCase) >= 0;
                    });
                if (anyOelTable != null)
                    expoText = HtmlAgilityPack.HtmlEntity.DeEntitize(anyOelTable.InnerText ?? string.Empty);
            }

            if (string.IsNullOrWhiteSpace(expoText))
            {
                // Last resort: full page text
                expoText = HtmlAgilityPack.HtmlEntity.DeEntitize(cdoc.DocumentNode.InnerText ?? string.Empty);
            }

            // normalize whitespace
            expoText = System.Text.RegularExpressions.Regex.Replace(expoText, @"\s+", " ").Trim();

            // Peek logs to verify we captured something meaningful
            System.Diagnostics.Debug.WriteLine($"[OSHA EXPO LEN] CAS={cas} len={expoText.Length}");
            System.Diagnostics.Debug.WriteLine($"[OSHA EXPO PEEK] {expoText.Substring(0, Math.Min(300, Math.Max(0, expoText.Length)))}");

            // D) Helpers
            void Add(string source, string limitType, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                var v = value;
                var cut = v.IndexOf(" [", StringComparison.Ordinal);
                if (cut > 0) v = v.Substring(0, cut).Trim();

                oels.Add(new IhChemicalOel
                {
                    Source = source,  // "OSHA", "NIOSH", "ACGIH", "Cal/OSHA"
                    Type = limitType,
                    Value = v
                });
            }

            // E) Parse series — assign first PEL-* to OSHA, second to Cal/OSHA
            void ParsePelSeries(string kind, string typeLabel)
            {
                var rx = new System.Text.RegularExpressions.Regex(
                    $@"PEL[-\s]?{kind}\s+(.+?)(?=(?:\s+(?:PEL|REL|TLV|Action Level|Peak)\b|$))",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                int seen = 0;
                foreach (System.Text.RegularExpressions.Match m in rx.Matches(expoText))
                {
                    var val = m.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(val)) continue;
                    seen++;
                    if (seen == 1) Add("OSHA", typeLabel, val);
                    else if (seen == 2) Add("Cal/OSHA", typeLabel, val);
                    else break;
                }
            }

            ParsePelSeries("TWA", "TWA");
            ParsePelSeries("STEL", "STEL");
            ParsePelSeries("C(?:eiling)?", "Ceiling");

            // Action Level (1st OSHA, 2nd Cal/OSHA)
            var rxAL = new System.Text.RegularExpressions.Regex(
                @"Action Level\s+(.+?)(?=(?:\s+(?:PEL|REL|TLV|Peak|Action Level)\b|$))",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            int alSeen = 0;
            foreach (System.Text.RegularExpressions.Match m in rxAL.Matches(expoText))
            {
                var val = m.Groups[1].Value.Trim();
                if (string.IsNullOrWhiteSpace(val)) continue;
                alSeen++;
                if (alSeen == 1) Add("OSHA", "Action Level", val);
                else if (alSeen == 2) Add("Cal/OSHA", "Action Level", val);
                else break;
            }

            // REL
            void ParseRel(string kind, string typeLabel)
            {
                var rx = new System.Text.RegularExpressions.Regex(
                    $@"REL[-\s]?{kind}\s+(.+?)(?=(?:\s+(?:REL|PEL|TLV|Action Level|Peak)\b|$))",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var m = rx.Match(expoText);
                if (m.Success)
                {
                    var val = m.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(val)) Add("NIOSH", typeLabel, val);
                }
            }
            ParseRel("TWA", "TWA");
            ParseRel("STEL", "STEL");
            ParseRel("C(?:eiling)?", "Ceiling");

            // TLV
            void ParseTlv(string kind, string typeLabel)
            {
                var rx = new System.Text.RegularExpressions.Regex(
                    $@"TLV[-\s]?{kind}\s+(.+?)(?=(?:\s+(?:TLV|REL|PEL|Action Level|Peak)\b|$))",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var m = rx.Match(expoText);
                if (m.Success)
                {
                    var val = m.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(val)) Add("ACGIH", typeLabel, val);
                }
            }
            ParseTlv("TWA", "TWA");
            ParseTlv("STEL", "STEL");
            ParseTlv("C(?:eiling)?", "Ceiling");

            if (oels.Count == 0)
                unavailable?.Add("OSHA OELs: no matches found in Exposure Limits");

            return oels
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }






        private static bool StringEqualsCas(string a, string b)
        {
            return NormalizeCas(a).Equals(NormalizeCas(b), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeCas(string s) => Regex.Replace(s ?? "", @"\s", "");

        private static string? ResolveHref(string? hrefRaw, string pageUrl)
        {
            if (string.IsNullOrWhiteSpace(hrefRaw)) return null;
            hrefRaw = hrefRaw.Trim();

            try
            {
                // Absolute?
                if (Uri.TryCreate(hrefRaw, UriKind.Absolute, out var abs))
                {
                    // Fix legacy file:// links (seen on some OSHA/NPG pages)
                    if (abs.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
                    {
                        // If host present (file://www.osha.gov/...), use it; else default to OSHA
                        var host = string.IsNullOrWhiteSpace(abs.Host) ? "www.osha.gov" : abs.Host;
                        return $"https://{host}{abs.AbsolutePath}";
                    }
                    return abs.ToString();
                }

                // Bare domains like "www.osha.gov/..." or "osha.gov/..."
                if (hrefRaw.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ||
                    hrefRaw.StartsWith("osha.gov", StringComparison.OrdinalIgnoreCase) ||
                    hrefRaw.StartsWith("cdc.gov", StringComparison.OrdinalIgnoreCase))
                {
                    return hrefRaw.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? hrefRaw
                        : $"https://{hrefRaw.TrimStart('/')}";
                }

                var baseUri = new Uri(pageUrl);

                // Protocol-relative: //www.cdc.gov/...
                if (hrefRaw.StartsWith("//"))
                    return $"{baseUri.Scheme}:{hrefRaw}";

                // Relative to page
                if (Uri.TryCreate(baseUri, hrefRaw, out var rel))
                    return rel.ToString();

                // Root-relative fallback
                if (!hrefRaw.StartsWith("/")) hrefRaw = "/" + hrefRaw;
                if (Uri.TryCreate(new Uri($"{baseUri.Scheme}://{baseUri.Host}"), hrefRaw, out var hostRel))
                    return hostRel.ToString();
            }
            catch { /* fall through */ }

            return null;
        }

        private sealed class TupleIgnoreCaseComparer : IEqualityComparer<(string source, string methodNum)>
        {
            public bool Equals((string source, string methodNum) x, (string source, string methodNum) y) =>
                string.Equals(x.source, y.source, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.methodNum, y.methodNum, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode((string source, string methodNum) obj) =>
                HashCode.Combine(
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.source ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.methodNum ?? string.Empty)
                );
        }

        // NPG fallback: find the chemical page via CAS index, then scrape NIOSH/OSHA methods listed there
        private async Task<List<IhChemicalSamplingMethod>> GetNpgMethodsByCasAsync(
            string cas, List<string>? unavailable, CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();

            // 1) CAS index -> agent page
            var indexUrl = "https://www.cdc.gov/niosh/npg/npgdcas.html";
            var indexHtml = await GetHtmlOrNullAsync(indexUrl, ct);
            if (string.IsNullOrEmpty(indexHtml))
            {
                unavailable?.Add("NIOSH Pocket Guide: CAS index unavailable/blocked");
                return results;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(indexHtml);

            var row = doc.DocumentNode.SelectNodes("//table//tr[td]")?
                .FirstOrDefault(tr =>
                {
                    var cells = tr.SelectNodes("./td")?.Select(td => HtmlEntity.DeEntitize(td.InnerText).Trim()) ?? Enumerable.Empty<string>();
                    return cells.Any(t => NormalizeCas(t) == NormalizeCas(cas));
                });

            if (row == null)
            {
                unavailable?.Add("NIOSH Pocket Guide: CAS not found in index");
                return results;
            }

            var linkNode = row.SelectSingleNode(".//a");
            if (linkNode == null)
            {
                unavailable?.Add("NIOSH Pocket Guide: Agent link missing for this CAS");
                return results;
            }

            var agentUrl = ResolveHref(linkNode.GetAttributeValue("href", string.Empty), indexUrl);
            if (string.IsNullOrWhiteSpace(agentUrl))
            {
                unavailable?.Add("NIOSH Pocket Guide: Could not resolve agent URL");
                return results;
            }

            // 2) Agent page: parse links and text
            var agentHtml = await GetHtmlOrNullAsync(agentUrl!, ct);
            if (string.IsNullOrEmpty(agentHtml))
            {
                unavailable?.Add("NIOSH Pocket Guide: Agent page unavailable/blocked");
                return results;
            }

            var adoc = new HtmlAgilityPack.HtmlDocument();
            adoc.LoadHtml(agentHtml);

            // 2a) Anchors that look like methods (preserves working links)
            foreach (var a in adoc.DocumentNode.SelectNodes("//a") ?? new HtmlAgilityPack.HtmlNodeCollection(null))
            {
                var text = HtmlEntity.DeEntitize(a.InnerText ?? string.Empty).Trim();
                var hrefAbs = ResolveHref(a.GetAttributeValue("href", string.Empty), agentUrl!);

                bool looksNiosh = text.IndexOf("NIOSH", StringComparison.OrdinalIgnoreCase) >= 0
                                  || (hrefAbs?.IndexOf("/niosh/", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                                  || (hrefAbs?.IndexOf("/nmam/", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;

                bool looksOsha = text.IndexOf("OSHA", StringComparison.OrdinalIgnoreCase) >= 0
                                  || (hrefAbs?.IndexOf("osha.gov", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;

                string? methodNum = null;
                var mText = System.Text.RegularExpressions.Regex.Match(text, @"\b(\d{3,4})\b");
                if (mText.Success) methodNum = mText.Groups[1].Value;
                if (methodNum is null && !string.IsNullOrWhiteSpace(hrefAbs))
                {
                    var mHref = System.Text.RegularExpressions.Regex.Match(hrefAbs, @"\b(\d{3,4})(?=\.pdf|/|$)");
                    if (mHref.Success) methodNum = mHref.Groups[1].Value;
                }

                if (looksNiosh && (methodNum != null || hrefAbs != null))
                {
                    results.Add(new IhChemicalSamplingMethod
                    {
                        Source = "NIOSH",
                        MethodId = methodNum is null ? "NMAM" : $"NIOSH {methodNum}",
                        Url = hrefAbs
                    });
                }
                else if (looksOsha)
                {
                    var resolved = ResolveHref(a.GetAttributeValue("href", string.Empty), agentUrl!);
                    resolved = NormalizeOshaMethodUrl(resolved);  // prefer PDF; may null generic index
                    if (resolved == null) continue;

                    var code = DeriveOshaMethodCode(resolved, text);

                    results.Add(new IhChemicalSamplingMethod
                    {
                        Source = "OSHA",
                        MethodId = code,
                        Url = resolved
                    });
                }
            }

            // 2b) Plain-text mentions near "NIOSH" (e.g., "use 7906, 7907, 7908")
            var fullText = HtmlEntity.DeEntitize(adoc.DocumentNode.InnerText ?? string.Empty);

            foreach (System.Text.RegularExpressions.Match hit in System.Text.RegularExpressions.Regex.Matches(fullText, @"NIOSH", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                var start = hit.Index;
                var len = Math.Min(800, fullText.Length - start);
                var seg = fullText.Substring(start, len);
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(seg, @"\b(\d{3,4})\b"))
                {
                    var n = m.Groups[1].Value;
                    if (!results.Any(r => r.Source == "NIOSH" && (r.MethodId?.EndsWith($" {n}") ?? false)))
                    {
                        results.Add(new IhChemicalSamplingMethod
                        {
                            Source = "NIOSH",
                            MethodId = $"NIOSH {n}",
                            Url = null
                        });
                    }
                }
            }

            // De-dup
            return results.GroupBy(m => (m.Source, m.MethodId, m.Url ?? ""))
                          .Select(g => g.First())
                          .ToList();
        }


        private static string? NormalizeOshaMethodUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var u)) return url;
            if (!u.Host.Contains("osha.gov", StringComparison.OrdinalIgnoreCase)) return url;

            var path = u.AbsolutePath;

            // 1) Drop the generic index page (not a method, very JS-heavy)
            if (path.EndsWith("/methods/index.html", StringComparison.OrdinalIgnoreCase))
                return null;

            // 2) Derive an OSHA method code (e.g., ID165SG, ID113) from URL or path
            //    Patterns handled: .../id165sg/id165sg.html, .../id113/id113.pdf, osha-id165sg.pdf
            var m = System.Text.RegularExpressions.Regex.Match(
                path,
                @"\b(id[\-_]?\d+[a-z]*)\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string? code = null;
            if (m.Success)
            {
                code = m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant(); // id165sg
            }
            else
            {
                // Last chance: pull code from filename if it's already in osha-idXXXX form
                var file = System.IO.Path.GetFileName(path); // e.g., osha-id165sg.pdf
                var m2 = System.Text.RegularExpressions.Regex.Match(file, @"osha-(id\d+[a-z]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m2.Success)
                    code = m2.Groups[1].Value.ToLowerInvariant(); // id165sg
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                // 3) Canonical, stable PDF location (works for ID113, ID165SG, etc.)
                return $"https://www.osha.gov/sites/default/files/methods/osha-{code}.pdf";
            }

            // 4) If we didn't recognize a code, still prefer .pdf over .html when under /methods/
            if (path.Contains("/methods/", StringComparison.OrdinalIgnoreCase) &&
                path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                var pdfPath = System.IO.Path.ChangeExtension(path, ".pdf");
                return new UriBuilder(u) { Path = pdfPath }.Uri.ToString();
            }

            return url;
        }


        public async Task<List<IhChemicalSamplingMethod>> GetNmamByChemicalNameAsync(
    string? preferredName,
    IEnumerable<string>? synonyms,
    List<string>? unavailable = null,
    CancellationToken ct = default)
        {
            var results = new List<IhChemicalSamplingMethod>();
            var nameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddName(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return;
                var v = s.Trim();
                if (v.Length < 2) return;
                nameSet.Add(v);
            }

            AddName(preferredName);
            if (synonyms != null) { foreach (var s in synonyms) AddName(s); }
            if (nameSet.Count == 0) return results;

            // Group names by their starting letter (for NMAM method-{letter}.html pages)
            var letters = new HashSet<char>();
            foreach (var n in nameSet)
            {
                var first = n.TrimStart().FirstOrDefault(char.IsLetter);
                if (first != default)
                    letters.Add(char.ToLowerInvariant(first));
            }
            if (letters.Count == 0) return results;

            foreach (var letter in letters)
            {
                var pageUrl = $"https://www.cdc.gov/niosh/nmam/method-{letter}.html";
                var html = await GetHtmlOrNullAsync(pageUrl, ct);
                if (string.IsNullOrEmpty(html))
                {
                    unavailable?.Add($"NIOSH NMAM: letter page '{char.ToUpper(letter)}' unavailable");
                    continue;
                }

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Table rows: "Chemical | Method No. | Method Name"
                foreach (var tr in doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlAgilityPack.HtmlNodeCollection(null))
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count < 2) continue;

                    var chemicalText = HtmlEntity.DeEntitize(tds[0].InnerText ?? "").Trim();

                    // Loose match: contains any of the candidate names (handles "Sulfuric acid")
                    bool match = nameSet.Any(n => chemicalText.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!match) continue;

                    // Method number cell should contain an <a> with text like "7908"
                    var a = tds[1].SelectSingleNode(".//a");
                    if (a == null) continue;

                    var aText = HtmlEntity.DeEntitize(a.InnerText ?? "").Trim();
                    var numM = System.Text.RegularExpressions.Regex.Match(aText, @"\b(\d{3,4})\b");
                    if (!numM.Success) continue;

                    var methodNum = numM.Groups[1].Value;
                    var hrefAbs = ResolveHref(a.GetAttributeValue("href", string.Empty), pageUrl);

                    results.Add(new IhChemicalSamplingMethod
                    {
                        Source = "NIOSH",
                        MethodId = $"NIOSH {methodNum}",
                        Url = hrefAbs
                    });
                }
            }

            // De-dup
            return results
                .GroupBy(m => (m.Source, m.MethodId, m.Url ?? ""))
                .Select(g => g.First())
                .ToList();
        }

        // --- NMAM method whitelist cache ---
        private static readonly SemaphoreSlim _nmamCacheLock = new(1, 1);
        private static DateTime _nmamCacheExpires = DateTime.MinValue;
        private static HashSet<string> _nmamMethodSet = new(StringComparer.OrdinalIgnoreCase);

        private static bool IsCasFragment(string maybeNumber, string cas)
        {
            if (string.IsNullOrWhiteSpace(maybeNumber) || string.IsNullOrWhiteSpace(cas)) return false;
            var firstSeg = System.Text.RegularExpressions.Regex.Match(cas, @"^\s*([0-9]+)-").Groups[1].Value;
            return !string.IsNullOrEmpty(firstSeg) &&
                   string.Equals(maybeNumber, firstSeg, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<HashSet<string>> GetNmamMethodNumbersAsync(CancellationToken ct)
        {
            if (DateTime.UtcNow < _nmamCacheExpires && _nmamMethodSet.Count > 0)
                return _nmamMethodSet;

            await _nmamCacheLock.WaitAsync(ct);
            try
            {
                if (DateTime.UtcNow < _nmamCacheExpires && _nmamMethodSet.Count > 0)
                    return _nmamMethodSet;

                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (char letter = 'a'; letter <= 'z'; letter++)
                {
                    var url = $"https://www.cdc.gov/niosh/nmam/method-{letter}.html";
                    var html = await GetHtmlOrNullAsync(url, ct);
                    if (string.IsNullOrWhiteSpace(html)) continue;

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    foreach (var a in doc.DocumentNode.SelectNodes("//table//tr/td[2]//a") ?? new HtmlAgilityPack.HtmlNodeCollection(null))
                    {
                        var txt = HtmlEntity.DeEntitize(a.InnerText ?? "").Trim();
                        var m = System.Text.RegularExpressions.Regex.Match(txt, @"\b(\d{3,4})\b");
                        if (m.Success) set.Add(m.Groups[1].Value);
                    }
                }

                _nmamMethodSet = set;
                _nmamCacheExpires = DateTime.UtcNow.AddHours(24);
                return _nmamMethodSet;
            }
            finally
            {
                _nmamCacheLock.Release();
            }
        }

        // --- OSHA Methods via Occupational Chemical Database (by CAS) ---
        private async Task<List<IhChemicalSamplingMethod>> GetOshaFromChemicalDbByCasAsync(
            string cas,
            CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();

            // Paginated search results – find the per-chemical page for this CAS
            for (int page = 1; page <= 30; page++)
            {
                var searchUrl = $"https://www.osha.gov/chemicaldata/search?page={page}";
                var html = await GetHtmlOrNullAsync(searchUrl, ct);
                if (string.IsNullOrWhiteSpace(html)) continue;

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Row whose text contains the CAS (any cell)
                var tr = doc.DocumentNode.SelectSingleNode($"//table//tr[td[contains(normalize-space(.), '{cas}')]]");
                if (tr == null) continue;

                // Link to the chemical detail page (/chemicaldata/{id})
                var a = tr.SelectSingleNode(".//a[contains(@href, '/chemicaldata/')]");
                if (a == null) break;

                var chemUrl = ResolveHref(a.GetAttributeValue("href", string.Empty), searchUrl);
                var chemHtml = await GetHtmlOrNullAsync(chemUrl, ct);
                if (string.IsNullOrWhiteSpace(chemHtml)) break;

                var cdoc = new HtmlAgilityPack.HtmlDocument();
                cdoc.LoadHtml(chemHtml);

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // --- 1) Capture linked OSHA methods on the page
                foreach (var link in cdoc.DocumentNode.SelectNodes("//a[contains(., 'OSHA') or contains(translate(@href,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'), '/methods/')]")
                         ?? new HtmlAgilityPack.HtmlNodeCollection(null))
                {
                    var anchorText = HtmlEntity.DeEntitize(link.InnerText ?? "").Trim();
                    var hrefAbs = ResolveHref(link.GetAttributeValue("href", string.Empty), chemUrl);
                    hrefAbs = NormalizeOshaMethodUrl(hrefAbs); // maps to canonical PDF when possible

                    var code = DeriveOshaMethodCode(hrefAbs, anchorText);
                    if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("ID", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (seen.Add(code))
                    {
                        // Ensure canonical PDF if href wasn't a method link
                        if (string.IsNullOrWhiteSpace(hrefAbs) || !hrefAbs.Contains("/methods/", StringComparison.OrdinalIgnoreCase))
                        {
                            var slug = code.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant(); // id113
                            hrefAbs = $"https://www.osha.gov/sites/default/files/methods/osha-{slug}.pdf";
                        }

                        results.Add(new IhChemicalSamplingMethod
                        {
                            Source = "OSHA",
                            MethodId = code,   // e.g., ID113
                            Url = hrefAbs
                        });
                    }
                }

                // --- 2) Capture PLAIN-TEXT OSHA method codes (e.g., “OSHA ID-113”) and canonicalize
                var fullText = HtmlEntity.DeEntitize(cdoc.DocumentNode.InnerText ?? string.Empty);
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                             fullText, @"OSHA\s+ID[-\s]?(\d+[A-Za-z]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    var code = "ID" + m.Groups[1].Value.ToUpperInvariant(); // e.g., ID113, ID165SG
                    if (!seen.Add(code)) continue;

                    var slug = code.ToLowerInvariant(); // id113
                    var url = $"https://www.osha.gov/sites/default/files/methods/osha-{slug}.pdf";

                    results.Add(new IhChemicalSamplingMethod
                    {
                        Source = "OSHA",
                        MethodId = code,
                        Url = url
                    });
                }

                break; // processed the matched CAS's page; no need to scan more pages
            }

            // De-dup (defensive) using a tuple-key dictionary with case-insensitive comparer
            var dedup = new Dictionary<(string source, string methodId), IhChemicalSamplingMethod>(new TupleIgnoreCaseComparer());
            foreach (var r in results)
            {
                var key = (r.Source ?? "", r.MethodId ?? "");
                if (!dedup.TryGetValue(key, out var existing))
                {
                    dedup[key] = r;
                }
                else if (string.IsNullOrWhiteSpace(existing.Url) && !string.IsNullOrWhiteSpace(r.Url))
                {
                    existing.Url = r.Url; // prefer rows with URLs
                }
            }
            return dedup.Values.ToList();
        }



        private async Task<bool> IsValidNioshMethodAsync(string? num, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(num)) return false;
            var set = await GetNmamMethodNumbersAsync(ct);
            return set.Contains(num);
        }

        // Public wrapper used by the service layer
        public Task<bool> IsValidNioshMethodNumberAsync(string num, CancellationToken ct = default)
            => IsValidNioshMethodAsync(num, ct);


        // using System.Diagnostics;  // <- make sure this is at the top of the file

        private async Task<string?> FindOshaChemicalUrlByCasAsync(string cas, CancellationToken ct)
        {
            // --- helpers for CAS matching ---
            static string NormalizeHyphens(string s) =>
                string.IsNullOrEmpty(s) ? s
                : System.Text.RegularExpressions.Regex.Replace(s, "[\u2010-\u2015\u2212\uFE63\uFF0D]", "-"); // odd hyphens → '-'

            static string DigitsOnly(string s) =>
                new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

            var casNorm = NormalizeHyphens(cas?.Trim() ?? "");
            var casDigits = DigitsOnly(casNorm);

            string? chemUrl = null;

            // FIX #1: search pages start at 1 on some deployments; expand range
            for (int page = 1; page <= 60 && chemUrl == null; page++)
            {
                var searchUrl = $"https://www.osha.gov/chemicaldata/search?page={page}";
                var html = await GetHtmlOrNullAsync(searchUrl, ct);
                if (string.IsNullOrWhiteSpace(html))
                {
                    Debug.WriteLine($"[OSHA SEARCH] page={page} html=null");
                    continue;
                }

                try
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    // Try structured table first (CAS is usually the 2nd <td>)
                    var rows = doc.DocumentNode.SelectNodes("//table//tr[td]");
                    if (rows != null)
                    {
                        foreach (var tr in rows)
                        {
                            var link = tr.SelectSingleNode(".//a[contains(@href, '/chemicaldata/')]");
                            var casTd = tr.SelectSingleNode("./td[2]");
                            if (link == null || casTd == null) continue;

                            var rowCas = HtmlAgilityPack.HtmlEntity.DeEntitize(casTd.InnerText ?? string.Empty).Trim();
                            rowCas = NormalizeHyphens(rowCas);

                            if (DigitsOnly(rowCas) == casDigits)
                            {
                                chemUrl = ResolveHref(link.GetAttributeValue("href", string.Empty), searchUrl);
                                break;
                            }
                        }
                    }
                }
                catch { /* fall back below */ }

                // Fallback: regex around each /chemicaldata/{id} anchor and look for CAS text nearby
                if (chemUrl == null)
                {
                    foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                                 html, "<a[^>]+href=[\"'](?<href>/chemicaldata/\\d+)[\"'][^>]*>(?<text>.*?)</a>",
                                 System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline))
                    {
                        int idx = m.Index;
                        int start = Math.Max(0, idx - 1200);
                        int len = Math.Min(html.Length - start, 2400);
                        var window = html.Substring(start, len);

                        var windowNorm = NormalizeHyphens(HtmlAgilityPack.HtmlEntity.DeEntitize(window));
                        var windowDigits = DigitsOnly(windowNorm);

                        if (windowNorm.IndexOf(casNorm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            windowDigits.IndexOf(casDigits, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            chemUrl = ResolveHref(m.Groups["href"].Value, searchUrl);
                            break;
                        }
                    }
                }

                Debug.WriteLine($"[OSHA SEARCH] page={page} foundUrl={(chemUrl ?? "null")}");
            }

            return chemUrl;
        }

        // ===== NIOSH Pocket Guide OELs (by CAS) =====
        public async Task<List<IhChemicalOel>> GetOelsFromNpgByCasAsync(
            string cas,
            List<string>? unavailable = null,
            CancellationToken ct = default)
        {
            var results = new List<IhChemicalOel>();

            // 1) Locate NPG detail page for this CAS
            var url = await FindNpgUrlByCasAsync(cas, ct);
            Debug.WriteLine($"[NPG URL] len={(url?.Length ?? 0)} peek=\"{(url is null ? "" : (url.Length > 60 ? url[..60] : url))}\"");
            if (string.IsNullOrWhiteSpace(url))
            {
                unavailable?.Add("NIOSH NPG: no page found for this CAS");
                return results;
            }

            // 2) Fetch + flatten page text
            var html = await GetHtmlOrNullAsync(url!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add("NIOSH NPG: page fetch failed/blocked");
                return results;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var fullText = HtmlAgilityPack.HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? string.Empty);
            fullText = Regex.Replace(fullText, @"\s+", " ").Trim();
            Debug.WriteLine($"[NPG TEXT] len={fullText.Length} peek=\"{fullText.Substring(0, Math.Min(200, fullText.Length))}\"");

            // 3) Slice “REL / PEL / TLV / Cal/OSHA” blocks
            static string ExtractBlock(string labelRegex, string text)
            {
                var rx = new Regex(
                    labelRegex + @"\s*(.*?)(?=OSHA\s+PEL|ACGIH\s+TLV|Cal/OSHA|California\s+OSHA|$)",
                    RegexOptions.IgnoreCase);
                var m = rx.Match(text);
                return m.Success ? m.Groups[1].Value.Trim() : string.Empty;
            }

            var relBlock = ExtractBlock(@"NIOSH\s+REL", fullText);
            var pelBlock = ExtractBlock(@"OSHA\s+PEL(?:\s*\[[^\]]*\])?", fullText);
            var tlvBlock = ExtractBlock(@"ACGIH\s+TLV", fullText);
            var calBlock = ExtractBlock(@"(?:Cal/OSHA|California\s+OSHA)", fullText);

            Debug.WriteLine($"[REL block] len={relBlock.Length} peek=\"{relBlock.Substring(0, Math.Min(120, relBlock.Length))}\"");
            Debug.WriteLine($"[PEL block] len={pelBlock.Length} peek=\"{pelBlock.Substring(0, Math.Min(120, pelBlock.Length))}\"");
            Debug.WriteLine($"[TLV block] len={tlvBlock.Length} peek=\"{tlvBlock.Substring(0, Math.Min(120, tlvBlock.Length))}\"");
            Debug.WriteLine($"[CAL block] len={calBlock.Length} peek=\"{calBlock.Substring(0, Math.Min(120, calBlock.Length))}\"");

            // 4) Parse limits inside each block with a simpler/more tolerant extractor
            results.AddRange(ParseLimitsFromBlockSimple("NIOSH", relBlock));
            results.AddRange(ParseLimitsFromBlockSimple("OSHA", pelBlock));
            results.AddRange(ParseLimitsFromBlockSimple("ACGIH", tlvBlock));
            results.AddRange(ParseLimitsFromBlockSimple("Cal/OSHA", calBlock));

            Debug.WriteLine($"[OEL count before dedupe] {results.Count}");

            // 5) Dedupe triplet (Source|Type|Value)
            results = results
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            Debug.WriteLine($"[OEL after parse] {results.Count}");
            foreach (var r in results)
                Debug.WriteLine($"[OEL] {r.Source} {r.Type} {r.Value} {(string.IsNullOrWhiteSpace(r.Notes) ? "" : r.Notes)}");

            if (results.Count == 0)
                unavailable?.Add("NPG: no OEL tokens matched");

            return results;
        }


        // --- helpers ---

        // Pulls the text immediately following a label like "NIOSH REL" up to the next label (or end)
        private static string ExtractBlockFromNpgText(string labelRegex, string text)
        {
            var rx = new Regex(
                labelRegex + @"\s*(.*?)(?=OSHA\s+PEL|ACGIH\s+TLV|Cal/OSHA|California\s+OSHA|$)",
                RegexOptions.IgnoreCase);
            var m = rx.Match(text);
            return m.Success ? m.Groups[1].Value.Trim() : string.Empty;
        }

        // Parses TWA / STEL (incl. “ST”) / Ceiling (“C” or “Ceiling”), normalizes units
        private static List<IhChemicalOel> ParseLimitsFromOelBlock(string source, string? block)
        {
            var list = new List<IhChemicalOel>();
            if (string.IsNullOrWhiteSpace(block)) return list;

            var text = Regex.Replace(block, @"\s+", " ").Trim();

            // Accept mg/m3, mg/m^3, ug/m3, µg/m3, μg/m3 (normalize later)
            var rx = new Regex(
                @"(?<![A-Za-z0-9])(?<kind>TWA|STEL|ST|C(?:eiling)?)\s*[:\-]?\s*(?<val>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg\s*/\s*m(?:\^?3)|ug\s*/\s*m(?:\^?3)|µg\s*/\s*m(?:\^?3)|μg\s*/\s*m(?:\^?3))(?<tail>[^.;)]{0,40})",
                RegexOptions.IgnoreCase
            );

            foreach (Match m in rx.Matches(text))
            {
                var kindRaw = m.Groups["kind"].Value;
                var val = m.Groups["val"].Value;
                var unitRaw = m.Groups["unit"].Value;
                var tail = m.Groups["tail"].Success ? m.Groups["tail"].Value : "";

                if (string.IsNullOrWhiteSpace(val)) continue;

                string type;
                var k = kindRaw.Trim().ToUpperInvariant();
                if (k == "TWA") type = "TWA";
                else if (k == "ST" || k == "STEL") type = "STEL";
                else type = "Ceiling";

                var unit = NormalizeUnit(unitRaw);

                string? notes = null;
                // if block mentions 15-minute in the immediate tail, keep it
                if (!string.IsNullOrWhiteSpace(tail) && tail.IndexOf("15", StringComparison.Ordinal) >= 0)
                    notes = tail.Trim();

                list.Add(new IhChemicalOel
                {
                    Source = source,
                    Type = type,
                    Value = $"{val} {unit}",
                    Notes = notes
                });
            }

            return list;
        }

        /// <summary>
        /// Tolerant parser: pulls TWA, ST/STEL, C/Ceil/Ceiling with ppm/ppb/mg/m3/µg/m3 units.
        /// Ignores the carcinogen flag “Ca” and trims bracket notes.
        /// </summary>
        // Parse simple OEL patterns inside a text "block" (REL/PEL/TLV/Cal/OSHA section).
        // Now also captures an immediate bracketed note after the limit, e.g. "C 0.1 ppm [15-minute]".
        public static List<IhChemicalOel> ParseLimitsFromBlockSimple(string source, string? blockText)
        {
            var list = new List<IhChemicalOel>();
            if (string.IsNullOrWhiteSpace(blockText)) return list;

            // Normalize whitespace
            var txt = System.Text.RegularExpressions.Regex.Replace(blockText, @"\s+", " ").Trim();

            // Optional type token (TWA/STEL/ST/C/Ceil/Ceiling), then numeric value + unit
            var rx = new System.Text.RegularExpressions.Regex(
                @"(?:(?<type>\bTWA\b|\bSTEL\b|\bST\b|\bC(?:eil(?:ing)?)?\b)\s*)?"
              + @"(?<value>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            foreach (System.Text.RegularExpressions.Match m in rx.Matches(txt))
            {
                var t = m.Groups["type"].Value;
                var v = m.Groups["value"].Value;
                var u = m.Groups["unit"].Value;

                string normType;
                if (System.Text.RegularExpressions.Regex.IsMatch(t, @"^twa$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    normType = "TWA";
                else if (System.Text.RegularExpressions.Regex.IsMatch(t, @"^(st|stel)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    normType = "STEL";
                else if (System.Text.RegularExpressions.Regex.IsMatch(t, @"^c", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) // C / Ceil / Ceiling
                    normType = "Ceiling";
                else
                    normType = string.IsNullOrWhiteSpace(t) ? "LIMIT" : t.Trim();

                var valueStr = $"{v} {u}".Replace("m^3", "m3");

                // Capture an immediate bracketed note right after the match: e.g., "C 0.1 ppm [15-minute]"
                string? note = null;
                var after = txt.Substring(m.Index + m.Length);
                var mNote = System.Text.RegularExpressions.Regex.Match(after, @"^\s*\[(?<note>[^\]]+)\]");
                if (mNote.Success)
                    note = mNote.Groups["note"].Value.Trim();

                list.Add(new IhChemicalOel
                {
                    Source = source,   // "NIOSH", "OSHA", "ACGIH", "Cal/OSHA"
                    Type = normType, // "TWA", "STEL", "Ceiling", or "LIMIT"
                    Value = valueStr, // "0.1 ppm", "0.016 ppm", etc.
                    Notes = note      // e.g., "15-minute"
                });
            }

            // De-dupe (Source|Type|Value|Notes)
            return list
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}|{x.Notes}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }


        private static string NormalizeUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit)) return "";

            var u = unit.ToLowerInvariant().Replace(" ", "");
            // unify all mg/m^3 or mg/m3 variants
            if (u.Contains("mg/m")) return "mg/m3";
            // unify ug/µg/μg per m^3 variants
            if (u.Contains("ug/m") || u.Contains("µg/m") || u.Contains("μg/m")) return "µg/m3";
            if (u.Contains("ppm")) return "ppm";
            if (u.Contains("ppb")) return "ppb";
            return unit.Trim();
        }

        private static string HtmlToPlain(string html)
        {
            // Strip scripts/styles, tags, decode entities, normalize whitespace
            html = Regex.Replace(html, "<script.*?</script>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<style.*?</style>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        // Extract REL/PEL/IDLH like "TWA 0.75 ppm", "STEL 2 ppm", "Ceiling 0.3 ppm", "IDLH 20 ppm"
        private static List<IhChemicalOel> ParseOelsFromNpgHtml(string html)
        {
            var text = HtmlToPlain(html);

            var results = new List<IhChemicalOel>();

            // Match type + number + unit
            var rx = new Regex(
                @"(?<type>TWA|STEL|Ceiling|Peak|IDLH)\s*[:\-]?\s*(?<val>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)",
                RegexOptions.IgnoreCase);

            foreach (Match m in rx.Matches(text))
            {
                var type = m.Groups["type"].Value;
                var val = $"{m.Groups["val"].Value} {m.Groups["unit"].Value}";

                // Look back a bit to infer source (NIOSH/REL vs OSHA/PEL vs ACGIH/TLV)
                var start = Math.Max(0, m.Index - 180);
                var ctx = text.Substring(start, m.Index - start);

                string src = "NIOSH"; // default
                if (Regex.IsMatch(ctx, @"OSHA|PEL", RegexOptions.IgnoreCase)) src = "OSHA";
                else if (Regex.IsMatch(ctx, @"ACGIH|TLV", RegexOptions.IgnoreCase)) src = "ACGIH";
                else if (Regex.IsMatch(ctx, @"NIOSH|REL", RegexOptions.IgnoreCase)) src = "NIOSH";

                if (type.Equals("IDLH", StringComparison.OrdinalIgnoreCase)) src = "NIOSH"; // IDLH is a NIOSH value

                results.Add(new IhChemicalOel
                {
                    Source = src,
                    Type = type.Equals("Peak", StringComparison.OrdinalIgnoreCase) ? "Peak" :
                           type.Equals("Ceiling", StringComparison.OrdinalIgnoreCase) ? "Ceiling" :
                           type.Equals("TWA", StringComparison.OrdinalIgnoreCase) ? "TWA" :
                           type.Equals("STEL", StringComparison.OrdinalIgnoreCase) ? "STEL" :
                           "IDLH",
                    Value = val,
                    Notes = null
                });
            }

            // Dedup by Source|Type|Value
            return results
                .GroupBy(x =>
                    $"{(x.Source ?? "").ToLowerInvariant()}|" +
                    $"{(x.Type ?? "").ToLowerInvariant()}|" +
                    $"{(x.Value ?? "").ToLowerInvariant()}")
                .Select(g => g.First())
                .OrderBy(x => x.Source)
                .ThenBy(x => x.Type)
                .ToList();
        }


        // ---------- CAS → NPG page resolver (robust, non-regex dependent on tag shape) ----------
        public async Task<string?> ResolveNpgPageUrlByCasAsync(string cas, CancellationToken ct = default)
        {
            const string idxUrl = "https://www.cdc.gov/niosh/npg/npgdcas.html";
            string html;
            try { html = await _http.GetStringAsync(idxUrl, ct); }
            catch { return null; }
            if (string.IsNullOrWhiteSpace(html)) return null;

            var norm = NormalizeHtml(html);

            // Find the CAS string in the index page
            var pos = norm.IndexOf(cas, StringComparison.OrdinalIgnoreCase);
            if (pos < 0) return null;

            // Grab the next href= after the CAS (within a short window)
            var href = TryExtractHrefAfter(norm, pos);
            if (string.IsNullOrWhiteSpace(href)) return null;

            // Make absolute
            if (!href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                href = new Uri(new Uri(idxUrl), href).ToString();

            return href;
        }

        private static string NormalizeHtml(string s)
        {
            if (s is null) return string.Empty;
            return s
                .Replace("&nbsp;", " ")
                .Replace("\u00A0", " ")
                .Replace("\u2010", "-").Replace("\u2011", "-").Replace("\u2012", "-")
                .Replace("\u2013", "-").Replace("\u2014", "-");
        }

        private static string? TryExtractHrefAfter(string html, int startIndex)
        {
            // look ahead ~400 chars for the first href=
            var i = html.IndexOf("href=", startIndex, StringComparison.OrdinalIgnoreCase);
            if (i < 0 || i - startIndex > 400) return null;

            i += 5; // after href=
            if (i >= html.Length) return null;

            char q = html[i];
            if (q == '"' || q == '\'')
            {
                i++;
                var j = html.IndexOf(q, i);
                return (j > i) ? html.Substring(i, j - i) : null;
            }
            // unquoted href=
            var end = html.IndexOfAny(new[] { ' ', '>' }, i);
            if (end < 0) end = html.Length;
            return html.Substring(i, end - i);
        }



        private static string ExtractLimitBlock(string text, string heading, string[] stopHeadings)
        {
            var (start, end) = FindHeadingRange(text, heading, stopHeadings);
            if (start < 0 || end <= start) return string.Empty;

            // Keep the block compact
            var block = text.Substring(start, end - start);
            // Trim to the first couple of lines after the heading marker if it runs long
            return block;
        }

        private static (int start, int end) FindHeadingRange(string text, string heading, string[] stops)
        {
            var start = text.IndexOf(heading, StringComparison.OrdinalIgnoreCase);
            if (start < 0) return (-1, -1);

            var end = text.Length;
            foreach (var stop in stops)
            {
                var i = text.IndexOf(stop, start + heading.Length, StringComparison.OrdinalIgnoreCase);
                if (i >= 0 && i < end) end = i;
            }
            // Also cap to a sane window to prevent runaway (many pages fit within 1200 chars)
            end = Math.Min(end, start + 1200);
            return (start, end);
        }

        private static (int start, int end) FindHeadingRangeByHeadings(string text,string[] headings,string[] stops,int hardCap = 4000)
        {
            var start = -1;
            foreach (var h in headings)
            {
                var i = text.IndexOf(h, StringComparison.OrdinalIgnoreCase);
                if (i >= 0 && (start < 0 || i < start)) start = i;
            }
            if (start < 0) return (-1, -1);

            var end = text.Length;
            foreach (var stop in stops)
            {
                var j = text.IndexOf(stop, start + 1, StringComparison.OrdinalIgnoreCase);
                if (j >= 0 && j < end) end = j;
            }

            // Safety cap (wider than before so we don't cut off STEL/AL lines)
            end = Math.Min(end, start + hardCap);
            return (start, Math.Max(start, end));
        }

        private static string ExtractLimitBlockByHeadings(string text, string[] headingAlts, string[] stopHeadings)
        {
            var (start, end) = FindHeadingRangeByHeadings(text, headingAlts, stopHeadings);
            if (start < 0 || end <= start) return string.Empty;
            return text.Substring(start, end - start);
        }

        private static readonly Regex SegmentRx = new Regex(
            // optional kind (TWA/STEL/C/AL), then numeric + unit
            @"(?:(?<kind>TWA|STEL|Ceiling|Ceil|C|Action\s*Level|AL)\b[^0-9]{0,20})?(?<val>[0-9]+(?:\.[0-9]+)?)\s*(?<unit>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex LimitLineRx = new Regex(
            @"(?:(TWA|STEL|Ceiling|C|PEL|REL)\s*)?([0-9]+(?:\.[0-9]+)?)\s*(ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private static List<IhChemicalOel> ParseLimitsFromBlock(string source, string? block)
        {
            var list = new List<IhChemicalOel>();
            if (string.IsNullOrWhiteSpace(block)) return list;

            // Flatten whitespace so regex is easier
            var text = System.Text.RegularExpressions.Regex.Replace(block, @"\s+", " ").Trim();

            // Matches forms like:
            //   TWA 0.75 ppm
            //   ST 2 ppm
            //   STEL 2 ppm
            //   C 0.1 ppm
            //   Ceiling 0.1 ppm [15-minute]
            //
            // We capture a short trailing tail (up to ~40 chars) to sniff things like "15-minute"
            var rx = new System.Text.RegularExpressions.Regex(
                @"(?<![A-Za-z0-9])(?<kind>TWA|STEL|ST|C(?:eiling)?)\s*[:\-]?\s*(?<val>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg\s*/\s*m(?:\^?3|\u00B3)|(?:\u00B5|\u03BC)g\s*/\s*m(?:\^?3|\u00B3))(?<tail>[^.;)]{0,40})",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            foreach (System.Text.RegularExpressions.Match m in rx.Matches(text))
            {
                var kindRaw = m.Groups["kind"].Value;
                var val = m.Groups["val"].Value;
                var unitRaw = m.Groups["unit"].Value;
                var tail = (m.Groups["tail"].Success ? m.Groups["tail"].Value : null) ?? "";

                if (string.IsNullOrWhiteSpace(val)) continue;

                // Normalize type
                string type;
                var k = kindRaw.Trim().ToUpperInvariant();
                if (k == "TWA") type = "TWA";
                else if (k == "ST" || k == "STEL") type = "STEL";
                else /* C / Ceiling */ type = "Ceiling";

                // Normalize unit (ppm/ppb/mg/m3/µg/m3)
                var unit = NormalizeUnit(unitRaw);

                // Optional short note (e.g., “15-minute”)
                string? notes = null;
                if (!string.IsNullOrWhiteSpace(tail) && tail.IndexOf("15", StringComparison.Ordinal) >= 0)
                    notes = tail.Trim();

                list.Add(new IhChemicalOel
                {
                    Source = source,
                    Type = type,
                    Value = $"{val} {unit}",
                    Notes = notes
                });
            }

            return list;
        }



        // ======== NPG OELs: parse exposure limit lines ========
        private List<IhChemicalOel> ParseNpgOelsFromHtml(string html, string sourceUrl)
        {
            // Quick ‘plaintext’ view helps with regex
            var text = StripTags(html);
            text = NormalizeWs(text);

            // Extract the "Exposure Limits" block (loose; robust to layout changes)
            // We take up to the next major section header ("NIOSH REL", "Odor", etc.) if present.
            string block = text;
            var expIdx = text.IndexOf("Exposure Limits", StringComparison.OrdinalIgnoreCase);
            if (expIdx >= 0)
            {
                block = text.Substring(expIdx);
                // stop at some likely next headings to keep the block compact
                var cut = IndexOfAny(block,
                    new[] { "References", "Synonyms", "IDLH", "Odor threshold", "Measurement Methods", "Measurement methods" });
                if (cut > 0) block = block.Substring(0, cut);
            }

            // We’ll look for lines that begin with NIOSH REL / OSHA PEL / ACGIH TLV
            var lines = block.Split('\n').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();

            var list = new List<IhChemicalOel>();
            foreach (var line in lines)
            {
                // Identify which source the line belongs to
                string? src = null;
                if (Regex.IsMatch(line, @"^\s*NIOSH\s+REL\b", RegexOptions.IgnoreCase)) src = "NIOSH";
                else if (Regex.IsMatch(line, @"^\s*OSHA\s+PEL\b", RegexOptions.IgnoreCase)) src = "OSHA";
                else if (Regex.IsMatch(line, @"^\s*ACGIH\s+TLV\b", RegexOptions.IgnoreCase)) src = "ACGIH";
                else continue;

                // Pull out individual limit types if present (TWA, STEL, Ceiling)
                // Examples:
                //   "NIOSH REL: TWA 0.016 ppm; STEL 0.1 ppm"
                //   "OSHA PEL: Ceiling 0.1 ppm"
                //   "ACGIH TLV: TWA 0.1 mg/m3 (as ...)"
                ExtractLimitsForLine(src, line, list);
            }

            // If nothing matched, try a last-resort pattern that grabs any numeric limits on the three key lines
            if (list.Count == 0)
            {
                foreach (var line in lines)
                {
                    string? src = null;
                    if (line.StartsWith("NIOSH REL", StringComparison.OrdinalIgnoreCase)) src = "NIOSH";
                    else if (line.StartsWith("OSHA PEL", StringComparison.OrdinalIgnoreCase)) src = "OSHA";
                    else if (line.StartsWith("ACGIH TLV", StringComparison.OrdinalIgnoreCase)) src = "ACGIH";
                    if (src == null) continue;

                    var val = ExtractFirstUnitValue(line);
                    if (val != null)
                    {
                        list.Add(new IhChemicalOel
                        {
                            Source = src,
                            Type = "Limit",
                            Value = val,
                            Notes = line
                        });
                    }
                }
            }

            // De-dup defensively
            return list
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}".ToLowerInvariant())
                .Select(g => g.First())
                .OrderBy(x => x.Source)
                .ThenBy(x => x.Type)
                .ToList();
        }

        // ----- helpers -----
        private static void ExtractLimitsForLine(string source, string line, List<IhChemicalOel> list)
        {
            // TWA
            foreach (Match m in Regex.Matches(line, @"\bTWA\b[^0-9]{0,20}(?<v>[0-9]+(?:\.[0-9]+)?)\s*(?<u>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)", RegexOptions.IgnoreCase))
            {
                list.Add(new IhChemicalOel { Source = source, Type = "TWA", Value = $"{m.Groups["v"].Value} {m.Groups["u"].Value}", Notes = line });
            }
            // STEL
            foreach (Match m in Regex.Matches(line, @"\bSTEL\b[^0-9]{0,20}(?<v>[0-9]+(?:\.[0-9]+)?)\s*(?<u>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)", RegexOptions.IgnoreCase))
            {
                list.Add(new IhChemicalOel { Source = source, Type = "STEL", Value = $"{m.Groups["v"].Value} {m.Groups["u"].Value}", Notes = line });
            }
            // Ceiling
            foreach (Match m in Regex.Matches(line, @"\bCeiling\b[^0-9]{0,20}(?<v>[0-9]+(?:\.[0-9]+)?)\s*(?<u>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)", RegexOptions.IgnoreCase))
            {
                list.Add(new IhChemicalOel { Source = source, Type = "Ceiling", Value = $"{m.Groups["v"].Value} {m.Groups["u"].Value}", Notes = line });
            }
            // If we didn’t match a specific subtype but there *is* a numeric w/ unit, record a generic
            if (!list.Any(x => x.Notes == line) && LooksLikeLimit(line))
            {
                var val = ExtractFirstUnitValue(line);
                if (val != null)
                    list.Add(new IhChemicalOel { Source = source, Type = "Limit", Value = val, Notes = line });
            }
        }

        // ===== Helpers used by NPG OEL parsing =====
        private static string StripTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            // Turn structural tags into newlines BEFORE stripping everything else.
            html = Regex.Replace(html, @"<(?:br|p|li|tr|th|td|h[1-6])\b[^>]*>", "\n", RegexOptions.IgnoreCase);

            // Remove remaining tags
            html = Regex.Replace(html, "<[^>]+>", " ");

            // Decode some common entities / unicode punctuation
            html = html.Replace("&nbsp;", " ")
                       .Replace("\u00A0", " ")
                       .Replace("\u2010", "-").Replace("\u2011", "-").Replace("\u2012", "-")
                       .Replace("\u2013", "-").Replace("\u2014", "-");

            // Collapse whitespace/newlines
            html = Regex.Replace(html, @"[ \t\r\f]+", " ");
            html = Regex.Replace(html, @"\n\s*\n", "\n");
            return html.Trim();
        }

        private static string NormalizeWs(string s)
        {
            s = s.Replace("\r", " ");
            s = Regex.Replace(s, @"[ \t]+", " ");
            s = Regex.Replace(s, @"\n\s+", "\n");
            return s;
        }

        private static int IndexOfAny(string s, string[] candidates)
        {
            var best = -1;
            foreach (var c in candidates)
            {
                var i = s.IndexOf(c, StringComparison.OrdinalIgnoreCase);
                if (i >= 0 && (best < 0 || i < best)) best = i;
            }
            return best;
        }

        private static bool LooksLikeLimit(string value)
        {
            return Regex.IsMatch(value, @"\b(ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)\b", RegexOptions.IgnoreCase)
                && Regex.IsMatch(value, @"\b\d+(\.\d+)?\b");
        }

        private static string? ExtractFirstUnitValue(string line)
        {
            var m = Regex.Match(line, @"(?<v>\d+(?:\.\d+)?)\s*(?<u>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3)", RegexOptions.IgnoreCase);
            if (m.Success) return $"{m.Groups["v"].Value} {m.Groups["u"].Value}";
            return null;
        }

        // ------------------------------------------------------
        // Find NIOSH Pocket Guide (NPG) detail URL by CAS number
        // ------------------------------------------------------
        public async Task<string?> FindNpgUrlByCasAsync(string cas, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cas)) return null;
            cas = cas.Trim();

            // Primary CAS index; fallbacks are just in case the structure changes
            var indexCandidates = new[]
            {
        "https://www.cdc.gov/niosh/npg/npgdcas.html",   // current CAS index (most likely)
        "https://www.cdc.gov/niosh/npg/",              // fallback (rarely helps, but cheap to try)
    };

            foreach (var url in indexCandidates)
            {
                ct.ThrowIfCancellationRequested();

                var html = await GetHtmlOrNullAsync(url, ct);
                if (string.IsNullOrWhiteSpace(html))
                {
                    Debug.WriteLine($"[NPG FIND] failed to fetch index: {url}");
                    continue;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // 1) Best path: find the cell that contains the exact CAS text, then locate the row's NPG link.
                var casNode = doc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{cas}')]");
                if (casNode != null)
                {
                    // climb to the <tr> containing this CAS (if it's a table)
                    var row = casNode;
                    for (int i = 0; i < 6 && row != null && !row.Name.Equals("tr", StringComparison.OrdinalIgnoreCase); i++)
                        row = row.ParentNode;

                    HtmlNode? linkNode = null;
                    if (row != null)
                    {
                        linkNode = row.SelectSingleNode(".//a[contains(@href,'npgd') and contains(@href,'.html')]");
                    }

                    // if not found in the same row, search a few ancestor levels for a nearby link
                    if (linkNode == null)
                    {
                        var scope = casNode;
                        for (int i = 0; i < 4 && scope != null && linkNode == null; i++)
                        {
                            scope = scope.ParentNode;
                            if (scope != null)
                                linkNode = scope.SelectSingleNode(".//a[contains(@href,'npgd') and contains(@href,'.html')]");
                        }
                    }

                    if (linkNode != null)
                    {
                        var href = linkNode.GetAttributeValue("href", "");
                        var abs = MakeAbsoluteNpgUrl(href);
                        Debug.WriteLine($"[NPG FIND] CAS={cas} => {abs}");
                        return abs;
                    }
                }

                // 2) Fallback: scan all anchors that look like NPG detail links and see if their nearby text contains the CAS
                var anchors = doc.DocumentNode.SelectNodes("//a[contains(@href,'npgd') and contains(@href,'.html')]");
                if (anchors != null)
                {
                    foreach (var a in anchors)
                    {
                        var ctx = a.ParentNode?.InnerText ?? "";
                        if (ctx.IndexOf(cas, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var abs = MakeAbsoluteNpgUrl(a.GetAttributeValue("href", ""));
                            Debug.WriteLine($"[NPG FIND: parent ctx] CAS={cas} => {abs}");
                            return abs;
                        }

                        // try row text if in a table
                        var row = a;
                        for (int i = 0; i < 6 && row != null && !row.Name.Equals("tr", StringComparison.OrdinalIgnoreCase); i++)
                            row = row.ParentNode;

                        if (row != null)
                        {
                            var rowText = row.InnerText ?? "";
                            if (rowText.IndexOf(cas, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var abs = MakeAbsoluteNpgUrl(a.GetAttributeValue("href", ""));
                                Debug.WriteLine($"[NPG FIND: row ctx] CAS={cas} => {abs}");
                                return abs;
                            }
                        }
                    }
                }
            }

            Debug.WriteLine($"[NPG FIND] CAS={cas} => not found");
            return null;
        }

        // Build an absolute URL from a relative NPG href
        private static string MakeAbsoluteNpgUrl(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return "";
            href = href.Trim();

            if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return href;

            if (href.StartsWith("//"))
                return "https:" + href;

            // Many NPG links are relative like "npgd0293.html"
            if (!href.StartsWith("/"))
                href = "/niosh/npg/" + href.TrimStart('/');

            return "https://www.cdc.gov" + href;
        }

    }
}
