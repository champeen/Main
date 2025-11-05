namespace EHS.Models.IH
{
    public class IhChemicalSynonym
    {
        public int Id { get; set; }
        public int IhChemicalId { get; set; }
        public IhChemical IhChemical { get; set; } = null!;
        public string Synonym { get; set; } = string.Empty;
    }
}