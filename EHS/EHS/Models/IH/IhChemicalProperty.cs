namespace EHS.Models.IH
{
    public class IhChemicalProperty
    {
        public int Id { get; set; }
        public int IhChemicalId { get; set; }
        public IhChemical IhChemical { get; set; } = null!;
        public string Key { get; set; } = string.Empty; // e.g., MolecularFormula, MolecularWeight, CanonicalSMILES, InChI, InChIKey, XLogP, ExactMass
        public string? Value { get; set; }
    }
}