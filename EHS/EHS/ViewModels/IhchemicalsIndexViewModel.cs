using EHS.Models.IH;
using System.Collections.Generic;


namespace EHS.ViewModels
{
    public class IhChemicalsIndexViewModel
    {
        public string? CasInput { get; set; }
        public ChemicalCoreDto? Result { get; set; }
        public List<IhChemHistoryRow> Recent { get; set; } = new();
        public List<string> UnavailableSources { get; set; } = new();
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
