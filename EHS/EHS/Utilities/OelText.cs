using System.Text.RegularExpressions;

namespace EHS.Utilities
{
    public static class OelText
    {
        public static string PrettyType(string? type, string? notes)
        {
            type = (type ?? "").Trim().ToUpperInvariant();
            var n = notes ?? "";

            var is8hr = Regex.IsMatch(n, @"\b(8\s*[- ]?hr|8\s*hour)\b", RegexOptions.IgnoreCase);
            var is15m = Regex.IsMatch(n, @"\b(15\s*[- ]?min|15\s*minute)\b", RegexOptions.IgnoreCase);

            return type switch
            {
                "TWA" => is8hr ? "TWA (8-hr)" : "TWA",
                "STEL" => is15m ? "STEL (15-min)" : "STEL",
                "CEIL" or "CEILING" or "C" => "Ceiling",
                "ACTION LEVEL" or "AL" => "Action Level",
                "" => "Limit",
                _ => type
            };
        }
    }
}
