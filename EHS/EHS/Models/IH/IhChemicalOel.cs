namespace EHS.Models.IH
{
    public class IhChemicalOel
    {
        public int Id { get; set; }
        public int IhChemicalId { get; set; }
        public IhChemical IhChemical { get; set; } = null!;
        public string Source { get; set; } = string.Empty; // OSHA, NIOSH, ACGIH(ref)
        public string Type { get; set; } = string.Empty; // PEL, REL, TLV-TWA, STEL, Ceiling
        public string? Value { get; set; } // e.g., 0.75 ppm TWA
        public string? Notes { get; set; }
    }
}