using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace EHS.Services.Chemicals
{
    public class PubChemClient
    {
        private readonly HttpClient _http;
        public PubChemClient(HttpClient http) { _http = http; }

        /// <summary>
        /// Resolve CID from CAS using Substance→CIDs (preferred) then Compound/name→CID (fallback).
        /// Populates 'unavailable' with human-readable messages if endpoints are down or blocked.
        /// </summary>
        public async Task<int?> ResolveCidByCasAsync(string cas, List<string>? unavailable = null, CancellationToken ct = default)
        {
            try
            {
                // 1) Substance → CIDs
                var sidUrl = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/substance/name/{Uri.EscapeDataString(cas)}/cids/JSON";
                using (var sidResp = await _http.GetAsync(sidUrl, ct))
                {
                    if (sidResp.IsSuccessStatusCode)
                    {
                        var json = await sidResp.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: ct);
                        var cids = json?["InformationList"]?["Information"]?[0]?["CID"]?.AsArray();
                        if (cids is not null && cids.Count > 0 && int.TryParse(cids[0]?.ToString(), out var cid1))
                            return cid1;
                    }
                    else
                    {
                        unavailable?.Add("PubChem: Substance→CID endpoint unavailable");
                    }
                }

                // 2) Fallback: Compound/name → CID
                var cmpUrl = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/{Uri.EscapeDataString(cas)}/cids/JSON";
                using (var cmpResp = await _http.GetAsync(cmpUrl, ct))
                {
                    if (cmpResp.IsSuccessStatusCode)
                    {
                        var json = await cmpResp.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: ct);
                        var cids = json?["IdentifierList"]?["CID"]?.AsArray();
                        if (cids is not null && cids.Count > 0 && int.TryParse(cids[0]?.ToString(), out var cid2))
                            return cid2;
                    }
                    else
                    {
                        unavailable?.Add("PubChem: Compound/name→CID endpoint unavailable");
                    }
                }
            }
            catch
            {
                unavailable?.Add("PubChem: CID resolution error");
            }

            // No CID found (might be a real “no mapping” case, not an outage)
            return null;
        }

        /// <summary>
        /// Get a compact set of properties for a CID from the Properties API.
        /// </summary>
        public async Task<Dictionary<string, string?>> GetCorePropertiesAsync(int cid, List<string>? unavailable = null, CancellationToken ct = default)
        {
            var dict = new Dictionary<string, string?>();
            try
            {
                var fields = string.Join(',', new[]
                {
                    "MolecularFormula","MolecularWeight","IUPACName",
                    "CanonicalSMILES","IsomericSMILES","InChI","InChIKey",
                    "XLogP","ExactMass"
                });

                var url = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/{cid}/property/{fields}/JSON";
                using var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    unavailable?.Add("PubChem: Properties API unavailable");
                    return dict;
                }

                var json = await resp.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: ct);
                var props = json?["PropertyTable"]?["Properties"]?[0] as JsonObject;
                if (props is null) return dict;

                foreach (var kv in props)
                {
                    if (kv.Key == "CID") continue;
                    dict[kv.Key] = kv.Value?.ToString();
                }
            }
            catch
            {
                unavailable?.Add("PubChem: Properties API error");
            }
            return dict;
        }

        /// <summary>
        /// Get PUG-View JSON (full tree or a specific heading). Adds messages to 'unavailable' if the call fails.
        /// </summary>
        public async Task<JsonNode?> GetPugViewAsync(int cid, string? heading, List<string>? unavailable = null, CancellationToken ct = default)
        {
            try
            {
                var baseUrl = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/{cid}/JSON";
                var url = heading is null ? baseUrl : $"{baseUrl}?heading={Uri.EscapeDataString(heading)}";
                using var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    var label = heading is null ? "PubChem PUG-View (full tree)" : $"PubChem PUG-View (\"{heading}\")";
                    unavailable?.Add($"{label} unavailable");
                    return null;
                }
                return await resp.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: ct);
            }
            catch
            {
                var label = heading is null ? "PubChem PUG-View" : $"PubChem PUG-View (\"{heading}\")";
                unavailable?.Add($"{label} error");
                return null;
            }
        }

        public async Task<string[]> GetSynonymsAsync(int cid, List<string>? unavailable = null, CancellationToken ct = default)
        {
            try
            {
                var url = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/{cid}/synonyms/JSON";
                using var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    unavailable?.Add("PubChem: Synonyms API unavailable");
                    return Array.Empty<string>();
                }
                var json = await resp.Content.ReadFromJsonAsync<System.Text.Json.Nodes.JsonNode>(cancellationToken: ct);
                var arr = json?["InformationList"]?["Information"]?[0]?["Synonym"]?.AsArray();
                if (arr is null) return Array.Empty<string>();
                return arr.Select(x => x?.ToString())
                          .Where(s => !string.IsNullOrWhiteSpace(s))
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .ToArray()!;
            }
            catch
            {
                unavailable?.Add("PubChem: Synonyms API error");
                return Array.Empty<string>();
            }
        }

        // -------- Back-compat overloads (no 'unavailable' arg required) --------
        public Task<int?> ResolveCidByCasAsync(string cas, CancellationToken ct = default)
            => ResolveCidByCasAsync(cas, unavailable: null, ct: ct);

        public Task<Dictionary<string, string?>> GetCorePropertiesAsync(int cid, CancellationToken ct = default)
            => GetCorePropertiesAsync(cid, unavailable: null, ct: ct);

        public Task<JsonNode?> GetPugViewAsync(int cid, string? heading, CancellationToken ct = default)
            => GetPugViewAsync(cid, heading, unavailable: null, ct: ct);

        public Task<JsonNode?> GetPugViewAsync(int cid, CancellationToken ct = default)
            => GetPugViewAsync(cid, heading: null, unavailable: null, ct: ct);
    }
}
