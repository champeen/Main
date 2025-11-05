using EHS.Models.IH;
using HtmlAgilityPack;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EHS.Services.Chemicals
{
    public class OshaNioshClient
    {
        private readonly HttpClient _http;
        public OshaNioshClient(HttpClient http) { _http = http; }

        // -----------------------------------------------------------------
        // PPE key constants (keep labels consistent across all code paths)
        // -----------------------------------------------------------------
        private const string PERSONAL_AGG_KEY = "PPE (NPG): Personal protection & sanitation";
        private const string RESP_ABOVE_KEY = "PPE (NPG): Respirator — above REL/detectable";
        private const string RESP_ESCAPE_KEY = "PPE (NPG): Respirator — escape";

        // ------------------------------------------------------------
        // Shared helpers
        // ------------------------------------------------------------
        private async Task<string?> GetHtmlOrNullAsync(string url, CancellationToken ct)
        {
            try
            {
                // If OSHA URL, warm up session to get cookies first
                bool isOsha = url.Contains("osha.gov", StringComparison.OrdinalIgnoreCase);
                if (isOsha)
                    await EnsureOshaSessionAsync(ct);

                using var req = new HttpRequestMessage(HttpMethod.Get, url);

                // Browser-like headers; OSHA cares about these
                req.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                req.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                req.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
                req.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
                req.Headers.TryAddWithoutValidation("Pragma", "no-cache");
                req.Headers.TryAddWithoutValidation("Connection", "keep-alive");

                if (isOsha)
                {
                    req.Headers.TryAddWithoutValidation("Referer", "https://www.osha.gov/chemicaldata");
                    req.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                    req.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
                    req.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                    req.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                    req.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                }

                var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                Debug.WriteLine($"[HTTP] {(int)resp.StatusCode} {resp.StatusCode} GET {url}");

                // Retry once on 403 with tweaked headers
                if ((int)resp.StatusCode == 403)
                {
                    await Task.Delay(350, ct);

                    using var retry = new HttpRequestMessage(HttpMethod.Get, url);
                    retry.Headers.TryAddWithoutValidation("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                    retry.Headers.TryAddWithoutValidation("Accept", "text/html");
                    retry.Headers.TryAddWithoutValidation("Accept-Language", "en-US;q=0.8,en;q=0.6");
                    if (isOsha)
                    {
                        retry.Headers.TryAddWithoutValidation("Referer", "https://www.osha.gov/");
                        retry.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                        retry.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
                        retry.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                        retry.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                        retry.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                    }

                    var r2 = await _http.SendAsync(retry, HttpCompletionOption.ResponseHeadersRead, ct);
                    Debug.WriteLine($"[HTTP][RETRY] {(int)r2.StatusCode} {r2.StatusCode} GET {url}");
                    if (!r2.IsSuccessStatusCode) return null;
                    return await r2.Content.ReadAsStringAsync(ct);
                }

                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadAsStringAsync(ct);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP][ERR] {ex.GetType().Name}: {ex.Message} @ {url}");
                return null;
            }
        }

        private DateTime _oshaSessionFreshUntil = DateTime.MinValue;
        private readonly SemaphoreSlim _oshaGate = new(1, 1);

        private async Task EnsureOshaSessionAsync(CancellationToken ct)
        {
            if (DateTime.UtcNow < _oshaSessionFreshUntil) return;

            await _oshaGate.WaitAsync(ct);
            try
            {
                if (DateTime.UtcNow < _oshaSessionFreshUntil) return;

                var entry = "https://www.osha.gov/chemicaldata";
                using var req = new HttpRequestMessage(HttpMethod.Get, entry);
                req.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                req.Headers.TryAddWithoutValidation("Accept", "text/html");
                req.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
                req.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                req.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "none");
                req.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                req.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                req.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");

                var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                Debug.WriteLine($"[OSHA-WARMUP] {(int)resp.StatusCode} {resp.StatusCode}");
                // If we got any 2xx/3xx, cookies should now be set
                if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode < 400)
                    _oshaSessionFreshUntil = DateTime.UtcNow.AddMinutes(10);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OSHA-WARMUP][ERR] {ex.Message}");
                // don't throw; we’ll try real request anyway
            }
            finally
            {
                _oshaGate.Release();
            }
        }

        private static string? ResolveHref(string? hrefRaw, string pageUrl)
        {
            if (string.IsNullOrWhiteSpace(hrefRaw)) return null;
            hrefRaw = hrefRaw.Trim();

            try
            {
                if (Uri.TryCreate(hrefRaw, UriKind.Absolute, out var abs)) return abs.ToString();

                var baseUri = new Uri(pageUrl);
                if (hrefRaw.StartsWith("//")) return $"{baseUri.Scheme}:{hrefRaw}";
                if (Uri.TryCreate(baseUri, hrefRaw, out var rel)) return rel.ToString();

                if (!hrefRaw.StartsWith("/")) hrefRaw = "/" + hrefRaw;
                if (Uri.TryCreate(new Uri($"{baseUri.Scheme}://{baseUri.Host}"), hrefRaw, out var hostRel))
                    return hostRel.ToString();
            }
            catch { }
            return null;
        }

        private static string NormalizeCas(string s) => Regex.Replace(s ?? "", @"\s", "");
        private static bool StringEqualsCas(string a, string b) => NormalizeCas(a).Equals(NormalizeCas(b), StringComparison.OrdinalIgnoreCase);

        // ------------------------------------------------------------
        // NPG URL resolvers  (REPLACE THIS WHOLE SECTION)
        // ------------------------------------------------------------
        private static string MakeAbsoluteNpgUrl(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return href;
            if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return href;
            if (href.StartsWith("//")) return "https:" + href;
            if (!href.StartsWith("/")) href = "/" + href;
            return "https://www.cdc.gov" + href;
        }


        private static void FinalizePpe(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0) return;

            // 1) Canonicalize keys (but do not drop/rename respirators incorrectly)
            var remap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in dict.Keys.ToList())
            {
                var canon = CanonPpeKey(k); // keep your existing CanonPpeKey
                if (!canon.Equals(k, StringComparison.OrdinalIgnoreCase))
                {
                    var v = dict[k] ?? "";
                    dict.Remove(k);
                    if (!remap.ContainsKey(canon)) remap[canon] = v;
                    else remap[canon] += Environment.NewLine + v;
                }
            }
            foreach (var kv in remap) dict[kv.Key] = kv.Value;

            // 2) Bulletize values that aren’t already bullet lists
            static string Bulletize(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return s ?? "";
                if (s.Contains("\n• ")) return s; // already bulletized
                var lines = s.Replace("\r", "")
                             .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                             .Select(t => t.Trim())
                             .Where(t => t.Length > 0)
                             .ToArray();
                if (lines.Length <= 1)
                {
                    // If single long paragraph, try to break at sentences/semicolons to help the view
                    lines = Regex.Split(s, @"(?<=[.;])\s+")
                                 .Select(t => Regex.Replace(t, @"\s+", " ").Trim())
                                 .Where(t => t.Length > 0)
                                 .ToArray();
                }
                return string.Join(Environment.NewLine, lines.Select(l => l.StartsWith("•") ? l : "• " + l));
            }

            // 3) Never touch respirator keys beyond bulletizing
            foreach (var key in dict.Keys
                                    .Where(k => k.StartsWith("PPE (NPG): Respirator", StringComparison.OrdinalIgnoreCase))
                                    .ToList())
            {
                dict[key] = Bulletize(dict[key] ?? "");
            }

            // 4) Personal protection & sanitation:
            //    If you have per-key rows, you can choose to collapse them into one combined row
            //    — but DO NOT remove any respirator keys. Keep it conservative.
            var personalKeys = dict.Keys
                .Where(k => k.Equals("PPE (NPG): Personal protection & sanitation", StringComparison.OrdinalIgnoreCase)
                         || k.Equals("Personal protection & sanitation", StringComparison.OrdinalIgnoreCase)
                         || k.StartsWith("PPE (NPG): Personal protection & sanitation — ", StringComparison.OrdinalIgnoreCase)
                         || k.StartsWith("Personal protection & sanitation — ", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (personalKeys.Count > 0)
            {
                // If a combined row already exists, leave it and just bulletize it.
                var combinedKey = personalKeys
                    .FirstOrDefault(k => k.Equals("PPE (NPG): Personal protection & sanitation", StringComparison.OrdinalIgnoreCase))
                    ?? personalKeys.FirstOrDefault(k => k.Equals("Personal protection & sanitation", StringComparison.OrdinalIgnoreCase));

                if (combinedKey != null)
                {
                    dict[combinedKey] = Bulletize(dict[combinedKey] ?? "");
                    // Keep per-key rows as well (to avoid losing detail). If you *really* want to hide them,
                    // comment the loop below in.
                    foreach (var k in personalKeys.Where(k => !k.Equals(combinedKey, StringComparison.OrdinalIgnoreCase)))
                        dict[k] = Bulletize(dict[k] ?? "");
                }
                else
                {
                    // No combined row — create one by merging the per-key rows
                    var bullets = new List<string>();
                    foreach (var k in personalKeys)
                    {
                        var suffix = k;
                        var ix = Math.Max(suffix.LastIndexOf('—'), suffix.LastIndexOf('-'));
                        if (ix >= 0 && ix + 1 < suffix.Length) suffix = suffix[(ix + 1)..].Trim();
                        var val = dict[k] ?? "";
                        var lines = val.Replace("\r", "")
                                       .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(t => t.Trim().TrimStart('•').Trim())
                                       .Where(t => t.Length > 0);
                        foreach (var l in lines)
                            bullets.Add($"• {suffix}: {l}");
                    }
                    var merged = string.Join(Environment.NewLine, bullets.Distinct(StringComparer.OrdinalIgnoreCase));
                    dict["PPE (NPG): Personal protection & sanitation"] = merged;
                }
            }

            // 5) That’s it — DO NOT prune anything else. We want maximum completeness.
        }


        public async Task<string?> FindNpgUrlByCasAsync(string cas, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cas)) return null;

            static string NormalizeHyphens(string s) =>
                Regex.Replace(s ?? "", "[\u2010-\u2015\u2212\uFE63\uFF0D]", "-");
            static string DigitsOnly(string s) =>
                new string((s ?? string.Empty).Where(char.IsDigit).ToArray());
            static string ToAbsNpg(string href)
            {
                if (string.IsNullOrWhiteSpace(href)) return href;
                if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return href;
                if (href.StartsWith("/")) return "https://www.cdc.gov" + href;
                return "https://www.cdc.gov" + (href.StartsWith("niosh", StringComparison.OrdinalIgnoreCase) ? "/" + href : "/niosh/npg/" + href.TrimStart('/'));
            }

            var casNorm = NormalizeHyphens(cas.Trim());
            var casDigits = DigitsOnly(casNorm);

            var indexUrls = new[]
            {
                "https://www.cdc.gov/niosh/npg/npgdcas.html",
                "https://www.cdc.gov/niosh/docs/2005-149/npgdcas.html"
            };

            foreach (var indexUrl in indexUrls)
            {
                var html = await GetHtmlOrNullAsync(indexUrl, ct);
                if (string.IsNullOrWhiteSpace(html)) continue;

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // 1) Row scan
                foreach (var tr in doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlNodeCollection(null))
                {
                    var rowText = HtmlEntity.DeEntitize(tr.InnerText ?? string.Empty);
                    rowText = NormalizeHyphens(rowText);
                    if (!DigitsOnly(rowText).Contains(casDigits)) continue;

                    var a = tr.SelectSingleNode(".//a[contains(@href,'/niosh/npg/npgd') or contains(@href,'npgd')][contains(@href,'.html')]");
                    if (a != null)
                    {
                        var abs = ResolveHref(a.GetAttributeValue("href", string.Empty), indexUrl) ?? ToAbsNpg(a.GetAttributeValue("href", string.Empty));
                        if (!string.IsNullOrWhiteSpace(abs)) return abs;
                    }
                }

                // 2) Fallback: scope match
                foreach (var a in doc.DocumentNode.SelectNodes("//a[contains(@href,'npgd') and contains(@href,'.html')]")
                         ?? new HtmlNodeCollection(null))
                {
                    var container = a.Ancestors().FirstOrDefault(n =>
                        n.Name.Equals("tr", StringComparison.OrdinalIgnoreCase) ||
                        n.Name.Equals("p", StringComparison.OrdinalIgnoreCase) ||
                        n.Name.Equals("li", StringComparison.OrdinalIgnoreCase));
                    var scopeText = HtmlEntity.DeEntitize((container ?? a.ParentNode)?.InnerText ?? string.Empty);
                    scopeText = NormalizeHyphens(scopeText);
                    if (DigitsOnly(scopeText).Contains(casDigits))
                    {
                        var abs = ResolveHref(a.GetAttributeValue("href", string.Empty), indexUrl) ?? ToAbsNpg(a.GetAttributeValue("href", string.Empty));
                        if (!string.IsNullOrWhiteSpace(abs)) return abs;
                    }
                }
            }

            // 3) CDC site search fallbacks
            foreach (var q in new[]
            {
                $"\"{casNorm}\" site:cdc.gov/niosh/npg/npgd",
                $"{casNorm} site:cdc.gov/niosh/npg",
                $"{casDigits} site:cdc.gov/niosh/npg"
            })
            {
                var searchUrl = $"https://search.cdc.gov/search/?query={Uri.EscapeDataString(q)}";
                var html = await GetHtmlOrNullAsync(searchUrl, ct);
                if (string.IsNullOrWhiteSpace(html)) continue;

                var doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(html);
                var hit = doc.DocumentNode.SelectSingleNode("//a[contains(@href,'/niosh/npg/npgd') and contains(@href,'.html')]");
                var href = hit?.GetAttributeValue("href", null);
                if (!string.IsNullOrWhiteSpace(href))
                {
                    var abs = href.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? href : "https://www.cdc.gov" + href;
                    return abs;
                }
            }

            return null;
        }

        public async Task<string?> FindNpgUrlByNameAsync(string chemicalName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(chemicalName)) return null;

            static string Norm(string s) => Regex.Replace((s ?? "").ToLowerInvariant(), @"\s+", " ").Trim();
            static string CleanCandidate(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = Regex.Replace(s, @"\([^)]*\)", " ");
                s = Regex.Replace(s, @"\b(anhydrous|monohydrate|dihydrate|trihydrate|hydrate|pentahydrate|usp|nf|bp|fcc|tech(?:nical)?|solution|reagent|grade)\b", " ", RegexOptions.IgnoreCase);
                s = Regex.Replace(s, @"[^A-Za-z \-]", " ");
                s = Regex.Replace(s, @"\s+", " ").Trim();
                if (Regex.IsMatch(s, @"\bformalin\b", RegexOptions.IgnoreCase)) s = "formaldehyde";
                return s;
            }
            static IEnumerable<string> ExpandCandidateForms(string cleaned)
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
            static string ToAbsNpg(string href)
            {
                if (string.IsNullOrWhiteSpace(href)) return href;
                if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return href;
                if (href.StartsWith("/")) return "https://www.cdc.gov" + href;
                return "https://www.cdc.gov" + (href.StartsWith("niosh", StringComparison.OrdinalIgnoreCase) ? "/" + href : "/niosh/npg/" + href.TrimStart('/'));
            }

            const string indexUrl = "https://www.cdc.gov/niosh/npg/npgdalpha.html";
            var html = await GetHtmlOrNullAsync(indexUrl, ct);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var anchors = doc.DocumentNode.SelectNodes("//a[contains(@href,'npgd') and contains(@href,'.html')]")
                             ?? new HtmlNodeCollection(null);

                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var a in anchors)
                {
                    var disp = HtmlEntity.DeEntitize(a.InnerText ?? "");
                    var href = a.GetAttributeValue("href", string.Empty);
                    if (string.IsNullOrWhiteSpace(disp) || string.IsNullOrWhiteSpace(href)) continue;
                    var key = Norm(disp);
                    if (!map.ContainsKey(key)) map[key] = ToAbsNpg(href);
                }

                var cleaned = CleanCandidate(chemicalName);
                var candidates = ExpandCandidateForms(cleaned).Select(Norm).Distinct().ToList();

                foreach (var k in candidates)
                    if (map.TryGetValue(k, out var url1)) return url1;

                foreach (var k in candidates)
                {
                    var hit = map.Keys.FirstOrDefault(entry => k.Contains(entry, StringComparison.OrdinalIgnoreCase))
                           ?? map.Keys.FirstOrDefault(entry => entry.Contains(k, StringComparison.OrdinalIgnoreCase));
                    if (hit != null) return map[hit];
                }
            }

            // CDC search fallback
            foreach (var q in new[]
            {
                $"\"{chemicalName}\" site:cdc.gov/niosh/npg/npgd",
                $"{chemicalName} site:cdc.gov/niosh/npg"
            })
            {
                var searchUrl = $"https://search.cdc.gov/search/?query={Uri.EscapeDataString(q)}";
                var shtml = await GetHtmlOrNullAsync(searchUrl, ct);
                if (string.IsNullOrWhiteSpace(shtml)) continue;

                var sdoc = new HtmlAgilityPack.HtmlDocument(); sdoc.LoadHtml(shtml);
                var hit = sdoc.DocumentNode.SelectSingleNode("//a[contains(@href,'/niosh/npg/npgd') and contains(@href,'.html')]");
                var href = hit?.GetAttributeValue("href", null);
                if (!string.IsNullOrWhiteSpace(href))
                    return href.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? href : "https://www.cdc.gov" + href;
            }

            return null;
        }


        // ------------------------------------------------------------
        // OELs (NIOSH NPG): CAS first, then name fallback
        // ------------------------------------------------------------
        public async Task<List<IhChemicalOel>> GetOelsFromNpgByCasOrNamesAsync(
            string cas, IEnumerable<string> nameCandidates, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var oels = await GetOelsFromNpgByCasAsync(cas, unavailable, ct);
            if (oels.Count > 0) return oels;

            foreach (var n in nameCandidates ?? Array.Empty<string>())
            {
                var byName = await GetOelsFromNpgByNameAsync(n, unavailable, ct);
                if (byName.Count > 0) return byName;
            }
            return oels;
        }

        public async Task<List<IhChemicalOel>> GetOelsFromNpgByCasAsync(
            string cas, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var results = new List<IhChemicalOel>();
            var url = await FindNpgUrlByCasAsync(cas, ct);
            if (string.IsNullOrWhiteSpace(url))
            {
                unavailable?.Add("NIOSH NPG: no page found for this CAS");
                return results;
            }

            var html = await GetHtmlOrNullAsync(url!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add("NIOSH NPG: page fetch failed/blocked");
                return results;
            }

            ExtractNpgOelsFromPageHtml(html, results);
            return results
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        public async Task<List<IhChemicalOel>> GetOelsFromNpgByNameAsync(
            string name, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var results = new List<IhChemicalOel>();
            var url = await FindNpgUrlByNameAsync(name, ct);
            if (string.IsNullOrWhiteSpace(url))
            {
                unavailable?.Add($"NIOSH NPG: no page found by name \"{name}\"");
                return results;
            }

            var html = await GetHtmlOrNullAsync(url!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add($"NIOSH NPG: page fetch failed/blocked for \"{name}\"");
                return results;
            }

            ExtractNpgOelsFromPageHtml(html, results);
            return results
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        // Robust OEL extractor
        private static void ExtractNpgOelsFromPageHtml(string html, List<IhChemicalOel> results)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? string.Empty);
            text = Regex.Replace(text, @"\s+", " ").Trim();

            static string SliceBetween(string haystack, string startPattern, params string[] stopPatterns)
            {
                var m = Regex.Match(haystack, startPattern, RegexOptions.IgnoreCase);
                if (!m.Success) return string.Empty;

                int start = m.Index + m.Length;
                int end = haystack.Length;

                foreach (var sp in stopPatterns)
                {
                    var mm = Regex.Match(haystack, sp, RegexOptions.IgnoreCase);
                    if (mm.Success && mm.Index > start && mm.Index < end)
                        end = mm.Index;
                }
                return haystack.Substring(start, Math.Max(0, end - start)).Trim();
            }

            var relBlock = SliceBetween(text,
                @"NIOSH\s+REL",
                @"OSHA\s+PEL", @"ACGIH\s+TLV", @"Cal/OSHA", @"California\s+OSHA");
            var pelBlock = SliceBetween(text,
                @"OSHA\s+PEL(?:\s*\[[^\]]*\])?",
                @"NIOSH\s+REL", @"ACGIH\s+TLV", @"Cal/OSHA", @"California\s+OSHA");
            var tlvBlock = SliceBetween(text,
                @"ACGIH\s+TLV",
                @"NIOSH\s+REL", @"OSHA\s+PEL(?:\s*\[[^\]]*\])?", @"Cal/OSHA", @"California\s+OSHA");
            var calBlock = SliceBetween(text,
                @"(?:Cal/OSHA|California\s+OSHA)",
                @"NIOSH\s+REL", @"OSHA\s+PEL(?:\s*\[[^\]]*\])?", @"ACGIH\s+TLV");

            results.AddRange(ParseLimitsFromBlockSimple("NIOSH", relBlock));
            results.AddRange(ParseLimitsFromBlockSimple("OSHA", pelBlock));
            results.AddRange(ParseLimitsFromBlockSimple("ACGIH", tlvBlock));
            results.AddRange(ParseLimitsFromBlockSimple("Cal/OSHA", calBlock));
        }


        /// Tolerant OEL extractor
        public static List<IhChemicalOel> ParseLimitsFromBlockSimple(string source, string? blockText)
        {
            var list = new List<IhChemicalOel>();
            if (string.IsNullOrWhiteSpace(blockText)) return list;

            var txt = Regex.Replace(blockText, @"\s+", " ").Trim();

            // Case A
            var rxA = new Regex(
                @"(?:(?<type>\bTWA\b|\bSTEL\b|\bST\b|\bC(?:eil(?:ing)?)?\b)\s*[:\-]?\s*)?(?<value>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)",
                RegexOptions.IgnoreCase);

            // Case B
            var rxB = new Regex(
                @"(?<value>\d+(?:\.\d+)?)\s*(?<unit>ppm|ppb|mg/?m3|mg/?m\^3|mg/?m³|µg/?m3|μg/?m3|ug/?m3)\s*(?<type>\bTWA\b|\bSTEL\b|\bST\b|\bC(?:eil(?:ing)?)?\b)",
                RegexOptions.IgnoreCase);

            void AddMatch(Match m, bool reverseTypeOrder)
            {
                var t = m.Groups["type"].Success ? m.Groups["type"].Value : "";
                var v = m.Groups["value"].Value;
                var u = m.Groups["unit"].Value;

                var normType =
                    Regex.IsMatch(t, @"^twa$", RegexOptions.IgnoreCase) ? "TWA" :
                    Regex.IsMatch(t, @"^(st|stel)$", RegexOptions.IgnoreCase) ? "STEL" :
                    Regex.IsMatch(t, @"^c", RegexOptions.IgnoreCase) ? "Ceiling" :
                    (string.IsNullOrWhiteSpace(t) ? "LIMIT" : t.Trim());

                var valueStr = $"{v} {u}".Replace("m^3", "m3");

                string? note = null;
                var afterIndex = reverseTypeOrder ? (m.Index) : (m.Index + m.Length);
                var after = txt.Substring(afterIndex);
                var mNote = Regex.Match(after, @"\[(?<note>[^\]]+)\]");
                if (mNote.Success) note = mNote.Groups["note"].Value.Trim();

                list.Add(new IhChemicalOel { Source = source, Type = normType, Value = valueStr, Notes = note });
            }

            foreach (Match m in rxA.Matches(txt)) AddMatch(m, reverseTypeOrder: false);
            foreach (Match m in rxB.Matches(txt)) AddMatch(m, reverseTypeOrder: true);

            return list
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}|{x.Notes}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        // ------------------------------------------------------------
        // PPE (NIOSH NPG): CAS first, then name fallback
        // ------------------------------------------------------------
        public async Task<Dictionary<string, string>> GetPpeFromNpgByCasOrNamesAsync(
    string cas, IEnumerable<string> nameCandidates,
    List<string>? unavailable = null, CancellationToken ct = default)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1) Try CAS first
            string? url = null;
            try { url = await FindNpgUrlByCasAsync(cas, ct); } catch { }
            if (string.IsNullOrWhiteSpace(url))
            {
                // 2) Name fallback
                foreach (var n in nameCandidates ?? Array.Empty<string>())
                {
                    try
                    {
                        url = await FindNpgUrlByNameAsync(n, ct);
                        if (!string.IsNullOrWhiteSpace(url)) break;
                    }
                    catch { /* continue */ }
                }
            }
            if (string.IsNullOrWhiteSpace(url))
            {
                unavailable?.Add("NIOSH NPG: PPE page not found by CAS or name");
                return dict;
            }

            // Fetch HTML
            var html = await GetHtmlOrNullAsync(url!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add("NIOSH NPG: PPE page fetch failed/blocked");
                return dict;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // -------------------------
            // PERSONAL PROTECTION BLOCK
            // -------------------------
            var personalFromSection = ExtractSectionLines(
                doc,
                new[] {
            "personal protection & sanitation",
            "personal protection and sanitation",
            "personal protective equipment",
            "personal protection",
            "protection & sanitation",
            "protection / sanitation",
            "exposure controls/personal protection",
            "exposure controls and personal protection",
            "exposure controls – personal protection",
            "exposure controls - personal protection",
            "exposure control and personal protection"
                },
                new[] { "personal", "protection", "sanitation", "ppe" }
            );

            var personalKV = ExtractPersonalKeyValueFallback(doc);
            var personalBroad = ExtractBroadPersonalCandidates(doc);

            var personalLines = NormalizePersonalKeyValues(
                MergeDistinct(personalFromSection, personalKV, personalBroad)
            );

            EnsurePersonalRows(dict, personalLines);

            // -------------------------
            // RESPIRATOR BLOCKS
            // -------------------------
            // We will collect candidates from three routes and then split into Above vs Escape robustly.
            List<string> respCandidates = new();

            // (A) DOM heading slice (preferred)
            {
                var heads = doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//*[@role='heading']") ?? new HtmlNodeCollection(null);
                HtmlNode? hit = null;
                foreach (var h in heads)
                {
                    var t = HtmlEntity.DeEntitize(h.InnerText ?? "");
                    t = Regex.Replace(t, @"\s+", " ").Trim().ToLowerInvariant();
                    if (t.Contains("respirator recommendations") || t.Contains("respiratory protection") || t.Equals("respirator"))
                    {
                        hit = h; break;
                    }
                }
                if (hit != null)
                {
                    var sb = new System.Text.StringBuilder();
                    for (var n = hit.NextSibling; n != null; n = n.NextSibling)
                    {
                        if (n.NodeType == HtmlNodeType.Element)
                        {
                            var nm = n.Name.ToLowerInvariant();
                            var isHeading = nm is "h1" or "h2" or "h3" or "h4" ||
                                            n.GetAttributeValue("role", "") == "heading";
                            if (isHeading) break;
                        }
                        if (n.NodeType == HtmlNodeType.Element || n.NodeType == HtmlNodeType.Text)
                            sb.Append(' ').Append(n.InnerText);
                    }
                    var section = HtmlEntity.DeEntitize(sb.ToString());
                    section = Regex.Replace(section, @"\s+", " ").Trim();
                    if (!string.IsNullOrWhiteSpace(section)) respCandidates.Add(section);
                }
            }

            // (B) Text slice fallback using “Respirator”/“Respiratory protection” anchors
            {
                string fullText = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? string.Empty);
                fullText = Regex.Replace(fullText, @"\s+", " ").Trim();
                string Slice(string text, string startPattern, params string[] untilPatterns)
                {
                    var m = Regex.Match(text, startPattern, RegexOptions.IgnoreCase);
                    if (!m.Success) return string.Empty;
                    int start = m.Index + m.Length, end = text.Length;
                    foreach (var up in untilPatterns)
                    {
                        var u = Regex.Match(text, up, RegexOptions.IgnoreCase);
                        if (u.Success && u.Index > start && u.Index < end) end = u.Index;
                    }
                    return text.Substring(start, Math.Max(0, end - start)).Trim();
                }

                var respSlice = Slice(fullText,
                    @"(Respirator\s+recommendations|Respiratory\s+protection|Respirator)\s*:?",
                    "Exposure\\s+controls", "First\\s+Aid", "Emergency", "Spills", "Appendix", "Page last reviewed", "NIOSH\\s+REL", "OSHA\\s+PEL", "ACGIH\\s+TLV");

                if (!string.IsNullOrWhiteSpace(respSlice)) respCandidates.Add(respSlice);
            }

            // (C) Harvest respirator-like lines anywhere in DOM as last resort
            {
                var domLines = HarvestRespiratorCandidatesFromDom(doc)?.ToArray() ?? Array.Empty<string>();
                if (domLines.Length > 0) respCandidates.Add(string.Join(" || ", domLines));
            }

            // Merge candidates and split into Above vs Escape
            string joined = string.Join(" || ", respCandidates);
            joined = HtmlEntity.DeEntitize(joined ?? string.Empty);
            joined = Regex.Replace(joined, @"\s+", " ").Trim();

            static string[] SplitRespLines(string text)
            {
                if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();
                var bits = text
                    .Replace("•", "\n")
                    .Replace("·", "\n")
                    .Replace("; ", "\n")
                    .Replace(". ", "\n")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                    .Where(s => s.Length > 2)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                bool Keep(string s) =>
                    Regex.IsMatch(s, @"\b(APF|SCBA|supplied[-\s]?air|SAR|air[-\s]?line|positive[-\s]?pressure|gas mask|full[-\s]?facepiece|cartridge|canister)\b", RegexOptions.IgnoreCase) ||
                    s.StartsWith("Use ", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("Any ", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("Escape", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("(APF", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("At concentrations above", StringComparison.OrdinalIgnoreCase);

                return bits.Where(Keep).ToArray();
            }

            var allResp = SplitRespLines(joined);

            // Identify the “Escape:” part and the “Above REL/detectable” part with robust rules.
            var escapeLines = new List<string>();
            var aboveLines = new List<string>();

            // 1) If explicit "Escape:" label present, split by that
            {
                var m = Regex.Match(joined, @"\bEscape\s*:\s*(?<esc>.+)$", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var before = joined.Substring(0, m.Index);
                    var escTxt = m.Groups["esc"].Value;
                    aboveLines.AddRange(SplitRespLines(before));
                    escapeLines.AddRange(SplitRespLines(escTxt));
                }
            }

            // 2) If no explicit label, infer by “At concentrations above … REL … detectable …”
            if (aboveLines.Count == 0)
            {
                var aboveInferred = allResp.Where(s =>
                    s.StartsWith("At concentrations above", StringComparison.OrdinalIgnoreCase) ||
                    Regex.IsMatch(s, @"\babove\s+the\s+NIOSH\s+REL\b", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(s, @"\bno\s+REL\b", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(s, @"\bdetectable\b", RegexOptions.IgnoreCase)
                ).ToList();

                // Include the APF/instructions that follow the “At concentrations…” anchor
                if (aboveInferred.Count > 0)
                {
                    foreach (var s in allResp)
                    {
                        if (aboveInferred.Contains(s)) { aboveLines.Add(s); continue; }
                        if (Regex.IsMatch(s, @"^\(APF\s*=", RegexOptions.IgnoreCase) ||
                            s.StartsWith("Any ", StringComparison.OrdinalIgnoreCase) ||
                            s.StartsWith("Use ", StringComparison.OrdinalIgnoreCase) ||
                            Regex.IsMatch(s, @"\b(SCBA|supplied[-\s]?air|SAR|gas mask|canister|cartridge|full[-\s]?facepiece)\b", RegexOptions.IgnoreCase))
                        {
                            aboveLines.Add(s);
                        }
                    }
                }
            }

            // 3) If we still have no aboveLines but have APF lines and no “Escape:” marker, treat the first APF block as Above.
            if (aboveLines.Count == 0 && escapeLines.Count == 0)
            {
                var apfBlocks = allResp.Where(s => Regex.IsMatch(s, @"^\(APF\s*=", RegexOptions.IgnoreCase)).ToList();
                if (apfBlocks.Count > 0)
                {
                    // collect from first APF forward until we hit an explicit Escape line (if any)
                    bool inAbove = false;
                    foreach (var s in allResp)
                    {
                        if (!inAbove && apfBlocks.Contains(s)) inAbove = true;
                        if (!inAbove) continue;
                        if (Regex.IsMatch(s, @"^Escape", RegexOptions.IgnoreCase)) break;
                        aboveLines.Add(s);
                    }
                }
            }

            // 4) Escape inference if not explicitly labeled: any line starting with “Escape …”
            if (escapeLines.Count == 0)
            {
                escapeLines.AddRange(allResp.Where(s => s.StartsWith("Escape", StringComparison.OrdinalIgnoreCase)));
            }

            // De-dup and clean
            string BulletJoin(IEnumerable<string> items)
                => string.Join(Environment.NewLine, (items ?? Array.Empty<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.StartsWith("•") ? s : "• " + s)
                    .Distinct(StringComparer.OrdinalIgnoreCase));

            var aboveOut = aboveLines
                .Select(s => Regex.Replace(s ?? "", @"\s+", " ").Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var escapeOut = escapeLines
                .Select(s => Regex.Replace(s ?? "", @"\s+", " ").Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (aboveOut.Count > 0)
                dict["PPE (NPG): Respirator — above REL / detectable"] = BulletJoin(aboveOut);
            if (escapeOut.Count > 0)
                dict["PPE (NPG): Respirator — escape"] = BulletJoin(escapeOut);

            // -------------------------
            // FINALIZE (normalize keys, bulletize respirators, collapse personal)
            // -------------------------
            FinalizePpe(dict);

            PostProcessPpeForUi(dict);

            // EXTRA GUARD: FinalizePpe should never remove “Above REL / detectable”.
            // If for any reason it disappeared, restore it from our local copies.
            if (!dict.Keys.Any(k => k.StartsWith("PPE (NPG): Respirator — above", StringComparison.OrdinalIgnoreCase)) && aboveOut.Count > 0)
                dict["PPE (NPG): Respirator — above REL / detectable"] = BulletJoin(aboveOut);

            return dict;
        }


        public async Task<Dictionary<string, string>> GetPpeFromNpgByNameAsync(string name, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var url = await FindNpgUrlByNameAsync(name, ct);
            if (string.IsNullOrWhiteSpace(url))
            {
                unavailable?.Add($"NIOSH NPG: PPE page not found by name \"{name}\"");
                return dict;
            }

            var html = await GetHtmlOrNullAsync(url!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add($"NIOSH NPG: PPE page fetch failed/blocked for \"{name}\"");
                return dict;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // --- Personal protection (prefer dedicated section; enrich with KV/broad if thin) ---
            var personalFromSection = ExtractSectionLines(
                doc,
                new[] {
            "personal protection & sanitation",
            "personal protection and sanitation",
            "personal protective equipment",
            "personal protection",
            "protection & sanitation",
            "protection / sanitation",
            // common variants
            "exposure controls/personal protection",
            "exposure controls and personal protection",
            "exposure controls – personal protection",
            "exposure controls - personal protection",
            "exposure control and personal protection"
                },
                new[] { "personal", "protection", "sanitation", "ppe" }
            );

            var personalKV = ExtractPersonalKeyValueFallback(doc);
            var personalBroad = Array.Empty<string>();

            static IEnumerable<string> DropNoise(IEnumerable<string> seq) =>
                (seq ?? Array.Empty<string>()).Where(s =>
                    !Regex.IsMatch(s ?? "", @"^\(?\s*see\s+protection\s+codes\)?\s*$", RegexOptions.IgnoreCase));

            var personalLines = NormalizePersonalKeyValues(
                MergeDistinct(DropNoise(personalFromSection), DropNoise(personalKV))
            );

            if (personalLines.Length < 3)
            {
                personalBroad = ExtractBroadPersonalCandidates(doc);
                personalLines = NormalizePersonalKeyValues(
                    MergeDistinct(DropNoise(personalLines), DropNoise(personalBroad))
                );
            }

            EnsurePersonalRows(dict, personalLines);

            // --- Respirator section: DOM-first extractor
            var (above, escape) = ExtractRespiratorRows(doc);
            if (above?.Length > 0)
                dict[RESP_ABOVE_KEY] = BulletJoin(above);
            if (escape?.Length > 0)
                dict[RESP_ESCAPE_KEY] = BulletJoin(escape);

            // --- Merge tolerant extractor (fills any missing pieces)
            var tolerantAll = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ExtractNpgPpeFromPageHtml(html, tolerantAll);

            foreach (var kv in tolerantAll)
            {
                if (!dict.ContainsKey(kv.Key) || string.IsNullOrWhiteSpace(dict[kv.Key]))
                {
                    dict[kv.Key] = kv.Value;
                    continue;
                }

                var have = (dict[kv.Key] ?? "")
                    .Replace("\r", "")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var add = (kv.Value ?? "")
                    .Replace("\r", "")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim());

                foreach (var line in add)
                    if (line.Length > 0 && !have.Contains(line)) have.Add(line);

                dict[kv.Key] = string.Join(Environment.NewLine, have.Select(s => s.StartsWith("•") ? s : "• " + s));
            }

            FinalizePpe(dict);

            foreach (var k in dict.Keys.OrderBy(k => k)) Debug.WriteLine($"  {k} len={(dict[k]?.Length ?? 0)}");

            PostProcessPpeForUi(dict);

            return dict;
        }



        private static void ExtractNpgPpeFromPageHtml(string html, Dictionary<string, string> dict)
        {
            // Defensive: never throw on a bad page
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                static string W(string s) =>
                    Regex.Replace(HtmlEntity.DeEntitize(s ?? ""), @"\s+", " ").Trim();

                // ---------- Grab full-page plain text once ----------
                var fullText = W(doc.DocumentNode.InnerText ?? "");

                // ---------- Find a section by headings (DOM) or text slicing (tolerant) ----------
                static string GrabSectionByHeads(HtmlDocument d, params string[] heads)
                {
                    string Norm(string s) => Regex.Replace((s ?? "").ToLowerInvariant(), @"\s+", " ").Trim();

                    var wanted = heads.Select(Norm).ToArray();
                    var nodes = d.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//*[@role='heading']") ?? new HtmlNodeCollection(null);

                    HtmlNode hit = null;
                    foreach (var h in nodes)
                    {
                        var t = Norm(h.InnerText);
                        if (wanted.Any(w => t.Contains(w))) { hit = h; break; }
                    }
                    if (hit == null) return string.Empty;

                    var sb = new System.Text.StringBuilder();
                    for (var n = hit.NextSibling; n != null; n = n.NextSibling)
                    {
                        if (n.NodeType == HtmlNodeType.Element)
                        {
                            var nm = n.Name.ToLowerInvariant();
                            var isHeading = nm is "h1" or "h2" or "h3" or "h4" ||
                                            n.GetAttributeValue("role", "").Equals("heading", StringComparison.OrdinalIgnoreCase);
                            if (isHeading) break;
                        }
                        if (n.NodeType == HtmlNodeType.Element || n.NodeType == HtmlNodeType.Text)
                            sb.Append(' ').Append(n.InnerText);
                    }
                    return W(sb.ToString());
                }

                static string SliceBetween(string text, string startPattern, params string[] untilPatterns)
                {
                    var m = Regex.Match(text, startPattern, RegexOptions.IgnoreCase);
                    if (!m.Success) return string.Empty;
                    int start = m.Index + m.Length, end = text.Length;
                    foreach (var up in untilPatterns)
                    {
                        var u = Regex.Match(text, up, RegexOptions.IgnoreCase);
                        if (u.Success && u.Index > start && u.Index < end) end = u.Index;
                    }
                    return text.Substring(start, Math.Max(0, end - start)).Trim();
                }

                // ---------- PERSONAL ----------
                // Try “Personal protection & sanitation” section via DOM heading first, then tolerant text slice
                var personalDom = GrabSectionByHeads(
                    doc,
                    "personal protection & sanitation", "personal protection and sanitation",
                    "personal protective equipment", "personal protection",
                    "exposure controls/personal protection", "exposure controls and personal protection",
                    "exposure controls – personal protection", "exposure controls - personal protection",
                    "exposure control and personal protection"
                );

                if (string.IsNullOrWhiteSpace(personalDom))
                {
                    // tolerant slice – stop when respirator or next big section begins
                    personalDom = SliceBetween(
                        fullText,
                        @"(?:Personal\s+protection\s*(?:&|and|/)?\s*sanitation|Exposure\s*controls\s*(?:/|and|\-|–)\s*Personal\s*protection)\s*:?",
                        "Respirator\\s+recommendations", "Respiratory\\s+protection",
                        "First\\s+Aid", "Emergency", "Spills", "Handling and storage",
                        "Appendix", "NIOSH\\s+REL", "OSHA\\s+PEL", "ACGIH\\s+TLV", "Page\\s+last\\s+reviewed"
                    );
                }

                // Clean personal lines but keep wide to avoid losing data
                static string[] CleanPersonal(string block)
                {
                    if (string.IsNullOrWhiteSpace(block)) return Array.Empty<string>();

                    // Break generously
                    var raw = block
                        .Replace("•", "\n").Replace("·", "\n")
                        .Replace(" - ", "\n- ").Replace("; ", "\n").Replace(". ", "\n")
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                        .Where(s => s.Length > 2)
                        .ToList();

                    // Light keep rules: we keep explicit KV and obvious PPE phrases
                    bool LooksKV(string s) => Regex.IsMatch(s, @"^[A-Za-z][A-Za-z \-/]{2,40}\s*:\s*.+$");
                    bool HasPpeWords(string s) =>
                        Regex.IsMatch(s, @"\b(glove|goggle|eye|face[-\s]?shield|apron|impervious|wash skin|remove clothing|change clothing|protective clothing|eye protection|contact lenses|avoid|do not|provide)\b", RegexOptions.IgnoreCase);

                    var kept = raw.Where(s => LooksKV(s) || HasPpeWords(s))
                                  .Distinct(StringComparer.OrdinalIgnoreCase)
                                  .ToArray();

                    return kept;
                }

                var personalLines = CleanPersonal(personalDom);

                // Emit as one combined section; view will render bullets on '\n'
                if (personalLines.Length > 0)
                {
                    var bullets = string.Join(Environment.NewLine, personalLines.Select(l => l.StartsWith("•") ? l : "• " + l));
                    dict["PPE (NPG): Personal protection & sanitation"] = bullets;
                }

                // ---------- RESPIRATORS ----------
                // Find respirator section (DOM heading first)
                var respiratorDom = GrabSectionByHeads(doc,
                    "respirator recommendations", "respiratory protection", "respirator");

                if (string.IsNullOrWhiteSpace(respiratorDom))
                {
                    respiratorDom = SliceBetween(
                        fullText,
                        @"(Respirator\s+recommendations|Respiratory\s+protection|Respirator)\s*:?",
                        "Exposure\\s+controls", "First\\s+Aid", "Emergency", "Spills",
                        "Appendix", "NIOSH\\s+REL", "OSHA\\s+PEL", "ACGIH\\s+TLV", "Page\\s+last\\s+reviewed"
                    );
                }

                // Split into Above vs Escape reliably
                string aboveBlock = "";
                string escapeBlock = "";

                if (!string.IsNullOrWhiteSpace(respiratorDom))
                {
                    // Keep the APF lines and list items. We will split at 'Escape:' if present.
                    var withTrim = Regex.Replace(respiratorDom, @"(Important additional information.*|See also INTRODUCTION.*|ICSC CARD:.*|MEDICAL TESTS:.*|Page last reviewed:.*|Content source:.*)$", "", RegexOptions.IgnoreCase).Trim();

                    var mEsc = Regex.Match(withTrim, @"\bEscape\s*:\s*(?<esc>.+)$", RegexOptions.IgnoreCase);
                    if (mEsc.Success)
                    {
                        escapeBlock = mEsc.Groups["esc"].Value.Trim();
                        aboveBlock = withTrim.Substring(0, mEsc.Index).Trim();
                    }
                    else
                    {
                        // No explicit Escape label: heuristically split by lines that mention “Escape …”
                        var lines = SplitRespLines(withTrim);
                        var above = new List<string>();
                        var esc = new List<string>();
                        foreach (var ln in lines)
                            if (Regex.IsMatch(ln, @"\bEscape\b", RegexOptions.IgnoreCase)) esc.Add(ln); else above.Add(ln);

                        if (above.Count > 0) aboveBlock = string.Join("\n", above);
                        if (esc.Count > 0) escapeBlock = string.Join("\n", esc);
                    }
                }

                // Helper: split block into distinct respirator list lines (keep APF, SCBA/SAR, gas mask, etc.)
                static List<string> SplitRespLines(string block)
                {
                    if (string.IsNullOrWhiteSpace(block)) return new List<string>();
                    var raw = block
                        .Replace("•", "\n").Replace("·", "\n")
                        .Replace("; ", "\n").Replace(". ", "\n")
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => W(s))
                        .Where(s => s.Length > 2)
                        .ToList();

                    bool Keep(string s) =>
                        Regex.IsMatch(s, @"\b(APF|SCBA|self-contained|supplied[-\s]?air|SAR|air[-\s]?line|positive[-\s]?pressure|gas mask|full[-\s]?facepiece|half[-\s]?facepiece|cartridge|canister|air[-\s]?purifying|escape|respirator)\b", RegexOptions.IgnoreCase)
                        || s.StartsWith("Any ", StringComparison.OrdinalIgnoreCase)
                        || s.StartsWith("Use ", StringComparison.OrdinalIgnoreCase)
                        || Regex.IsMatch(s, @"^\(APF\s*=\s*\d+", RegexOptions.IgnoreCase);

                    return raw.Where(Keep)
                              .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .ToList();
                }

                var aboveLines = SplitRespLines(aboveBlock);
                var escapeLines = SplitRespLines(escapeBlock);

                // Sometimes the “Above REL” preamble is a single sentence — keep it too
                if (!string.IsNullOrWhiteSpace(aboveBlock))
                {
                    var preface = Regex.Match(aboveBlock, @"^.{0,220}?(?=\(APF|\bAny\b|\bUse\b|\bSCBA\b|\bSAR\b|\bgas mask\b|$)", RegexOptions.IgnoreCase).Value.Trim();
                    if (!string.IsNullOrWhiteSpace(preface) && preface.Length > 10)
                        aboveLines.Insert(0, preface);
                }

                // Emit bullets if we have anything; do NOT drop anything in finalize.
                if (aboveLines.Count > 0)
                {
                    var bullets = string.Join(Environment.NewLine, aboveLines.Select(l => l.StartsWith("•") ? l : "• " + l));
                    dict["PPE (NPG): Respirator — above REL / detectable"] = bullets;
                }
                if (escapeLines.Count > 0)
                {
                    var bullets = string.Join(Environment.NewLine, escapeLines.Select(l => l.StartsWith("•") ? l : "• " + l));
                    dict["PPE (NPG): Respirator — escape"] = bullets;
                }
            }
            catch
            {
                // swallow; the caller will fall back to other extractors if any
            }
        }


        // Sweeps DOM to harvest respirator-looking lines from lists/tables even if the heading is missing.
        // (kept here for completeness; used elsewhere if needed)
        private static IEnumerable<string> HarvestRespiratorCandidatesFromDom(HtmlAgilityPack.HtmlDocument doc)
        {
            var outLines = new List<string>();

            // 1) List items
            var liNodes = doc.DocumentNode.SelectNodes("//li") ?? new HtmlNodeCollection(null);
            foreach (var li in liNodes)
            {
                var t = HtmlEntity.DeEntitize(li.InnerText ?? "");
                t = Regex.Replace(t, @"\s+", " ").Trim();
                if (IsRespiratorLike(t)) outLines.Add(t);
            }

            // 2) Tables
            var trNodes = doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlNodeCollection(null);
            foreach (var tr in trNodes)
            {
                var cells = tr.SelectNodes("./td|./th") ?? new HtmlNodeCollection(null);
                var parts = new List<string>();
                foreach (var c in cells)
                    parts.Add(HtmlEntity.DeEntitize(c.InnerText ?? ""));
                var row = string.Join(" — ", parts);
                row = Regex.Replace(row, @"\s+", " ").Trim();
                if (IsRespiratorLike(row)) outLines.Add(row);
            }

            // 3) Paragraphs/divs
            var pdNodes = doc.DocumentNode.SelectNodes("//p|//div") ?? new HtmlNodeCollection(null);
            foreach (var p in pdNodes)
            {
                var t = HtmlEntity.DeEntitize(p.InnerText ?? "");
                t = Regex.Replace(t, @"\s+", " ").Trim();
                if (IsRespiratorLike(t)) outLines.Add(t);
            }

            return outLines
                .Select(s => Regex.Replace(s ?? "", @"\s+", " ").Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsRespiratorLike(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = Regex.Replace(s, @"\s+", " ").Trim();
            if (t.Length < 3) return false;

            if (Regex.IsMatch(t, @"\b(APF|SCBA|self-contained|supplied-air|SAR|air[-\s]?line|positive-pressure|full facepiece|half facepiece|P100|cartridge|canister|air-purifying|escape|respirator)\b",
                RegexOptions.IgnoreCase))
                return true;

            if (t.StartsWith("Any ", StringComparison.OrdinalIgnoreCase)) return true;
            if (t.StartsWith("Use ", StringComparison.OrdinalIgnoreCase)) return true;
            if (t.StartsWith("Escape", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        public async Task<List<IhChemicalSamplingMethod>> GetSamplingMethodsByCasAsync(
            string cas,
            string? preferredName,
            IEnumerable<string>? synonyms,
            List<string>? unavailable = null,
            CancellationToken ct = default)
        {
            Debug.WriteLine($"[SM][START] cas='{cas}', pref='{preferredName}', synCount={synonyms?.Count() ?? 0}");

            var byKey = new Dictionary<(string source, string id), IhChemicalSamplingMethod>(
                new TupleIgnoreCaseComparer());

            // ---- NIOSH NMAM by CAS ----
            try
            {
                var sw = Stopwatch.StartNew();
                var nmamByCas = await GetNmamByCasAsync(cas, ct);
                sw.Stop();
                Debug.WriteLine($"[SM][NIOSH-CAS] found={nmamByCas.Count} in {sw.ElapsedMilliseconds} ms");

                foreach (var m in nmamByCas)
                {
                    var key = (m.Source, m.MethodId ?? "");
                    if (!byKey.ContainsKey(key)) byKey[key] = m;
                }
            }
            catch (Exception ex)
            {
                unavailable?.Add("NIOSH NMAM: CAS lookup error");
                Debug.WriteLine($"[SM][NIOSH-CAS][ERR] {ex.Message}");
            }

            // Names to try (for OSHA + NMAM by name)
            var nameSet = BuildMatchNameSet(preferredName, synonyms);

            // ---- OSHA SAM Index by NAME ----
            try
            {
                var sw = Stopwatch.StartNew();
                var fromIndex = await GetOshaSamIndexByNamesAsync(nameSet, ct);
                sw.Stop();
                Debug.WriteLine($"[SM][OSHA-INDEX-NAME] found={fromIndex.Count} in {sw.ElapsedMilliseconds} ms; names=[{string.Join(", ", nameSet)}]");

                foreach (var m in fromIndex)
                {
                    var key = (m.Source, m.MethodId ?? "");
                    if (!byKey.ContainsKey(key)) byKey[key] = m;
                }
            }
            catch (Exception ex)
            {
                unavailable?.Add("OSHA: SAM index name scan error");
                Debug.WriteLine($"[SM][OSHA-INDEX-NAME][ERR] {ex.Message}");
            }

            // ---- OSHA chemical detail page by CAS ----
            try
            {
                var sw = Stopwatch.StartNew();
                var viaDbCas = await GetOshaFromChemicalDbByCasAsync(cas, ct);
                sw.Stop();
                Debug.WriteLine($"[SM][OSHA-CHEM-CAS] found={viaDbCas.Count} in {sw.ElapsedMilliseconds} ms");

                foreach (var m in viaDbCas)
                {
                    var key = (m.Source, m.MethodId ?? "");
                    if (!byKey.ContainsKey(key)) byKey[key] = m;
                    else if (string.IsNullOrWhiteSpace(byKey[key].Url) && !string.IsNullOrWhiteSpace(m.Url))
                        byKey[key].Url = m.Url;
                }
            }
            catch (Exception ex)
            {
                unavailable?.Add("OSHA: chemical database page parse error (CAS)");
                Debug.WriteLine($"[SM][OSHA-CHEM-CAS][ERR] {ex.Message}");
            }

            // ---- OSHA chemical detail page by NAME ----
            try
            {
                var sw = Stopwatch.StartNew();
                var viaDbName = await GetOshaFromChemicalDbByNameAsync(nameSet, ct);
                sw.Stop();
                Debug.WriteLine($"[SM][OSHA-CHEM-NAME] found={viaDbName.Count} in {sw.ElapsedMilliseconds} ms; names=[{string.Join(", ", nameSet)}]");

                foreach (var m in viaDbName)
                {
                    var key = (m.Source, m.MethodId ?? "");
                    if (!byKey.ContainsKey(key)) byKey[key] = m;
                    else if (string.IsNullOrWhiteSpace(byKey[key].Url) && !string.IsNullOrWhiteSpace(m.Url))
                        byKey[key].Url = m.Url;
                }
            }
            catch (Exception ex)
            {
                unavailable?.Add("OSHA: chemical database page parse error (name)");
                Debug.WriteLine($"[SM][OSHA-CHEM-NAME][ERR] {ex.Message}");
            }

            // ---- NIOSH NMAM by NAME ----
            try
            {
                var sw = Stopwatch.StartNew();
                var nmamByName = await GetNmamByChemicalNameAsync(preferredName, synonyms, unavailable, ct);
                sw.Stop();
                Debug.WriteLine($"[SM][NIOSH-NAME] found={nmamByName.Count} in {sw.ElapsedMilliseconds} ms; pref='{preferredName}'");

                foreach (var m in nmamByName)
                {
                    var key = (m.Source, m.MethodId ?? "");
                    if (!byKey.ContainsKey(key)) byKey[key] = m;
                }
            }
            catch (Exception ex)
            {
                unavailable?.Add("NIOSH NMAM: name lookup error");
                Debug.WriteLine($"[SM][NIOSH-NAME][ERR] {ex.Message}");
            }

            var outList = byKey.Values.OrderBy(v => v.Source).ThenBy(v => v.MethodId).ToList();
            Debug.WriteLine($"[SM][DONE] total={outList.Count} unique methods. Keys=[{string.Join(" | ", outList.Select(o => $"{o.Source}:{o.MethodId}"))}]");
            return outList;
        }



        // Small helper to unify name candidates
        private static HashSet<string> BuildMatchNameSet(string? preferredName, IEnumerable<string>? synonyms)
{
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    void Add(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return;
        var v = s.Trim();
        if (v.Length >= 2) set.Add(v);
    }

    Add(preferredName);
    if (synonyms != null) foreach (var s in synonyms) Add(s);

    Debug.WriteLine($"[NAMES] pref='{preferredName}', synCount={synonyms?.Count() ?? 0}, names=[{string.Join(", ", set)}]");
    return set;
}




        private async Task<List<IhChemicalSamplingMethod>> GetOshaSamIndexByNamesAsync(
    HashSet<string> matchNames,
    CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();
            if (matchNames == null || matchNames.Count == 0) { Debug.WriteLine("[OSHA-INDEX] no names"); return results; }

            var oshaIndexUrl = "https://www.osha.gov/chemicaldata/sampling-analytical-methods";
            var html = await GetHtmlOrNullAsync(oshaIndexUrl, ct);
            if (string.IsNullOrEmpty(html)) { Debug.WriteLine("[OSHA-INDEX] page fetch failed"); return results; }

            var doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(html);

            int rowHits = 0, methodHits = 0;
            foreach (var tr in doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlNodeCollection(null))
            {
                var rowText = HtmlEntity.DeEntitize(tr.InnerText ?? string.Empty).Replace("\u00A0", " ").Trim();
                bool rowHasMatch = matchNames.Any(n => !string.IsNullOrWhiteSpace(n) &&
                                                       rowText.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
                if (!rowHasMatch) continue;
                rowHits++;

                foreach (var a in tr.SelectNodes(".//a") ?? new HtmlNodeCollection(null))
                {
                    var hrefAbs = ResolveHref(a.GetAttributeValue("href", string.Empty), oshaIndexUrl);
                    hrefAbs = NormalizeOshaMethodUrl(hrefAbs);

                    var code = DeriveOshaMethodCode(hrefAbs, HtmlEntity.DeEntitize(a.InnerText ?? string.Empty));
                    if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("ID", StringComparison.OrdinalIgnoreCase))
                        continue;

                    methodHits++;
                    results.Add(new IhChemicalSamplingMethod { Source = "OSHA", MethodId = code, Url = hrefAbs });
                    Debug.WriteLine($"[OSHA-INDEX] +{code} url='{hrefAbs}'");
                }
            }

            Debug.WriteLine($"[OSHA-INDEX] rowHits={rowHits}, methodHits={methodHits}, names=[{string.Join(", ", matchNames)}]");
            return results
                .GroupBy(m => (m.Source, m.MethodId, m.Url ?? ""))
                .Select(g => g.First())
                .ToList();
        }



        private async Task<List<IhChemicalSamplingMethod>> GetOshaFromChemicalDbByNameAsync(
            HashSet<string> matchNames,
            CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();
            if (matchNames == null || matchNames.Count == 0)
            {
                Debug.WriteLine("[OSHA-CHEM-NAME] no names");
                return results;
            }

            // Tighten scan window and add safety exits.
            const int MAX_PAGES = 5;            // was 60 — keep this small to avoid long scans / bans
            const int MAX_EMPTY_STREAK = 2;     // break if we see this many pages with zero links
            const int EARLY_RETURN_AFTER = 3;   // return as soon as we have this many distinct OSHA methods

            int emptyStreak = 0;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Source:ID dedupe

            Debug.WriteLine($"[OSHA-CHEM-NAME][BEGIN] names=[{string.Join(", ", matchNames)}]");

            for (int page = 1; page <= MAX_PAGES; page++)
            {
                ct.ThrowIfCancellationRequested();

                var links = await EnumerateOshaSearchPageChemLinksAsync(page, ct);
                Debug.WriteLine($"[OSHA-CHEM-NAME][p{page}] links={links.Count}");

                // If the page is blocked/empty, count it and possibly bail out early.
                if (links.Count == 0)
                {
                    emptyStreak++;
                    if (emptyStreak >= MAX_EMPTY_STREAK)
                    {
                        Debug.WriteLine("[OSHA-CHEM-NAME] consecutive empty/blocked pages — stopping early");
                        break;
                    }
                    continue;
                }
                else
                {
                    emptyStreak = 0; // reset once we get a non-empty page
                }

                foreach (var (displayName, chemUrl) in links)
                {
                    ct.ThrowIfCancellationRequested();

                    // Prefilter by preferred name / synonyms (case-insensitive contains match)
                    bool nameHit = matchNames.Any(n =>
                        !string.IsNullOrWhiteSpace(n) &&
                        displayName.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (!nameHit) continue;

                    var methods = await ParseOshaChemicalDetailForMethodsAsync(chemUrl, ct);
                    Debug.WriteLine($"[OSHA-CHEM-NAME][DETAIL] '{displayName}' methods={methods.Count}");

                    foreach (var m in methods)
                    {
                        var key = $"{m.Source}:{m.MethodId}";
                        if (seen.Add(key))
                        {
                            results.Add(m);
                            // Return early once we have "enough" to be useful.
                            if (results.Count >= EARLY_RETURN_AFTER)
                            {
                                Debug.WriteLine($"[OSHA-CHEM-NAME][EARLY-RETURN] total={results.Count}");
                                return results;
                            }
                        }
                    }
                }
            }

            Debug.WriteLine($"[OSHA-CHEM-NAME][END] found={results.Count}");
            return results;
        }




        private async Task<List<IhChemicalSamplingMethod>> ParseOshaChemicalDetailForMethodsAsync(
    string chemUrl,
    CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();
            var chemHtml = await GetHtmlOrNullAsync(chemUrl, ct);
            if (string.IsNullOrWhiteSpace(chemHtml)) { Debug.WriteLine($"[OSHA-CHEM-DETAIL] no html for '{chemUrl}'"); return results; }

            var cdoc = new HtmlDocument(); cdoc.LoadHtml(chemHtml);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int aCount = 0, aHit = 0;
            foreach (var link in cdoc.DocumentNode.SelectNodes("//a") ?? new HtmlNodeCollection(null))
            {
                aCount++;
                var anchorText = HtmlEntity.DeEntitize(link.InnerText ?? "").Trim();
                var hrefAbs = ResolveHref(link.GetAttributeValue("href", string.Empty), chemUrl);
                hrefAbs = NormalizeOshaMethodUrl(hrefAbs);

                var code = DeriveOshaMethodCode(hrefAbs, anchorText);
                if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("ID", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (seen.Add(code))
                {
                    aHit++;

                    // If href isn't a /methods/ path, synthesize the standard OSHA PDF path
                    if (string.IsNullOrWhiteSpace(hrefAbs) || !hrefAbs.Contains("/methods/", StringComparison.OrdinalIgnoreCase))
                    {
                        var slug = code.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
                        hrefAbs = $"https://www.osha.gov/sites/default/files/methods/osha-{slug}.pdf";
                    }

                    // >>> CHANGED: normalize for https + encoded spaces, or synthesize if still blank
                    hrefAbs = NormalizeOshaPdfUrl(hrefAbs, code);

                    results.Add(new IhChemicalSamplingMethod { Source = "OSHA", MethodId = code, Url = hrefAbs });
                    Debug.WriteLine($"[OSHA-CHEM-DETAIL] +{code} url='{hrefAbs}' (from anchor '{anchorText}')");
                }
            }

            var fullText = HtmlEntity.DeEntitize(cdoc.DocumentNode.InnerText ?? string.Empty);
            int textHits = 0;
            foreach (Match m in Regex.Matches(fullText, @"OSHA\s+ID[-\s]?(\d+[A-Za-z]*)", RegexOptions.IgnoreCase))
            {
                var code = "ID" + m.Groups[1].Value.ToUpperInvariant();
                if (!seen.Add(code)) continue;

                textHits++;

                // Build default PDF and normalize it (handles spaces/https)
                var slug = code.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
                var url = $"https://www.osha.gov/sites/default/files/methods/osha-{slug}.pdf";
                url = NormalizeOshaPdfUrl(url, code); // >>> CHANGED

                results.Add(new IhChemicalSamplingMethod { Source = "OSHA", MethodId = code, Url = url });
                Debug.WriteLine($"[OSHA-CHEM-DETAIL][TEXT] +{code} url='{url}'");
            }

            Debug.WriteLine($"[OSHA-CHEM-DETAIL] anchors={aCount}, anchorHits={aHit}, textHits={textHits}, total={results.Count} ({chemUrl})");
            return results;
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
            if (synonyms != null) foreach (var s in synonyms) AddName(s);
            if (nameSet.Count == 0) return results;

            var letters = new HashSet<char>();
            foreach (var n in nameSet)
            {
                var first = n.TrimStart().FirstOrDefault(char.IsLetter);
                if (first != default) letters.Add(char.ToLowerInvariant(first));
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

                var letterRows = doc.DocumentNode.SelectNodes("//table//tr[td]") ?? new HtmlAgilityPack.HtmlNodeCollection(null);

                foreach (var tr in letterRows)
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count < 2) continue;

                    var chemicalText = HtmlAgilityPack.HtmlEntity.DeEntitize(tds[0].InnerText ?? "").Trim();

                    bool match = nameSet.Any(n => chemicalText.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!match) continue;

                    var a = tds[1].SelectSingleNode(".//a");
                    if (a == null) continue;

                    var aText = HtmlAgilityPack.HtmlEntity.DeEntitize(a.InnerText ?? "").Trim();
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

            return results
                .GroupBy(m => (m.Source, m.MethodId, m.Url ?? ""))
                .Select(g => g.First())
                .ToList();
        }


        //// OSHA per-chemical page (also used to pull method links)
        private static readonly MemoryCache _oshaChemCache = new(new MemoryCacheOptions());

        private async Task<List<IhChemicalSamplingMethod>> GetOshaFromChemicalDbByCasAsync(
            string cas, CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();
            var wantDigits = new string((cas ?? "").Where(char.IsDigit).ToArray());
            var visited = new ConcurrentDictionary<string, byte>();
            var found = false;

            // Cache: if we already resolved this CAS recently, return it.
            if (_oshaChemCache.TryGetValue($"osha:methods:{wantDigits}", out List<IhChemicalSamplingMethod> cached))
                return cached;

            Debug.WriteLine($"[OSHA-CHEM-CAS][BEGIN] cas='{cas}'");

            // Limit concurrent detail fetches to avoid 403s.
            using var gate = new SemaphoreSlim(6);

            for (int page = 1; page <= 60 && !found; page++)
            {
                var links = await EnumerateOshaSearchPageChemLinksAsync(page, ct);
                Debug.WriteLine($"[OSHA-CHEM-CAS][p{page}] links={links.Count}");
                if (links.Count == 0) continue;

                var tasks = links.Select(async tuple =>
                {
                    if (found) return; // early skip if someone already matched
                    var (name, chemUrl) = tuple;

                    if (!visited.TryAdd(chemUrl, 0)) return;

                    await gate.WaitAsync(ct);
                    try
                    {
                        var html = await GetHtmlOrNullAsync(chemUrl, ct);
                        if (string.IsNullOrWhiteSpace(html))
                        {
                            Debug.WriteLine($"[OSHA-CHEM-CAS][DETAIL] no html for '{chemUrl}'");
                            return;
                        }

                        var cdoc = new HtmlAgilityPack.HtmlDocument();
                        cdoc.LoadHtml(html);

                        var casSet = ExtractCasNumbersFromDetail(cdoc);
                        bool match = casSet.Contains(cas) || casSet.Contains(wantDigits);
                        Debug.WriteLine($"[OSHA-CHEM-CAS][DETAIL] name='{name}', match={match}, casSeen=[{string.Join(", ", casSet.Take(5))}{(casSet.Count > 5 ? "…" : "")}]");

                        if (!match) return;

                        var methods = await ParseOshaChemicalDetailForMethodsAsync(chemUrl, ct);
                        Debug.WriteLine($"[OSHA-CHEM-CAS][DETAIL] methods={methods.Count} for '{name}'");

                        if (methods.Count > 0)
                        {
                            lock (results)
                            {
                                foreach (var m in methods)
                                {
                                    if (!results.Any(x => x.Source == m.Source && x.MethodId == m.MethodId))
                                        results.Add(m);
                                }
                            }
                        }

                        found = true; // signal others to stop work ASAP
                    }
                    finally
                    {
                        gate.Release();
                    }
                });

                // Wait for all link checks on this page (they'll self-short-circuit when found==true)
                await Task.WhenAll(tasks);

                if (found) break;
            }

            // Cache successful (or empty) result briefly to avoid immediate repeats.
            _oshaChemCache.Set($"osha:methods:{wantDigits}", results, TimeSpan.FromMinutes(30));

            Debug.WriteLine($"[OSHA-CHEM-CAS][END] found={results.Count}");
            return results;
        }



        // ------------------------------------------------------------
        // OSHA OELs (per-chemical page) — CAS based
        // ------------------------------------------------------------
        public async Task<List<IhChemicalOel>> GetOelsByCasAsync(
            string cas, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var oels = new List<IhChemicalOel>();
            var chemUrl = await FindOshaChemicalUrlByCasAsync(cas, ct);
            if (string.IsNullOrWhiteSpace(chemUrl))
            {
                unavailable?.Add("OSHA OELs: per-chemical page not found");
                return oels;
            }

            var chemHtml = await GetHtmlOrNullAsync(chemUrl!, ct);
            if (string.IsNullOrWhiteSpace(chemHtml))
            {
                unavailable?.Add("OSHA OELs: chemical page blocked/unavailable");
                return oels;
            }

            var cdoc = new HtmlDocument();
            cdoc.LoadHtml(chemHtml);

            string expoText = "";
            var heading = cdoc.DocumentNode.SelectSingleNode("//*[self::h1 or self::h2 or self::h3][contains(., 'Exposure Limits')]");
            if (heading != null)
            {
                var table = heading.SelectSingleNode("following::table[1]");
                if (table != null)
                    expoText = HtmlEntity.DeEntitize(table.InnerText ?? string.Empty);
            }
            if (string.IsNullOrWhiteSpace(expoText))
                expoText = HtmlEntity.DeEntitize(cdoc.DocumentNode.InnerText ?? string.Empty);

            expoText = Regex.Replace(expoText, @"\s+", " ").Trim();

            void Add(string source, string limitType, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                var v = value;
                var cut = v.IndexOf(" [", StringComparison.Ordinal);
                if (cut > 0) v = v.Substring(0, cut).Trim();
                oels.Add(new IhChemicalOel { Source = source, Type = limitType, Value = v });
            }

            void ParseSeries(string tag, string src, string kind, string typeLabel)
            {
                var rx = new Regex($@"{tag}\s*{kind}\s+(.+?)(?=(?:\s+(?:PEL|REL|TLV|Action Level|Peak|Cal/OSHA|Californi a\s+OSHA)\b|$))", RegexOptions.IgnoreCase);
                var seen = 0;
                foreach (Match m in rx.Matches(expoText))
                {
                    var val = m.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(val)) continue;
                    seen++;
                    if (src == "OSHA")
                    {
                        if (seen == 1) Add("OSHA", typeLabel, val);
                        else if (seen == 2) Add("Cal/OSHA", typeLabel, val);
                    }
                    else Add(src, typeLabel, val);
                }
            }

            ParseSeries("PEL", "OSHA", "TWA", "TWA");
            ParseSeries("PEL", "OSHA", "STEL", "STEL");
            ParseSeries("PEL", "OSHA", "C(?:eiling)?", "Ceiling");

            ParseSeries("REL", "NIOSH", "TWA", "TWA");
            ParseSeries("REL", "NIOSH", "STEL", "STEL");
            ParseSeries("REL", "NIOSH", "C(?:eiling)?", "Ceiling");

            ParseSeries("TLV", "ACGIH", "TWA", "TWA");
            ParseSeries("TLV", "ACGIH", "STEL", "STEL");
            ParseSeries("TLV", "ACGIH", "C(?:eiling)?", "Ceiling");

            return oels
                .GroupBy(x => $"{x.Source}|{x.Type}|{x.Value}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        private async Task<string?> FindOshaChemicalUrlByCasAsync(string cas, CancellationToken ct)
        {
            static string NormalizeHyphens(string s) =>
                string.IsNullOrEmpty(s) ? s
                : Regex.Replace(s, "[\u2010-\u2015\u2212\uFE63\uFF0D]", "-");

            static string DigitsOnly(string s) =>
                new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

            var casNorm = NormalizeHyphens(cas?.Trim() ?? "");
            var casDigits = DigitsOnly(casNorm);

            string? chemUrl = null;

            for (int page = 1; page <= 60 && chemUrl == null; page++)
            {
                var searchUrl = $"https://www.osha.gov/chemicaldata/search?page={page}";
                var html = await GetHtmlOrNullAsync(searchUrl, ct);
                if (string.IsNullOrWhiteSpace(html)) continue;

                try
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var rows = doc.DocumentNode.SelectNodes("//table//tr[td]");
                    if (rows != null)
                    {
                        foreach (var tr in rows)
                        {
                            var link = tr.SelectSingleNode(".//a[contains(@href, '/chemicaldata/')]");
                            var casTd = tr.SelectSingleNode("./td[2]");
                            if (link == null || casTd == null) continue;

                            var rowCas = HtmlEntity.DeEntitize(casTd.InnerText ?? string.Empty).Trim();
                            rowCas = NormalizeHyphens(rowCas);

                            if (DigitsOnly(rowCas) == casDigits)
                            {
                                chemUrl = ResolveHref(link.GetAttributeValue("href", string.Empty), searchUrl);
                                break;
                            }
                        }
                    }
                }
                catch { }

                if (chemUrl == null)
                {
                    foreach (Match m in Regex.Matches(
                                 html, "<a[^>]+href=[\"'](?<href>/chemicaldata/\\d+)[\"'][^>]*>(?<text>.*?)</a>",
                                 RegexOptions.IgnoreCase | RegexOptions.Singleline))
                    {
                        int idx = m.Index;
                        int start = Math.Max(0, idx - 1200);
                        int len = Math.Min(html.Length - start, 2400);
                        var window = html.Substring(start, len);

                        var windowNorm = NormalizeHyphens(HtmlEntity.DeEntitize(window));
                        var windowDigits = DigitsOnly(windowNorm);

                        if (windowNorm.IndexOf(casNorm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            windowDigits.IndexOf(casDigits, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            chemUrl = ResolveHref(m.Groups["href"].Value, searchUrl);
                            break;
                        }
                    }
                }
            }
            return chemUrl;
        }

        // ------------------------------------------------------------
        // NMAM whitelist cache + helpers (unchanged)
        // ------------------------------------------------------------
        private static readonly SemaphoreSlim _nmamCacheLock = new(1, 1);
        private static DateTime _nmamCacheExpires = DateTime.MinValue;
        private static HashSet<string> _nmamMethodSet = new(StringComparer.OrdinalIgnoreCase);

        private static bool IsCasFragment(string maybeNumber, string cas)
        {
            if (string.IsNullOrWhiteSpace(maybeNumber) || string.IsNullOrWhiteSpace(cas)) return false;
            var firstSeg = Regex.Match(cas, @"^\s*([0-9]+)-").Groups[1].Value;
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

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    foreach (var a in doc.DocumentNode.SelectNodes("//table//tr/td[2]//a") ?? new HtmlNodeCollection(null))
                    {
                        var txt = HtmlEntity.DeEntitize(a.InnerText ?? "").Trim();
                        var m = Regex.Match(txt, @"\b(\d{3,4})\b");
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

        private async Task<bool> IsValidNioshMethodAsync(string? num, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(num)) return false;
            var set = await GetNmamMethodNumbersAsync(ct);
            return set.Contains(num);
        }

        public Task<bool> IsValidNioshMethodNumberAsync(string num, CancellationToken ct = default)
            => IsValidNioshMethodAsync(num, ct);

        // ------------------------------------------------------------
        // Small OSHA helpers (unchanged from your earlier version)
        // ------------------------------------------------------------
        private static string? NormalizeOshaMethodUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var u)) return url;
            if (!u.Host.Contains("osha.gov", StringComparison.OrdinalIgnoreCase)) return url;

            var path = u.AbsolutePath;
            if (path.EndsWith("/methods/index.html", StringComparison.OrdinalIgnoreCase)) return null;

            var m = Regex.Match(path, @"\b(id[\-_]?\d+[a-z]*)\b", RegexOptions.IgnoreCase);
            string? code = null;
            if (m.Success) code = m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
            else
            {
                var file = System.IO.Path.GetFileName(path);
                var m2 = Regex.Match(file, @"osha-(id\d+[a-z]*)", RegexOptions.IgnoreCase);
                if (m2.Success) code = m2.Groups[1].Value.ToLowerInvariant();
            }

            if (!string.IsNullOrWhiteSpace(code))
                return $"https://www.osha.gov/sites/default/files/methods/osha-{code}.pdf";

            if (path.Contains("/methods/", StringComparison.OrdinalIgnoreCase) &&
                path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                var pdfPath = System.IO.Path.ChangeExtension(path, ".pdf");
                return new UriBuilder(u) { Path = pdfPath }.Uri.ToString();
            }

            return url;
        }

        private static string DeriveOshaMethodCode(string? url, string? textFallback)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var m = Regex.Match(url, @"\b(id[\-_]?\d+[a-z]*)\b", RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();
            }
            if (!string.IsNullOrWhiteSpace(textFallback))
            {
                var m = Regex.Match(textFallback, @"\b(id[\-_]?\d+[a-z]*)\b", RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();
            }
            return string.IsNullOrWhiteSpace(textFallback) ? "OSHA METHOD" : textFallback;
        }

        // Normalize and collapse whitespace
        private static string W(string s)
        {
            s = HtmlEntity.DeEntitize(s ?? "");
            s = Regex.Replace(s, @"\s+", " ").Trim();
            return s;
        }

        // Find the start node of a section by heading text OR by id/name hints
        private static HtmlNode? FindSectionStart(HtmlDocument doc, string[] headings, string[] idHints)
        {
            static string N(string s)
            {
                s = HtmlEntity.DeEntitize(s ?? "");
                s = Regex.Replace(s, @"[&/]", " and ", RegexOptions.IgnoreCase);
                s = Regex.Replace(s, @"[^a-z0-9\s\-]", " ", RegexOptions.IgnoreCase);
                s = s.ToLowerInvariant();
                s = Regex.Replace(s, @"\s+", " ").Trim();
                return s;
            }

            var wanted = headings.Select(N).ToArray();

            foreach (var h in doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//h5|//h6|//*[@role='heading']") ?? new HtmlNodeCollection(null))
            {
                var ht = N(h.InnerText);
                if (wanted.Any(w => ht.Contains(w))) return h;
            }

            foreach (var el in doc.DocumentNode.SelectNodes("//*[@id or @name]") ?? new HtmlNodeCollection(null))
            {
                var id = (el.GetAttributeValue("id", "") + " " + el.GetAttributeValue("name", "")).ToLowerInvariant();
                if (idHints.Any(h => id.Contains(h))) return el;
            }

            return null;
        }

        // Extract section lines between the section start and the next heading
        private static string[] ExtractSectionLines(HtmlDocument doc, string[] headings, string[] idHints)
        {
            var start = FindSectionStart(doc, headings, idHints);
            if (start == null) return Array.Empty<string>();

            var pieces = new List<string>();
            for (var n = start.NextSibling; n != null; n = n.NextSibling)
            {
                if (n.NodeType == HtmlNodeType.Element &&
                    (n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase) ||
                     n.Name.Equals("h2", StringComparison.OrdinalIgnoreCase) ||
                     n.Name.Equals("h3", StringComparison.OrdinalIgnoreCase) ||
                     n.Name.Equals("h4", StringComparison.OrdinalIgnoreCase) ||
                     n.Name.Equals("h5", StringComparison.OrdinalIgnoreCase) ||
                     n.Name.Equals("h6", StringComparison.OrdinalIgnoreCase)))
                    break;

                if (n.NodeType != HtmlNodeType.Element) continue;

                if (n.Name.Equals("ul", StringComparison.OrdinalIgnoreCase) ||
                    n.Name.Equals("ol", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var li in n.SelectNodes(".//li") ?? new HtmlNodeCollection(null))
                    {
                        var t = W(li.InnerText);
                        if (t.Length > 2) pieces.Add(t);
                    }
                    continue;
                }

                if (n.Name.Equals("table", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var tr in n.SelectNodes(".//tr[td]") ?? new HtmlNodeCollection(null))
                    {
                        var cells = tr.SelectNodes("./td|./th") ?? new HtmlNodeCollection(null);
                        var row = string.Join(" — ", cells.Select(c => W(c.InnerText)).Where(s => !string.IsNullOrWhiteSpace(s)));
                        if (row.Length > 2) pieces.Add(row);
                    }
                    continue;
                }

                if (n.Name.Equals("p", StringComparison.OrdinalIgnoreCase) ||
                    n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) ||
                    n.Name.Equals("section", StringComparison.OrdinalIgnoreCase))
                {
                    var text = W(n.InnerText);
                    if (text.Length > 2)
                    {
                        foreach (var part in text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .SelectMany(x => x.Split(';'))
                                                 .Select(s => W(s)))
                            if (part.Length > 2) pieces.Add(part);
                    }
                }
            }

            return pieces
                .Select(s => Regex.Replace(s, @"(Page last reviewed:.*|Content source:.*)$", "", RegexOptions.IgnoreCase).Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        // === PPE helpers =====================================================

        private static string[] MergeDistinct(params IEnumerable<string>[] sets)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var outList = new List<string>();
            foreach (var set in sets)
            {
                if (set == null) continue;
                foreach (var s in set)
                {
                    var t = Regex.Replace((s ?? ""), @"\s+", " ").Trim();
                    if (t.Length < 3) continue;
                    if (seen.Add(t)) outList.Add(t);
                }
            }
            return outList.ToArray();
        }

        private static string[] ExtractPersonalKeyValueFallback(HtmlDocument doc)
        {
            string all = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? "");
            all = Regex.Replace(all, @"\s+", " ").Trim();

            static string Slice(string text, string startPattern, params string[] untilPatterns)
            {
                var m = Regex.Match(text, startPattern, RegexOptions.IgnoreCase);
                if (!m.Success) return string.Empty;
                int start = m.Index + m.Length, end = text.Length;
                foreach (var up in untilPatterns)
                {
                    var u = Regex.Match(text, up, RegexOptions.IgnoreCase);
                    if (u.Success && u.Index > start && u.Index < end) end = u.Index;
                }
                return text.Substring(start, Math.Max(0, end - start)).Trim();
            }

            var block = Slice(
                all,
                @"Personal\s+protection\s*(?:&|and|/)?\s*sanitation\s*",
                "Respirator\\s+recommendations", "Respiratory\\s+protection", "Respirator",
                "Exposure\\s+controls", "First\\s+Aid", "Spills", "Appendix", "Page\\s+last\\s+reviewed"
            );
            if (string.IsNullOrWhiteSpace(block)) block = all;

            var keys = new[]
            {
                "Skin", "Eyes", "Wash skin", "Remove", "Change", "Provide",
                "Gloves", "Clothing", "Face", "Hands", "Contact lenses"
            };

            var lines = new List<string>();

            foreach (var k in keys)
            {
                var rx = new Regex($@"\b{k}\s*:\s*(?<v>[^;.\n\r]+)", RegexOptions.IgnoreCase);
                foreach (Match m in rx.Matches(block))
                {
                    var val = Regex.Replace(m.Groups["v"].Value, @"\s+", " ").Trim();
                    if (val.Length > 1) lines.Add($"{k}: {val}");
                }
            }

            if (lines.Count < 3)
            {
                foreach (Match m in Regex.Matches(block, @"\b([A-Za-z ]{3,30}):\s*([^;.\n\r]+)"))
                {
                    var key = Regex.Replace(m.Groups[1].Value, @"\s+", " ").Trim();
                    var val = Regex.Replace(m.Groups[2].Value, @"\s+", " ").Trim();
                    if (key.Length > 2 && val.Length > 1) lines.Add($"{key}: {val}");
                }
            }

            return lines
                .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string[] ExtractBroadPersonalCandidates(HtmlDocument doc)
        {
            string all = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? "");
            all = System.Text.RegularExpressions.Regex.Replace(all, @"\s+", " ").Trim();

            var bits = System.Text.RegularExpressions.Regex.Split(all, @"[.;]\s+")
                .Concat(all.Split('\n'))
                .Select(s => System.Text.RegularExpressions.Regex.Replace(s ?? "", @"\s+", " ").Trim())
                .Where(s => s.Length > 6);

            // Strict filter: keep PPE-like lines ONLY; explicitly drop non-PPE sections/phrases
            var keep = bits.Where(s =>
                !System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(First Aid|Symptoms|Target Organs|Carcinogen|TLV|REL|PEL|NIOSH\s+REL|ACGIH|OSHA|Respirator|Escape|APF|Exposure Limit|Concentration|Recommendations)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase) &&
                (
                    System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(glove|goggle|eye|face[-\s]?shield|apron|protective clothing|impervious|wash skin|remove clothing|change clothing|contact lenses|Skin:|Eyes:|Gloves?:|Face[-\s]?shield:|Wash skin:|Remove:|Change:|Provide:)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                    s.StartsWith("Avoid ", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("Do not ", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("Provide ", StringComparison.OrdinalIgnoreCase)
                )
            );

            return keep
                .Select(s => System.Text.RegularExpressions.Regex.Replace(s ?? "", @"\s+", " ").Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }


        private static string[] NormalizePersonalKeyValues(IEnumerable<string> inLines)
        {
            if (inLines == null) return Array.Empty<string>();

            // Canonical keys we allow in the Personal section
            var allowedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Eyes","Skin","Gloves","Face shield","Wash skin","Remove","Change","Provide",
        "Protective clothing","Eye protection","Contact lenses","Notes"
    };

            static string CanonKey(string k)
            {
                k = Regex.Replace(k ?? "", @"\s+", " ").Trim();
                if (k.Length == 0) return k;

                k = Regex.Replace(k, @"(?i)wash\s*skin", "Wash skin");
                k = Regex.Replace(k, @"(?i)face[-\s]*shield", "Face shield");
                k = Regex.Replace(k, @"(?i)gloves?", "Gloves");
                k = Regex.Replace(k, @"(?i)eyes?", "Eyes");
                k = Regex.Replace(k, @"(?i)\bskin\b", "Skin");
                k = Regex.Replace(k, @"(?i)remove", "Remove");
                k = Regex.Replace(k, @"(?i)change", "Change");
                k = Regex.Replace(k, @"(?i)provide", "Provide");
                k = Regex.Replace(k, @"(?i)protective\s*clothing", "Protective clothing");
                k = Regex.Replace(k, @"(?i)eye\s*protection", "Eye protection");
                k = Regex.Replace(k, @"(?i)contact\s*lenses", "Contact lenses");
                return k;
            }

            // Things that are **not** personal PPE content (keep these OUT)
            static bool LooksLikeNonPpe(string v)
            {
                if (string.IsNullOrWhiteSpace(v)) return true;
                var t = v;
                // anything referencing exposure limits, respirators, APF, symptoms, first aid, etc.
                if (Regex.IsMatch(t, @"\b(First Aid|Symptoms|Target Organs|Carcinogen|TLV|REL|PEL|NIOSH|OSHA|ACGIH|Respirator|Escape|APF|Exposure Limit|Concentration|Recommendations)\b", RegexOptions.IgnoreCase))
                    return true;
                // Long enumerations that usually belong to health effects
                if (Regex.IsMatch(t, @"\b(lacrimation|cough|wheezing|respiratory system|throat|nose|breathing)\b", RegexOptions.IgnoreCase))
                    return true;
                return false;
            }

            // Score stronger, more actionable language higher
            static int Score(string v)
            {
                if (string.IsNullOrWhiteSpace(v)) return 0;
                var t = v.Trim();
                if (t.Equals("No recommendation", StringComparison.OrdinalIgnoreCase)) return 1;
                int bonus = Regex.IsMatch(t, @"\b(prevent|avoid|use|wear|do not|provide|impervious|goggle|glove|face|shield)\b", RegexOptions.IgnoreCase) ? 20 : 0;
                return bonus + Math.Min(200, t.Length);
            }

            static string InsertKeyDelimiters(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                s = s.Replace("•", " ").Replace("·", " ");
                s = Regex.Replace(s, @"\s+", " ").Trim();

                var keyAlt = @"Wash\s*skin|Face\s*shield|Contact\s*lenses|Protective\s*clothing|Eye\s*protection|Skin|Eyes|Gloves|Remove|Change|Provide|Notes";

                s = Regex.Replace(
                    s,
                    $@"\b({keyAlt})\b\s*:?",
                    m => $" §{m.Groups[1].Value}:",
                    RegexOptions.IgnoreCase
                );

                return s;
            }

            // non-static so it can see allowedKeys
            IEnumerable<(string key, string val)> ExtractKVsFromOneLine(string line)
            {
                var s = InsertKeyDelimiters(line);
                if (string.IsNullOrWhiteSpace(s)) yield break;

                var segments = s.Split('§')
                                .Select(t => t.Trim())
                                .Where(t => t.Length > 0)
                                .ToArray();

                foreach (var seg in segments)
                {
                    var m = Regex.Match(seg, @"^(?<key>[A-Za-z][A-Za-z \-/]{2,40})\s*:\s*(?<val>.+)$");
                    if (!m.Success) continue;

                    var key = CanonKey(m.Groups["key"].Value);
                    var val = m.Groups["val"].Value;

                    // chop at next embedded key if present
                    var cut = Regex.Match(val, @"\b(Wash\s*skin|Face\s*shield|Contact\s*lenses|Protective\s*clothing|Eye\s*protection|Skin|Eyes|Gloves|Remove|Change|Provide|Notes)\b\s*:", RegexOptions.IgnoreCase);
                    if (cut.Success) val = val.Substring(0, cut.Index).Trim();

                    val = Regex.Replace(val, @"^\W+|\W+$", "").Trim();
                    val = Regex.Replace(val, @"\s+", " ");

                    if (allowedKeys.Contains(key) && val.Length > 0 && !LooksLikeNonPpe(val))
                        yield return (key, val);
                }
            }

            var best = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in inLines)
            {
                foreach (var (key, val) in ExtractKVsFromOneLine(line))
                {
                    if (!best.TryGetValue(key, out var exist) || Score(val) > Score(exist))
                        best[key] = val;
                }
            }

            if (best.Count == 0)
            {
                // Strict fallback: keep only sentences with PPE keywords, and still exclude non-PPE blocks.
                var filtered = (inLines ?? Array.Empty<string>())
                    .Select(s => Regex.Replace(s ?? "", @"\s+", " ").Trim())
                    .Where(s => s.Length > 2 && !LooksLikeNonPpe(s))
                    .Where(s => Regex.IsMatch(s, @"\b(glove|goggle|eye|face[-\s]?shield|apron|protective clothing|impervious|wash skin|remove clothing|change clothing|contact lenses)\b", RegexOptions.IgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                return filtered;
            }

            string[] order = { "Eyes", "Skin", "Gloves", "Face shield", "Wash skin", "Remove", "Change", "Provide", "Protective clothing", "Eye protection", "Contact lenses", "Notes" };
            var outArr = best
                .OrderBy(kv => Array.IndexOf(order, kv.Key) < 0 ? int.MaxValue : Array.IndexOf(order, kv.Key))
                .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                .Select(kv => $"{kv.Key}: {kv.Value}")
                .ToArray();

            return outArr;
        }


        private static string BulletJoin(IEnumerable<string> items)
        {
            var arr = items?
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? Array.Empty<string>();

            // One bullet per line, explicitly.
            return string.Join(Environment.NewLine, arr.Select(s => s.StartsWith("•") ? s : "• " + s));
        }


        private static string CanonPpeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return key ?? "";
            key = Regex.Replace(key, @"\s+", " ").Trim();
            key = key.Replace(" - ", " — ").Replace("–", "—");
            key = Regex.Replace(key, @"\s*—\s*", " — ");

            // Personal protection (root + optional subkey)
            var mPersonal = Regex.Match(
                key,
                @"^(?<root>(?:PPE\s*\(NPG\)\s*:\s*)?Personal\s*protection\s*&?\s*(?:and|/)?\s*sanitation)\s*(?:—\s*(?<sub>.+))?$",
                RegexOptions.IgnoreCase);
            if (mPersonal.Success)
            {
                var sub = mPersonal.Groups["sub"].Success ? mPersonal.Groups["sub"].Value.Trim() : null;
                const string root = "PPE (NPG): Personal protection & sanitation";
                return sub is null ? root : $"{root} — {sub}";
            }

            // Respirator buckets: accept ANY heading/label that implies respirators
            if (Regex.IsMatch(key, @"\bRespirator", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(key, @"\bRespiratory\s+protection\b", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(key, @"\bRespirator\s+recommendations\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(key, @"\bEscape\b", RegexOptions.IgnoreCase))
                    return "PPE (NPG): Respirator — escape";

                if (Regex.IsMatch(key, @"above\s+.*REL", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(key, @"detectable", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(key, @"no\s+REL", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(key, @"unknown\s+concentration", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(key, @"at\s+concentrations\s+above", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(key, @"where\s+there\s+is\s+no\s+REL", RegexOptions.IgnoreCase))
                    return "PPE (NPG): Respirator — above REL / detectable";

                return "PPE (NPG): Respirator — above REL / detectable";
            }

            // Already canonicalized respirator key
            if (key.StartsWith("PPE (NPG): Respirator", StringComparison.OrdinalIgnoreCase))
                return key;

            return key;
        }


        private static (string[] above, string[] escape) ExtractRespiratorRows(HtmlAgilityPack.HtmlDocument doc)
        {
            static string WT(string s) => System.Text.RegularExpressions.Regex.Replace(HtmlAgilityPack.HtmlEntity.DeEntitize(s ?? ""), @"\s+", " ").Trim();

            // Find the start of the respirator section by headings
            var heads = doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//*[@role='heading']") ?? new HtmlNodeCollection(null);
            HtmlNode hit = null;
            foreach (var h in heads)
            {
                var t = WT(h.InnerText).ToLowerInvariant();
                if (t.Contains("respirator recommendations") || t.Contains("respiratory protection") || t.Equals("respirator"))
                {
                    hit = h; break;
                }
            }

            // Collect nodes until the next heading
            var sectionNodes = new List<HtmlNode>();
            if (hit != null)
            {
                for (var n = hit.NextSibling; n != null; n = n.NextSibling)
                {
                    if (n.NodeType == HtmlNodeType.Element)
                    {
                        var nm = n.Name.ToLowerInvariant();
                        var isHeading = nm is "h1" or "h2" or "h3" or "h4" ||
                                        n.GetAttributeValue("role", "") == "heading";
                        if (isHeading) break;

                        if (nm is "p" or "div" or "section" or "ul" or "ol" or "table")
                            sectionNodes.Add(n);
                    }
                }
            }

            // Fallback: slice from the page text if DOM heading wasn’t found
            string fullText = WT(doc.DocumentNode.InnerText ?? "");
            string sectionTextFallback = "";
            if (sectionNodes.Count == 0)
            {
                var m = System.Text.RegularExpressions.Regex.Match(
                    fullText,
                    @"(Respirator\s+recommendations|Respiratory\s+protection|Respirator)\s*:?\s*(?<rest>.+)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success) sectionTextFallback = m.Groups["rest"].Value;
            }

            // Helpers
            static bool IsRespiratorLike(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return false;
                var t = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim();
                if (t.Length < 3) return false;
                if (System.Text.RegularExpressions.Regex.IsMatch(t,
                    @"\b(APF|SCBA|self[-\s]?contained|supplied[-\s]?air|SAR|air[-\s]?line|positive[-\s]?pressure|full[-\s]?facepiece|half[-\s]?facepiece|P[0-9]{2}|cartridge|canister|air[-\s]?purifying|gas mask|escape)\b",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return true;
                if (t.StartsWith("Any ", StringComparison.OrdinalIgnoreCase)) return true;
                if (t.StartsWith("Use ", StringComparison.OrdinalIgnoreCase)) return true;
                if (t.StartsWith("Escape", StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }

            static IEnumerable<string> SplitToCandidateLines(string text)
            {
                if (string.IsNullOrWhiteSpace(text)) yield break;

                // Normalize bullets and common separators into lines
                var raw = text
                    .Replace("•", "\n")
                    .Replace("·", "\n")
                    .Replace(" - ", "\n- ")
                    .Replace("; ", "\n")
                    .Replace(". ", "\n")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim());

                foreach (var r in raw)
                {
                    var t = System.Text.RegularExpressions.Regex.Replace(r, @"\s+", " ").Trim();
                    if (t.Length > 2) yield return t;
                }
            }

            static bool IsRangeHeader(string s) =>
                System.Text.RegularExpressions.Regex.IsMatch(s ?? "", @"^\s*At\s+concentrations\s+.*?:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            static bool IsEscapeHeader(string s) =>
                System.Text.RegularExpressions.Regex.IsMatch(s ?? "", @"^\s*Escape\s*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var above = new List<string>();
            var escape = new List<string>();

            // Pass 1: DOM parse (tables, lists, paragraphs)
            if (sectionNodes.Count > 0)
            {
                bool inEscape = false;

                // 1) Tables (common in NPG)
                foreach (var node in sectionNodes)
                {
                    if (!node.Name.Equals("table", StringComparison.OrdinalIgnoreCase)) continue;

                    foreach (var tr in node.SelectNodes(".//tr[td]") ?? new HtmlNodeCollection(null))
                    {
                        var cells = tr.SelectNodes("./td|./th") ?? new HtmlNodeCollection(null);
                        if (cells.Count == 0) continue;

                        // Join all cells to preserve headers + APF + recommendations
                        var rowText = string.Join(" — ",
                            cells.Select(c => WT(c.InnerText)).Where(s => !string.IsNullOrWhiteSpace(s)));

                        if (string.IsNullOrWhiteSpace(rowText)) continue;

                        if (IsEscapeHeader(rowText))
                        {
                            inEscape = true;
                            var after = System.Text.RegularExpressions.Regex.Replace(rowText, @"^\s*Escape\s*:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            foreach (var line in SplitToCandidateLines(after))
                                if (IsRespiratorLike(line)) escape.Add(line);
                            continue;
                        }

                        if (IsRangeHeader(rowText))
                        {
                            inEscape = false;
                            var after = System.Text.RegularExpressions.Regex.Replace(rowText, @"^\s*At\s+concentrations\s+.*?:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            foreach (var line in SplitToCandidateLines(after))
                                if (IsRespiratorLike(line)) above.Add(line);
                            continue;
                        }

                        foreach (var line in SplitToCandidateLines(rowText))
                        {
                            if (!IsRespiratorLike(line)) continue;
                            if (inEscape) escape.Add(line); else above.Add(line);
                        }
                    }
                }

                // 2) Lists
                foreach (var node in sectionNodes)
                {
                    if (!(node.Name.Equals("ul", StringComparison.OrdinalIgnoreCase) ||
                          node.Name.Equals("ol", StringComparison.OrdinalIgnoreCase))) continue;

                    foreach (var li in node.SelectNodes(".//li") ?? new HtmlNodeCollection(null))
                    {
                        var t = WT(li.InnerText);
                        if (string.IsNullOrWhiteSpace(t)) continue;

                        if (IsEscapeHeader(t))
                        {
                            var after = System.Text.RegularExpressions.Regex.Replace(t, @"^\s*Escape\s*:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            foreach (var line in SplitToCandidateLines(after))
                                if (IsRespiratorLike(line)) escape.Add(line);
                            continue;
                        }
                        if (IsRangeHeader(t))
                        {
                            var after = System.Text.RegularExpressions.Regex.Replace(t, @"^\s*At\s+concentrations\s+.*?:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            foreach (var line in SplitToCandidateLines(after))
                                if (IsRespiratorLike(line)) above.Add(line);
                            continue;
                        }

                        foreach (var line in SplitToCandidateLines(t))
                            if (IsRespiratorLike(line)) above.Add(line);
                    }
                }

                // 3) Paragraphs/Divs
                foreach (var node in sectionNodes)
                {
                    if (!(node.Name.Equals("p", StringComparison.OrdinalIgnoreCase) ||
                          node.Name.Equals("div", StringComparison.OrdinalIgnoreCase) ||
                          node.Name.Equals("section", StringComparison.OrdinalIgnoreCase))) continue;

                    var t = WT(node.InnerText);
                    if (string.IsNullOrWhiteSpace(t)) continue;

                    // Keep headers inline (Escape: / At concentrations up to:)
                    var lines = SplitToCandidateLines(t).ToList();
                    bool inEscapeHere = false;

                    foreach (var line in lines)
                    {
                        if (IsEscapeHeader(line))
                        {
                            inEscapeHere = true;
                            var after = System.Text.RegularExpressions.Regex.Replace(line, @"^\s*Escape\s*:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            if (IsRespiratorLike(after)) escape.Add(after);
                            continue;
                        }
                        if (IsRangeHeader(line))
                        {
                            inEscapeHere = false;
                            var after = System.Text.RegularExpressions.Regex.Replace(line, @"^\s*At\s+concentrations\s+.*?:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            if (IsRespiratorLike(after)) above.Add(after);
                            continue;
                        }

                        if (IsRespiratorLike(line))
                        {
                            if (inEscapeHere) escape.Add(line); else above.Add(line);
                        }
                    }
                }
            }

            // Pass 2: tolerant text-slice fallback (if we missed stuff)
            if (above.Count == 0 || escape.Count == 0)
            {
                if (!string.IsNullOrWhiteSpace(sectionTextFallback))
                {
                    var mEsc = System.Text.RegularExpressions.Regex.Match(sectionTextFallback, @"\bEscape\s*:\s*(?<esc>.+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (mEsc.Success)
                    {
                        var before = sectionTextFallback.Substring(0, mEsc.Index);
                        var escTxt = mEsc.Groups["esc"].Value;

                        foreach (var line in SplitToCandidateLines(before))
                            if (IsRespiratorLike(line)) above.Add(line);

                        foreach (var line in SplitToCandidateLines(escTxt))
                            if (IsRespiratorLike(line)) escape.Add(line);
                    }
                    else
                    {
                        foreach (var line in SplitToCandidateLines(sectionTextFallback))
                        {
                            if (!IsRespiratorLike(line)) continue;
                            if (System.Text.RegularExpressions.Regex.IsMatch(line, @"\bEscape\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                escape.Add(line);
                            else
                                above.Add(line);
                        }
                    }
                }
            }

            // Cleanup & de-dup
            static IEnumerable<string> CleanOut(IEnumerable<string> src) =>
                (src ?? Array.Empty<string>())
                    .Select(s => System.Text.RegularExpressions.Regex.Replace(s ?? "", @"\s+", " ").Trim())
                    .Where(s => s.Length > 2)
                    .Distinct(StringComparer.OrdinalIgnoreCase);

            var aboveOut = CleanOut(above).ToArray();
            var escapeOut = CleanOut(escape).ToArray();

            return (aboveOut, escapeOut);
        }



        private static void EnsurePersonalRows(Dictionary<string, string> dict, IEnumerable<string> personalLines)
        {
            if (dict == null) return;
            var lines = (personalLines ?? Array.Empty<string>())
                .Select(s => (s ?? "").Trim())
                .Where(s => s.Length > 0)
                .ToList();

            if (lines.Count == 0) return;

            // Only allow these keys/categories
            static string CanonKey(string k)
            {
                k = System.Text.RegularExpressions.Regex.Replace(k ?? "", @"\s+", " ").Trim();
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)wash\s*skin", "Wash skin");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)face[-\s]*shield", "Face shield");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)eyes?", "Eyes");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)\bskin\b", "Skin");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)gloves?", "Gloves");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)remove", "Remove");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)change", "Change");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)provide", "Provide");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)protective\s*clothing", "Protective clothing");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)eye\s*protection", "Eye protection");
                k = System.Text.RegularExpressions.Regex.Replace(k, @"(?i)contact\s*lenses", "Contact lenses");
                return k;
            }

            var allowedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Eyes","Skin","Gloves","Face shield","Wash skin","Remove","Change",
        "Provide","Protective clothing","Eye protection","Contact lenses"
    };

            static bool LooksLikeNonPpe(string s)
                => System.Text.RegularExpressions.Regex.IsMatch(s ?? "", @"\b(First Aid|Symptoms|Target Organs|Carcinogen|TLV|REL|PEL|NIOSH|OSHA|ACGIH|Respirator|Escape|APF|Exposure Limit|Concentration|Recommendations)\b",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            static IEnumerable<(string key, string val)> SplitKeyVals(IEnumerable<string> src,
                Func<string, string> canonKey, HashSet<string> allowed)
            {
                foreach (var raw in src)
                {
                    var s = System.Text.RegularExpressions.Regex.Replace(raw ?? "", @"\s+", " ").Trim().Trim('•');

                    var m = System.Text.RegularExpressions.Regex.Match(s, @"^(?<k>[A-Za-z][A-Za-z \-/]{2,40})\s*:\s*(?<v>.+)$");
                    if (m.Success)
                    {
                        var key = canonKey(m.Groups["k"].Value);
                        var val = m.Groups["v"].Value.Trim();
                        if (allowed.Contains(key) && val.Length > 0 && !LooksLikeNonPpe(val))
                            yield return (key, val);
                        continue;
                    }

                    // Infer category if no explicit "Key:"
                    if (LooksLikeNonPpe(s)) continue;

                    string? inferred = null;
                    if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\beye|goggle|contact lens|lacrimation\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        inferred = "Eyes";
                    else if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bskin|dermal|wash skin|remove clothing|impervious\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        inferred = "Skin";
                    else if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bglove", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        inferred = "Gloves";
                    else if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bface[-\s]?shield\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        inferred = "Face shield";
                    else if (s.StartsWith("Remove ", StringComparison.OrdinalIgnoreCase))
                        inferred = "Remove";
                    else if (s.StartsWith("Change ", StringComparison.OrdinalIgnoreCase))
                        inferred = "Change";
                    else if (s.StartsWith("Provide ", StringComparison.OrdinalIgnoreCase))
                        inferred = "Provide";

                    if (!string.IsNullOrWhiteSpace(inferred) && allowed.Contains(inferred!))
                        yield return (inferred!, s);
                }
            }

            var byKey = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var (k, v) in SplitKeyVals(lines, CanonKey, allowedKeys))
            {
                if (!byKey.TryGetValue(k, out var list))
                    byKey[k] = list = new List<string>();
                if (!list.Any(x => x.Equals(v, StringComparison.OrdinalIgnoreCase)))
                    list.Add(v);
            }
            if (byKey.Count == 0) return;

            string[] order =
            {
        "Eyes","Skin","Gloves","Face shield","Wash skin",
        "Remove","Change","Provide","Protective clothing","Eye protection","Contact lenses","Notes"
    };

            var ordered = byKey
                .OrderBy(kv =>
                {
                    var idx = Array.FindIndex(order, o => o.Equals(kv.Key, StringComparison.OrdinalIgnoreCase));
                    return idx < 0 ? int.MaxValue : idx;
                })
                .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Emit per-key rows (so callers can inspect per-field if they want)
            const string PERSONAL_AGG_KEY = "PPE (NPG): Personal protection & sanitation";
            foreach (var (k, valsList) in ordered)
            {
                var bullets = string.Join(Environment.NewLine, valsList.Select(v => "• " + v));
                dict[$"{PERSONAL_AGG_KEY} — {k}"] = bullets;
                dict[$"Personal protection & sanitation — {k}"] = bullets;
            }

            // Build combined/summary (best line per key)
            static int Score(string v) =>
                (System.Text.RegularExpressions.Regex.IsMatch(v ?? "", @"\b(prevent|avoid|use|wear|do not|provide|impervious|goggle|glove|shield)\b",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase) ? 1000 : 0) + Math.Min(500, v?.Length ?? 0);

            var sectionBullets = ordered
                .Select(kv =>
                {
                    var best = kv.Value.OrderByDescending(Score).FirstOrDefault();
                    return string.IsNullOrWhiteSpace(best) ? null : $"• {kv.Key}: {best}";
                })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (sectionBullets.Count > 0)
            {
                var combined = string.Join(Environment.NewLine, sectionBullets);
                dict[PERSONAL_AGG_KEY] = combined;
                dict["Personal protection & sanitation"] = combined;
            }
        }


        public async Task<Dictionary<string, string>> GetPpeFlatFromNpgByCasOrNamesAsync(string cas, IEnumerable<string>? nameCandidates, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var outDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1) Find the best NPG URL: CAS first, then by names
            string? npgUrl = null;

            try
            {
                npgUrl = await FindNpgUrlByCasAsync(cas, ct);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] FindNpgUrlByCasAsync error: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(npgUrl) && nameCandidates != null)
            {
                foreach (var n in nameCandidates.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    try
                    {
                        var byName = await FindNpgUrlByNameAsync(n, ct);
                        if (!string.IsNullOrWhiteSpace(byName)) { npgUrl = byName; break; }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] FindNpgUrlByNameAsync('{n}') error: {ex.Message}");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(npgUrl))
            {
                unavailable?.Add("NIOSH NPG: no page found (flat mode)");
                return outDict;
            }

            // 2) Fetch HTML
            var html = await GetHtmlOrNullAsync(npgUrl!, ct);
            if (string.IsNullOrWhiteSpace(html))
            {
                unavailable?.Add("NIOSH NPG: PPE page fetch failed/blocked (flat mode)");
                return outDict;
            }

            // 3) Use existing tolerant extractor to get structured sections first
            var sections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                ExtractNpgPpeFromPageHtml(html, sections);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] ExtractNpgPpeFromPageHtml error: {ex.Message}");
            }

            // 4) If sections are empty or obviously too small, do extra tolerant harvesting
            var extras = new List<string>();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // a) Personal-ish broad lines
                var personalBroad = ExtractBroadPersonalCandidates(doc);
                if (personalBroad != null && personalBroad.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] personal broad candidates={personalBroad.Length}");
                    extras.AddRange(personalBroad);
                }

                // b) Respirator-ish lines from DOM
                var respDom = HarvestRespiratorCandidatesFromDom(doc);
                var respList = respDom?.ToList() ?? new List<string>();
                if (respList.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] respirator DOM candidates={respList.Count}");
                    extras.AddRange(respList);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] tolerant DOM harvest error: {ex.Message}");
            }

            // 5) Flatten everything to ONE block (generic: works for any NPG page)
            static IEnumerable<string> SplitBullets(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) yield break;
                var raw = s.Replace("\r", "")
                           .Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in raw)
                {
                    var parts = line.Split('•', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in parts)
                    {
                        var t = System.Text.RegularExpressions.Regex.Replace(p, @"\s+", " ").Trim();
                        if (!string.IsNullOrWhiteSpace(t)) yield return t;
                    }
                }
            }

            var allLines = new List<string>();

            // pull section values first (usually already bulletized)
            foreach (var v in sections.Values)
                allLines.AddRange(SplitBullets(v));

            // add extra tolerant lines we harvested
            if (extras.Count > 0)
            {
                foreach (var t in extras)
                {
                    var cleaned = System.Text.RegularExpressions.Regex.Replace(t ?? "", @"\s+", " ").Trim();
                    if (cleaned.Length > 0) allLines.Add(cleaned);
                }
            }

            // de-dup and sort a little for stability
            var flat = allLines
                .Select(s => System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim())
                .Where(s => s.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"[PPE:FLAT] total merged lines={flat.Count}");

            if (flat.Count == 0)
            {
                unavailable?.Add("NIOSH NPG PPE: no recognizable PPE text parsed (flat mode)");
                return outDict;
            }

            // 6) Emit as ONE big property value with bullets (works with your UI)
            var bulletBlock = string.Join(Environment.NewLine, flat.Select(s => "• " + s));
            outDict["PPE (NPG): Flat"] = bulletBlock;

            return outDict;
        }


        /// ///////////////////////////////////////////////////////////////////////////////////////////
        /// <param name="dict"></param>
        // 29 items
        // ------------------------ POST-PROCESS PPE FOR UI (SAFE/CONSERVATIVE) ------------------------
        private static void PostProcessPpeForUi(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0) return;

            // ---------------- PERSONAL (unchanged from your approach) ----------------
            System.Diagnostics.Debug.WriteLine("[PPE:UI] Normalize + Collapse personal");

            static string Clean(string s) => Regex.Replace(s ?? "", @"\s+", " ").Trim();

            static IEnumerable<string> SplitBullets(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) yield break;
                var raw = s.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
                var more = new List<string>();
                foreach (var line in raw)
                {
                    var parts = line.Split('•', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in parts) more.Add(p);
                }
                foreach (var p in more)
                {
                    var t = Clean(p.TrimStart('•'));
                    if (t.Length > 0) yield return t;
                }
            }

            // Only look at per-key personal rows, then we’ll emit ONE combined canonical row.
            var personalPerKeyPrefixes = new[]
            {
        "PPE (NPG): Personal protection & sanitation — ",
        "Personal protection & sanitation — "
    };

            var perKey = dict.Keys
                .Where(k => personalPerKeyPrefixes.Any(p => k.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // legacy notes rows
            perKey.AddRange(dict.Keys.Where(k =>
                k.Equals("PPE (NPG): Personal protection & sanitation — Notes", StringComparison.OrdinalIgnoreCase) ||
                k.Equals("Personal protection & sanitation — Notes", StringComparison.OrdinalIgnoreCase)));

            perKey = perKey.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var bullets = new List<string>();
            foreach (var k in perKey)
            {
                var val = dict[k] ?? "";
                var keySuffix = k;
                var ix = Math.Max(keySuffix.LastIndexOf('—'), keySuffix.LastIndexOf('-'));
                if (ix >= 0 && ix + 1 < keySuffix.Length) keySuffix = keySuffix[(ix + 1)..];
                keySuffix = Clean(keySuffix);

                foreach (var line in SplitBullets(val))
                {
                    var text = line;

                    // Shape into "Key: value" if it isn’t already
                    if (!Regex.IsMatch(text, @"^[A-Za-z].{0,40}:\s"))
                        text = $"{keySuffix}: {text}";

                    // Filter obvious non-PPE bleed (keep filter minimal to avoid dropping legit items)
                    var isNonPpe =
                        Regex.IsMatch(text, @"\b(First Aid|Symptoms|Target Organs|Carcinogen|TLV|REL|PEL|NIOSH|OSHA|ACGIH|Respirator|Escape|APF|Exposure Limit|Concentration|Recommendations)\b",
                                        RegexOptions.IgnoreCase);
                    if (isNonPpe) continue;

                    bullets.Add("• " + text);
                }
            }

            // If no per-key rows were present, try to salvage from any combined rows we may have
            if (bullets.Count == 0)
            {
                foreach (var k in dict.Keys.Where(x =>
                    x.Equals("PPE (NPG): Personal protection & sanitation", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("Personal protection & sanitation", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    foreach (var line in SplitBullets(dict[k] ?? ""))
                    {
                        var t = Clean(line);
                        var isNonPpe =
                            Regex.IsMatch(t, @"\b(First Aid|Symptoms|Target Organs|Carcinogen|TLV|REL|PEL|NIOSH|OSHA|ACGIH|Respirator|Escape|APF|Exposure Limit|Concentration|Recommendations)\b",
                                            RegexOptions.IgnoreCase);
                        if (isNonPpe) continue;

                        var shaped = Regex.IsMatch(t, @"^[A-Za-z].{0,40}:\s") ? t : $"Notes: {t}";
                        var b = "• " + shaped;
                        if (!bullets.Any(bi => bi.Equals(b, StringComparison.OrdinalIgnoreCase)))
                            bullets.Add(b);
                    }
                }
            }

            // Dedup and write ONE canonical key ONLY (what your UI expects)
            var seenPersonal = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bullets = bullets.Where(b => seenPersonal.Add(Clean(b))).ToList();

            const string PERSONAL_CANON = "PPE (NPG): Personal protection & sanitation";
            dict.Remove("Personal protection & sanitation");

            if (bullets.Count > 0)
            {
                dict[PERSONAL_CANON] = string.Join(Environment.NewLine, bullets);
            }
            else
            {
                dict.Remove(PERSONAL_CANON);
            }

            // prune per-key & any other combined variants
            foreach (var k in perKey) dict.Remove(k);
            foreach (var k in dict.Keys.Where(k =>
                         k.Equals("Personal protection & sanitation", StringComparison.OrdinalIgnoreCase))
                         .ToList())
                dict.Remove(k);

            // ---------------- RESPIRATORS (improved & logged) ----------------
            static bool IsRespKey(string k) =>
                !string.IsNullOrWhiteSpace(k) &&
                k.IndexOf("Respirator", StringComparison.OrdinalIgnoreCase) >= 0;

            var respKeys = dict.Keys.Where(IsRespKey).ToList();
            foreach (var key in respKeys)
            {
                var raw = dict[key] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(raw)) continue;

                // 1) Tokenize: newlines, our "||" joiner, and " | " artifacts
                var tokens = raw.Replace("\r", "")
                                .Replace(" || ", "\n")
                                .Replace(" | ", "\n")
                                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .ToList();

                System.Diagnostics.Debug.WriteLine($"[PPE:UI] [{key}] tokens(before)={tokens.Count}");

                // 2) Trim page chrome early (keeps real respirator text intact)
                string TrimChrome(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return "";
                    var cutCues = new[]
                    {
                "Important additional information about respirator selection",
                "See also INTRODUCTION",
                "ICSC CARD:",
                "MEDICAL TESTS:",
                "Page last reviewed:",
                "Content source:",
                "NIOSH Homepage",
                "Workplace Safety & Health Topics",
                "Publications and Products",
                "Programs",
                "Follow NIOSH",
                "Facebook","Pinterest","Twitter","YouTube",
                "Related Pages",
                "Synonyms & Trade Names",
                "Personal Protection/Sanitation",
                "First Aid (See procedures)",
                "Chemical Names, Synonyms and Trade Names",
                "CAS Numbers","RTECS Numbers","Appendices expand",
                "Introduction","Search the Pocket Guide",
                "Target Organs","Cancer Site"
            };
                    foreach (var cue in cutCues)
                    {
                        var idx = s.IndexOf(cue, StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0) { s = s[..idx]; break; }
                    }
                    s = Regex.Replace(s, @"\s+", " ").Trim().TrimEnd('.', ';', ':', '·', '•');
                    return s;
                }
                for (int i = 0; i < tokens.Count; i++) tokens[i] = TrimChrome(tokens[i]);

                // 3) Keep respirator-like lines
                bool LooksLikeResp(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return false;
                    var t = Regex.Replace(s, @"\s+", " ").Trim();

                    // fast drops
                    if (t.StartsWith("[", StringComparison.Ordinal)) return false; // e.g., "[potential occupational carcinogen]"
                    if (t.Length < 4 && !t.StartsWith("(APF", StringComparison.OrdinalIgnoreCase)) return false;

                    // strong keeps
                    if (t.StartsWith("Escape", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("At concentrations", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("(APF", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("Any self-contained breathing apparatus", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("Any supplied-air respirator", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("Any air-purifying", StringComparison.OrdinalIgnoreCase)) return true;
                    if (t.StartsWith("Use ", StringComparison.OrdinalIgnoreCase)) return true;

                    // keyword keeps
                    return Regex.IsMatch(t,
                        @"\b(APF|SCBA|self[-\s]?contained|supplied[-\s]?air|SAR|air[-\s]?line|positive[-\s]?pressure|gas mask|full[-\s]?facepiece|half[-\s]?facepiece|P[0-9]{2}|cartridge|canister|air[-\s]?purifying|escape|respirator)\b",
                        RegexOptions.IgnoreCase);
                }

                var filtered = tokens.Where(LooksLikeResp)
                                     .Select(s => Regex.Replace(s, @"\s+", " ").Trim())
                                     .Where(s => s.Length > 0)
                                     .ToList();

                System.Diagnostics.Debug.WriteLine($"[PPE:UI] [{key}] tokens(resp-like)={filtered.Count}");

                // 4) APF coalescer: merge a standalone "(APF = N)" with the following respirator line
                bool IsStandaloneApf(string s) =>
                    Regex.IsMatch(s, @"^\(?\s*APF\s*=\s*[\d,]+\s*\)?$", RegexOptions.IgnoreCase);

                var merged = new List<string>();
                for (int i = 0; i < filtered.Count; i++)
                {
                    var cur = filtered[i];
                    if (IsStandaloneApf(cur) && i + 1 < filtered.Count)
                    {
                        var next = filtered[i + 1];
                        if (!IsStandaloneApf(next))
                        {
                            merged.Add($"{cur} — {next}");
                            i++; // consume the next item
                            continue;
                        }
                    }
                    merged.Add(cur);
                }

                System.Diagnostics.Debug.WriteLine($"[PPE:UI] [{key}] tokens(after APF merge)={merged.Count}");

                // 5) Bucket into ABOVE vs ESCAPE
                bool IsEscapeHeader(string s) => Regex.IsMatch(s, @"^\s*Escape\s*:?", RegexOptions.IgnoreCase);
                bool MentionsEscape(string s) =>
                    IsEscapeHeader(s) ||
                    s.IndexOf("escape-type", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.IndexOf("for escape", StringComparison.OrdinalIgnoreCase) >= 0;

                var aboveList = new List<string>();
                var escapeList = new List<string>();

                bool keyLooksEscape = key.IndexOf("escape", StringComparison.OrdinalIgnoreCase) >= 0;
                bool keyLooksAbove = key.IndexOf("above rel", StringComparison.OrdinalIgnoreCase) >= 0
                                  || key.IndexOf("detectable", StringComparison.OrdinalIgnoreCase) >= 0;

                if (keyLooksEscape)
                {
                    escapeList.AddRange(merged);
                }
                else if (keyLooksAbove)
                {
                    aboveList.AddRange(merged);
                }
                else
                {
                    foreach (var line in merged)
                        if (MentionsEscape(line)) escapeList.Add(line);
                        else aboveList.Add(line);
                }

                // 6) Final tidy: de-dup & bulletize; move the "Note:" (if any) to the end
                static IEnumerable<string> DedupDisplay(IEnumerable<string> src)
                {
                    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var s in src.Select(x => Regex.Replace(x ?? "", @"\s+", " ").Trim())
                                         .Where(x => x.Length > 0))
                    {
                        if (seen.Add(s)) yield return s.StartsWith("•") ? s : "• " + s;
                    }
                }

                aboveList = DedupDisplay(aboveList).ToList();
                escapeList = DedupDisplay(escapeList).ToList();

                // Ensure any standard "Note:" line (if present) is last
                void MoveNoteToEnd(List<string> list)
                {
                    var idx = list.FindIndex(l => l.StartsWith("• Note: NIOSH recommendations are guidance", StringComparison.OrdinalIgnoreCase));
                    if (idx > -1 && idx != list.Count - 1)
                    {
                        var note = list[idx];
                        list.RemoveAt(idx);
                        list.Add(note);
                    }
                }
                MoveNoteToEnd(aboveList);
                MoveNoteToEnd(escapeList);

                System.Diagnostics.Debug.WriteLine($"[PPE:UI] [{key}] aboveOut={aboveList.Count} escapeOut={escapeList.Count}");

                // 7) Write back under canonical keys; normalize the "above REL/detectable" key
                const string CANON_ABOVE = "PPE (NPG): Respirator — above REL/detectable";
                const string SPACED_ABOVE = "PPE (NPG): Respirator — above REL / detectable";

                if (aboveList.Count > 0)
                {
                    dict[CANON_ABOVE] = string.Join(Environment.NewLine, aboveList);
                    dict.Remove(SPACED_ABOVE);
                }
                if (escapeList.Count > 0)
                {
                    dict["PPE (NPG): Respirator — escape"] = string.Join(Environment.NewLine, escapeList);
                }
            }
        }

        // ========= HAZARD CLEANUP HELPERS =========
        private static string? NormalizeHazardCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            return Regex.Replace(code.Trim().ToUpperInvariant(), @"\s+", "");
        }





        // NEW: tolerant CAS match for NMAM cells that list multiple CAS values
        private static bool CasCellHasCas(string? casCellRaw, string targetCas)
        {
            if (string.IsNullOrWhiteSpace(casCellRaw) || string.IsNullOrWhiteSpace(targetCas)) return false;

            // Normalize weird hyphens and spaces
            static string Norm(string s) =>
                Regex.Replace(
                    Regex.Replace(s ?? "", @"[\u00A0\s]+", ""),
                    "[\u2010-\u2015\u2212\uFE63\uFF0D]", "-"
                );

            // Digits-only compare helper (handles odd hyphenation)
            static string Digits(string s) => new string((s ?? "").Where(char.IsDigit).ToArray());

            var cell = Norm(casCellRaw);
            var want = Norm(targetCas);

            if (cell.Equals(want, StringComparison.OrdinalIgnoreCase)) return true;
            if (Digits(cell).Equals(Digits(want), StringComparison.Ordinal)) return true;

            // NMAM tables often separate multiple CAS with commas/semicolons/“or/and”
            var tokens = Regex.Split(cell, @"(?:,|;|/|\bor\b|\band\b)", RegexOptions.IgnoreCase)
                              .Select(t => t.Trim())
                              .Where(t => t.Length > 0);

            foreach (var t in tokens)
            {
                if (t.Equals(want, StringComparison.OrdinalIgnoreCase)) return true;
                if (Digits(t).Equals(Digits(want), StringComparison.Ordinal)) return true;
            }

            return false;
        }

        // One comparer to use everywhere we key by (string, string) tuples
        private sealed class TupleIgnoreCaseComparer : IEqualityComparer<(string, string)>
        {
            public bool Equals((string, string) x, (string, string) y) =>
                StringComparer.OrdinalIgnoreCase.Equals(x.Item1, y.Item1) &&
                StringComparer.OrdinalIgnoreCase.Equals(x.Item2, y.Item2);

            public int GetHashCode((string, string) obj) =>
                HashCode.Combine(
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1 ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item2 ?? string.Empty)
                );
        }

        // NIOSH NMAM: scan letter pages (A–Z) and return methods when any row contains the CAS
        private async Task<List<IhChemicalSamplingMethod>> GetNmamByCasAsync(string cas, CancellationToken ct)
        {
            var results = new List<IhChemicalSamplingMethod>();
            if (string.IsNullOrWhiteSpace(cas)) { Debug.WriteLine("[NMAM-CAS] empty cas"); return results; }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Debug.WriteLine($"[NMAM-CAS][BEGIN] cas='{cas}'");

            try
            {
                for (char letter = 'a'; letter <= 'z'; letter++)
                {
                    var pageUrl = $"https://www.cdc.gov/niosh/nmam/method-{letter}.html";
                    var html = await GetHtmlOrNullAsync(pageUrl, ct);
                    if (string.IsNullOrWhiteSpace(html)) { Debug.WriteLine($"[NMAM-CAS][{letter}] no html"); continue; }

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var rows = doc.DocumentNode.SelectNodes("//table//tr[td]")
                               ?? Enumerable.Empty<HtmlAgilityPack.HtmlNode>();

                    int matchedRows = 0;
                    foreach (var tr in rows)
                    {
                        var tds = tr.SelectNodes("./td|./th") ?? Enumerable.Empty<HtmlAgilityPack.HtmlNode>();
                        var rowText = HtmlEntity.DeEntitize(string.Join(" | ", tds.Select(td => td.InnerText ?? ""))).Trim();
                        if (!CasCellHasCas(rowText, cas)) continue;
                        matchedRows++;

                        var link = tr.SelectSingleNode(".//a");
                        if (link == null) continue;

                        var anchorText = HtmlEntity.DeEntitize(link.InnerText ?? "").Trim();
                        var m = Regex.Match(anchorText, @"\b(\d{3,4}[A-Za-z]?)\b");
                        if (!m.Success) continue;

                        var methodNum = m.Groups[1].Value.ToUpperInvariant();
                        var code = $"NIOSH {methodNum}";
                        var hrefAbs = ResolveHref(link.GetAttributeValue("href", string.Empty), pageUrl);

                        if (seen.Add(code))
                        {
                            results.Add(new IhChemicalSamplingMethod { Source = "NIOSH", MethodId = code, Url = hrefAbs });
                            Debug.WriteLine($"[NMAM-CAS][{letter}] +{code} url='{hrefAbs}'");
                        }
                    }
                    Debug.WriteLine($"[NMAM-CAS][{letter}] matchedRows={matchedRows}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NMAM-CAS][ERR] {ex.Message}");
            }

            Debug.WriteLine($"[NMAM-CAS][END] cas='{cas}', found={results.Count}");
            return results
                .GroupBy(r => (r.Source ?? "", r.MethodId ?? "", r.Url ?? ""))
                .Select(g => g.First())
                .OrderBy(r => r.Source).ThenBy(r => r.MethodId)
                .ToList();
        }


        private static HashSet<string> ExtractCasNumbersFromDetail(HtmlAgilityPack.HtmlDocument doc)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            static string Digits(string s) => new string((s ?? "").Where(char.IsDigit).ToArray());

            // Look for obvious “CAS” fields (definition lists, tables)
            var textChunks = new List<string>();

            // Any dt/dd or th/td that mention CAS
            foreach (var node in doc.DocumentNode.SelectNodes("//*[self::dt or self::dd or self::th or self::td or self::p]")
                     ?? Enumerable.Empty<HtmlAgilityPack.HtmlNode>())
            {
                var t = HtmlEntity.DeEntitize(node.InnerText ?? "").Trim();
                if (t.IndexOf("CAS", StringComparison.OrdinalIgnoreCase) >= 0)
                    textChunks.Add(t);
            }

            // Whole-page regex safety net
            var full = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? "");
            textChunks.Add(full);

            var rx = new Regex(@"\b\d{2,7}-\d{2}-\d\b");
            foreach (var chunk in textChunks)
            {
                foreach (Match m in rx.Matches(chunk))
                {
                    var cas = m.Value;
                    if (set.Add(cas))
                        ; // captured
                }
            }

            // Normalize: keep both hyphenated and digit-only for tolerant compares
            var normed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in set)
            {
                normed.Add(s);
                normed.Add(Digits(s));
            }
            return normed;
        }

        private async Task<List<(string name, string url)>> EnumerateOshaSearchPageChemLinksAsync(int page, CancellationToken ct)
        {
            var list = new List<(string name, string url)>();
            var searchUrl = $"https://www.osha.gov/chemicaldata/search?page={page}";
            var html = await GetHtmlOrNullAsync(searchUrl, ct);
            if (string.IsNullOrWhiteSpace(html)) return list;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Rows with a link to /chemicaldata/<slug>
            foreach (var tr in doc.DocumentNode.SelectNodes("//table//tr[td]") ?? Enumerable.Empty<HtmlAgilityPack.HtmlNode>())
            {
                var link = tr.SelectSingleNode(".//a[contains(@href, '/chemicaldata/')]");
                var nameTd = tr.SelectSingleNode("./td[1]");
                if (link == null || nameTd == null) continue;

                var displayName = HtmlEntity.DeEntitize(nameTd.InnerText ?? "").Trim();
                var url = ResolveHref(link.GetAttributeValue("href", string.Empty), searchUrl);
                if (!string.IsNullOrWhiteSpace(url))
                    list.Add((displayName, url));
            }

            return list;
        }

        private static string NormalizeOshaPdfUrl(string? url, string methodCode)
        {
            // If we didn't get a concrete href, synthesize the standard OSHA PDF path
            if (string.IsNullOrWhiteSpace(url))
            {
                var slug = methodCode.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
                url = $"https://www.osha.gov/sites/default/files/methods/osha-{slug}.pdf";
            }

            // Force https and encode spaces
            url = url.Replace("http://", "https://", StringComparison.OrdinalIgnoreCase)
                     .Replace(" ", "%20");

            return url;
        }




    }
}
