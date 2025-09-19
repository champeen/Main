namespace EHS.Models.IH
{
    public class IhChemicalHazard
    {
        public int Id { get; set; }
        public int IhChemicalId { get; set; }
        public IhChemical IhChemical { get; set; } = null!;
        public string Source { get; set; } = string.Empty; // GHS, NFPA, HMIS, Toxicity
        public string? Code { get; set; } // e.g., H225, NFPA Health=3
        public string? Description { get; set; } // statement text
    }
}