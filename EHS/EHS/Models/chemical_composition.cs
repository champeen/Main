using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EHS.Models;
using EHS.Models.Dropdowns;
using EHS.Models.IH;

namespace EHS.Models
{
    public class chemical_composition : TimeStamps
    {
        [Key]
        public int id { get; set; }

        // Fix the FK name and add the navigation back to parent.
        [ForeignKey(nameof(assessment))]
        public int chemical_risk_assessment_id { get; set; }

        public chemical_risk_assessment? assessment { get; set; }

        [Display(Name = "CAS Number")]
        [MaxLength(64)]
        public string cas_number { get; set; } = string.Empty;

        [Display(Name = "Chemical Name")]
        [Required, MaxLength(256)]
        public string chemical_name { get; set; } = string.Empty;

        // Store concentrations as decimals, not strings, so you can sort/filter/range.
        // If you mean “percent”, use (5,2) or (6,3) etc.
        [Display(Name = "Concentration Low (%)")]
        [Column(TypeName = "numeric(6,3)")]
        public decimal? concentration_low { get; set; }

        [Display(Name = "Concentration High (%)")]
        [Column(TypeName = "numeric(6,3)")]
        public decimal? concentration_high { get; set; }
    }
}