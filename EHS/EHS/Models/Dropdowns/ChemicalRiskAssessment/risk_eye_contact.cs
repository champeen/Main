using System.ComponentModel.DataAnnotations;

namespace EHS.Models.Dropdowns.ChemicalRiskAssessment
{
    public class risk_eye_contact : TimeStamps
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Sort Order")]
        public string? sort_order { get; set; }
        [Display(Name = "Display in Select List")]
        public bool display { get; set; }
        public int risk_rating { get; set; }                    // 0-5?  0-none, 5-severe?
    }
}
