using System.ComponentModel.DataAnnotations;

namespace EHS.Models.IH
{
    public class IhChemical
    {
        public int Id { get; set; }
        [Required, StringLength(64)]
        public string CasNumber { get; set; } = string.Empty;
        public int? PubChemCid { get; set; }
        [StringLength(256)]
        public string? PreferredName { get; set; }
        public List<IhChemicalSynonym> Synonyms { get; set; } = new();
        public List<IhChemicalProperty> Properties { get; set; } = new();
        public List<IhChemicalHazard> Hazards { get; set; } = new();
        public List<IhChemicalOel> OELs { get; set; } = new();
        public List<IhChemicalSamplingMethod> SamplingMethods { get; set; } = new();
    }
}