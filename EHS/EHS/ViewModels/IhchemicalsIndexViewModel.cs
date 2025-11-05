using System;
using System.Collections.Generic;
using EHS.Models.IH;

namespace EHS.ViewModels
{
    public class IhChemicalsIndexViewModel
    {
        public string? CasInput { get; set; }
        public ChemicalCoreDto? Result { get; set; }
        public List<IhChemHistoryRow> Recent { get; set; } = new();
        public List<string> UnavailableSources { get; set; } = new();

        // Overwrite / existence flags for admin-save flow
        public bool ExistsAlready { get; set; }
        public bool OpenOverwriteModal { get; set; }
        public int? ExistingId { get; set; }
        public DateTimeOffset? ExistingUpdatedAt { get; set; } // optional; leave null if not tracked

        public bool ReadOnly { get; set; }

        public class RecentRow
        {
            public int Id { get; set; }
            public string CasNumber { get; set; } = "";
            public string? PreferredName { get; set; }
            public int? PubChemCid { get; set; }
            public int SynonymCount { get; set; }
            public int PropertyCount { get; set; }
            public int HazardCount { get; set; }
            public int OelCount { get; set; }
            public int SamplingCount { get; set; }
        }
    }

    public class IhChemHistoryRow
    {
        public int Id { get; set; }
        public string CasNumber { get; set; } = string.Empty;
        public string? PreferredName { get; set; }
        public int? PubChemCid { get; set; }
        public int SynonymCount { get; set; }
        public int PropertyCount { get; set; }
        public int HazardCount { get; set; }
        public int OelCount { get; set; }
        public int SamplingCount { get; set; }
    }

    public record ChemicalCoreDto(
        string CasNumber,
        string? PreferredName,
        int? PubChemCid,
        string[] Synonyms,
        Dictionary<string, string?> Properties,
        List<IhChemicalHazard> Hazards,
        List<IhChemicalOel> OELs,
        List<IhChemicalSamplingMethod> SamplingMethods
    );
}

