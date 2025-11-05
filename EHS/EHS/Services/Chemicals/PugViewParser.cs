using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using EHS.Models.IH;

namespace EHS.Services.Chemicals
{
    public class PugViewParser
    {
        // -------- Existing names/synonyms extraction (unchanged) --------
        public (string? Cas, string? PreferredName, List<string> Synonyms) ExtractNames(JsonNode? namesRoot)
        {
            var synonyms = new List<string>();
            string? cas = null; string? pref = null;
            if (namesRoot is null) return (cas, pref, synonyms);

            foreach (var sec in FindSectionsByHeading(namesRoot, "Names and Identifiers"))
            {
                foreach (var sub in EnumerateAllSections(sec))
                {
                    var head = sub?["TOCHeading"]?.ToString();

                    if (string.Equals(head, "Record Title", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = ExtractFirstStringFromInformation(sub);
                        if (!string.IsNullOrWhiteSpace(val)) pref = val;
                    }
                    if (Regex.IsMatch(head ?? string.Empty, @"CAS", RegexOptions.IgnoreCase))
                    {
                        var val = ExtractFirstStringFromInformation(sub);
                        if (IsCas(val)) cas = val;
                    }
                    if (string.Equals(head, "Synonyms", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = ExtractStringListFromInformation(sub);
                        synonyms.AddRange(list);
                    }
                }
            }
            synonyms = synonyms.Select(s => s.Trim())
                               .Where(s => !string.IsNullOrEmpty(s))
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToList();
            return (cas, pref, synonyms);
        }

        // -------- Existing hazards extraction (kept, but made internal helper-friendly) --------
        public List<IhChemicalHazard> ExtractHazards(JsonNode? root)
        {
            var hazards = new List<IhChemicalHazard>();
            if (root is null) return hazards;

            foreach (var sec in FindSectionsByHeading(root, "GHS Classification"))
            {
                var items = ExtractStringListFromInformation(sec);
                foreach (var t in items)
                    hazards.Add(new IhChemicalHazard { Source = "GHS", Code = TryExtractHazardCode(t), Description = t });
            }
            foreach (var sec in FindSectionsByHeading(root, "Hazards Identification"))
            {
                var items = ExtractStringListFromInformation(sec);
                foreach (var t in items)
                {
                    if (t.Contains("NFPA", StringComparison.OrdinalIgnoreCase))
                        hazards.Add(new IhChemicalHazard { Source = "NFPA", Description = t });
                    else if (t.Contains("HMIS", StringComparison.OrdinalIgnoreCase))
                        hazards.Add(new IhChemicalHazard { Source = "HMIS", Description = t });
                }
            }
            foreach (var sec in FindSectionsByHeading(root, "Toxicity"))
            {
                var items = ExtractStringListFromInformation(sec);
                foreach (var t in items)
                    hazards.Add(new IhChemicalHazard { Source = "Toxicity", Description = t });
            }
            return hazards;
        }

        // -------- NEW: Derived IH flags from GHS hazard codes --------
        public Dictionary<string, string?> DeriveFlagsFromHazards(IEnumerable<IhChemicalHazard> hazards)
        {
            var codes = new HashSet<string>(hazards.Where(h => !string.IsNullOrWhiteSpace(h.Code))
                                                   .Select(h => h.Code!.Trim().ToUpperInvariant()));

            bool has(Func<string, bool> predicate) =>
                hazards.Any(h => (h.Description ?? "").IndexOf("carcin", StringComparison.OrdinalIgnoreCase) >= 0) && predicate("H350") // helper used below
                || codes.Any(predicate);

            bool carcinogen = codes.Contains("H350") || codes.Contains("H351")
                || hazards.Any(h => (h.Description ?? "").IndexOf("carcin", StringComparison.OrdinalIgnoreCase) >= 0);

            bool respSens = codes.Contains("H334") || hazards.Any(h => (h.Description ?? "").IndexOf("respiratory sensit", StringComparison.OrdinalIgnoreCase) >= 0);
            bool skinSens = codes.Contains("H317") || hazards.Any(h => (h.Description ?? "").IndexOf("skin sensit", StringComparison.OrdinalIgnoreCase) >= 0);

            bool reproTox = codes.Contains("H360") || codes.Contains("H361")
                || hazards.Any(h => (h.Description ?? "").IndexOf("reproductive", StringComparison.OrdinalIgnoreCase) >= 0);

            bool stotSingle = codes.Contains("H370") || codes.Contains("H371");
            bool stotRepeated = codes.Contains("H372") || codes.Contains("H373");

            // Broad “skin absorption potential” proxy: strong dermal hazard H310/H311/H312 or text mentions “skin absorption”
            bool skinAbsorption = codes.Contains("H310") || codes.Contains("H311") || codes.Contains("H312")
                                  || hazards.Any(h => (h.Description ?? "").IndexOf("skin absorption", StringComparison.OrdinalIgnoreCase) >= 0);

            return new Dictionary<string, string?>
            {
                ["Flag_Carcinogen"] = carcinogen ? "true" : "false",
                ["Flag_RespiratorySensitizer"] = respSens ? "true" : "false",
                ["Flag_SkinSensitizer"] = skinSens ? "true" : "false",
                ["Flag_ReproductiveToxicant"] = reproTox ? "true" : "false",
                ["Flag_STOT_SingleExposure"] = stotSingle ? "true" : "false",
                ["Flag_STOT_RepeatedExposure"] = stotRepeated ? "true" : "false",
                ["Flag_SkinAbsorptionPotential"] = skinAbsorption ? "true" : "false"
            };
        }

        // -------- NEW: Extended phys/flammability/appearance properties --------
        // Accept one or more PUG-View roots (e.g., Experimental Properties, Safety & Hazards)
        public Dictionary<string, string?> ExtractExtendedProps(params JsonNode?[] roots)
        {
            var buckets = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Boiling Point"] = new(),
                ["Melting Point"] = new(),
                ["Freezing Point"] = new(),
                ["Vapor Pressure"] = new(),
                ["Density"] = new(),
                ["Specific Gravity"] = new(),
                ["Water Solubility"] = new(),
                ["Flash Point"] = new(),
                ["Autoignition Temperature"] = new(),
                ["pKa"] = new(),
                ["Appearance"] = new(),
                ["LEL"] = new(),
                ["UEL"] = new(),
            };

            foreach (var root in roots.Where(r => r is not null))
            {
                foreach (var sec in EnumerateAllSections(root!))
                {
                    var heading = sec?["TOCHeading"]?.ToString() ?? string.Empty;
                    string? key = null;

                    if (heading.Contains("Boiling", StringComparison.OrdinalIgnoreCase)) key = "Boiling Point";
                    else if (heading.Contains("Melting", StringComparison.OrdinalIgnoreCase)) key = "Melting Point";
                    else if (heading.Contains("Freezing", StringComparison.OrdinalIgnoreCase)) key = "Freezing Point";
                    else if (heading.Contains("Vapor Pressure", StringComparison.OrdinalIgnoreCase)) key = "Vapor Pressure";
                    else if (heading.Contains("Density", StringComparison.OrdinalIgnoreCase)) key = "Density";
                    else if (heading.Contains("Specific Gravity", StringComparison.OrdinalIgnoreCase)) key = "Specific Gravity";
                    else if (heading.Contains("Water Solubility", StringComparison.OrdinalIgnoreCase)) key = "Water Solubility";
                    else if (heading.Contains("Solubility", StringComparison.OrdinalIgnoreCase) && !heading.Contains("Water", StringComparison.OrdinalIgnoreCase)) key = "Water Solubility"; // loose fallback
                    else if (heading.Contains("Flash Point", StringComparison.OrdinalIgnoreCase)) key = "Flash Point";
                    else if (heading.Contains("Auto-Ignition", StringComparison.OrdinalIgnoreCase) || heading.Contains("Autoignition", StringComparison.OrdinalIgnoreCase)) key = "Autoignition Temperature";
                    else if (heading.Contains("pKa", StringComparison.OrdinalIgnoreCase)) key = "pKa";
                    else if (heading.Contains("Appearance", StringComparison.OrdinalIgnoreCase) || heading.Contains("Physical Description", StringComparison.OrdinalIgnoreCase) || heading.Contains("Color", StringComparison.OrdinalIgnoreCase))
                        key = "Appearance";
                    else if (Regex.IsMatch(heading, @"\b(LEL|Lower (Explosive|Flammability) Limit)\b", RegexOptions.IgnoreCase)) key = "LEL";
                    else if (Regex.IsMatch(heading, @"\b(UEL|Upper (Explosive|Flammability) Limit)\b", RegexOptions.IgnoreCase)) key = "UEL";
                    else continue;

                    foreach (var val in ExtractValuesForProperty(sec))
                    {
                        var cleaned = CleanWhitespace(val);
                        if (!string.IsNullOrWhiteSpace(cleaned))
                            buckets[key].Add(cleaned);
                    }
                }
            }

            // Mirror melting → freezing if absent (common)
            if (buckets["Freezing Point"].Count == 0 && buckets["Melting Point"].Count > 0)
                buckets["Freezing Point"].AddRange(buckets["Melting Point"]);

            // Distinct+join
            return buckets.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Count == 0 ? null : string.Join(" | ", kvp.Value.Distinct(StringComparer.OrdinalIgnoreCase))
            );
        }

        // -------- Helpers (kept + extended) --------
        private static bool IsCas(string? s) => !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s.Trim(), @"^\d{2,7}-\d{2}-\d$");
        private static string? TryExtractHazardCode(string s) { var m = Regex.Match(s ?? "", @"\bH\d{3}\b", RegexOptions.IgnoreCase); return m.Success ? m.Value.ToUpperInvariant() : null; }
        private static string CleanWhitespace(string s) => Regex.Replace(s, @"\s+", " ").Trim();

        private static IEnumerable<JsonNode?> FindSectionsByHeading(JsonNode root, string heading)
        {
            foreach (var sec in EnumerateAllSections(root))
            {
                var h = sec?["TOCHeading"]?.ToString();
                if (string.Equals(h, heading, StringComparison.OrdinalIgnoreCase)) yield return sec;
            }
        }

        public static IEnumerable<JsonNode?> EnumerateAllSections(JsonNode root)
        {
            var start = root["Record"] ?? root;
            var stack = new Stack<JsonNode?>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (node is null) continue;
                if (node["Section"] is JsonArray arr)
                    foreach (var s in arr) stack.Push(s);
                yield return node;
            }
        }

        private static string? ExtractFirstStringFromInformation(JsonNode? sec)
        {
            if (sec?["Information"] is JsonArray info)
            {
                foreach (var i in info)
                {
                    var val = i?["Value"]?["StringWithMarkup"]?[0]?["String"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
            }
            return null;
        }

        public static List<string> ExtractStringListFromInformation(JsonNode? sec)
        {
            var list = new List<string>();
            if (sec?["Information"] is JsonArray info)
            {
                foreach (var i in info)
                {
                    if (i?["Value"]?["StringWithMarkup"] is JsonArray swm)
                        foreach (var s in swm)
                        {
                            var t = s?["String"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(t)) list.Add(t);
                        }
                    var s2 = i?["Value"]?["String"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(s2)) list.Add(s2);
                }
            }
            return list;
        }

        // Pull textual/number+unit/table values out of a property section
        public static IEnumerable<string> ExtractValuesForProperty(JsonNode? sec)
        {
            var list = new List<string>();
            if (sec?["Information"] is JsonArray info)
            {
                foreach (var i in info)
                {
                    var val = i?["Value"];
                    if (val is null) continue;

                    // Tables
                    if (val["Table"]?["Row"] is JsonArray rows)
                    {
                        foreach (var row in rows)
                        {
                            var cells = row?["Cell"]?.AsArray();
                            if (cells is null || cells.Count == 0) continue;
                            var text = string.Join(" ",
                                cells.Select(c =>
                                    c?["StringWithMarkup"]?[0]?["String"]?.ToString()
                                    ?? c?.ToString()
                                ).Where(s => !string.IsNullOrWhiteSpace(s)));
                            if (!string.IsNullOrWhiteSpace(text)) list.Add(text);
                        }
                    }

                    // Numbers + Unit (supports ranges)
                    var nums = val["Number"]?.AsArray();
                    var unit = val["Unit"]?.ToString();
                    if (nums is not null && nums.Count > 0)
                    {
                        string numStr = string.Join("–", nums.Select(n => n?.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)));
                        list.Add(string.IsNullOrWhiteSpace(unit) ? numStr : $"{numStr} {unit}");
                    }

                    // StringWithMarkup entries
                    if (val["StringWithMarkup"] is JsonArray swm)
                        foreach (var s in swm)
                        {
                            var text = s?["String"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(text)) list.Add(text);
                        }

                    // Plain String
                    var s2 = val["String"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(s2)) list.Add(s2);
                }
            }
            return list;
        }
    }
}