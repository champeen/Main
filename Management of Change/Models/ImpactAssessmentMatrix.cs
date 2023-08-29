using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
        [NotMapped]
        [Display(Name = "Reviewer Name")]
        public string? ReviewerName { get; set; }
        [NotMapped]
        [Display(Name = "Reviewer Email")]
        public string? ReviewerEmail { get; set; }
        [NotMapped]
        [Display(Name = "Reviewer Username")]
        public string? ReviewerUsername { get; set; }
    }
}
