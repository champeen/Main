namespace EHS.Models.IH
{
    public class IhChemicalSamplingMethod
    {
        public int Id { get; set; }
        public int IhChemicalId { get; set; }
        public IhChemical IhChemical { get; set; } = null!;
        public string Source { get; set; } = string.Empty; // NIOSH, OSHA
        public string MethodId { get; set; } = string.Empty; // e.g., "NIOSH 2016"
        public string? Url { get; set; }
        public string? Notes { get; set; }
    }
}