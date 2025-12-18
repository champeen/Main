using System.ComponentModel.DataAnnotations;

namespace EHS.Models.Dropdowns.ChemicalRiskAssessment
{
    public class hazard_codes : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Code")]
        public string code { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Sort Order")]
        public string? sort_order { get; set; }
        [Display(Name = "Display in Select List")]
        public bool display { get; set; }
        [Display(Name = "Risk Rating")]
        public int risk_rating { get; set; }                    // 0-4?  0-none, 4-severe?
    }
}
