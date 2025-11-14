using System.ComponentModel.DataAnnotations;

namespace EHS.Models.Dropdowns.ChemicalRiskAssessment
{
    public class ppe_glove : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Sort Order")]
        public string? sort_order { get; set; }
        [Display(Name = "Display in Select List")]
        public bool display { get; set; }
    }
}
