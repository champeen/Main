using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class AdditionalImpactAssessmentReviewers : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public int ChangeRequestId { get; set; }
        public string Reviewer { get; set; }
        [Display(Name="Reviewer Name")]
        public string? ReviewerName { get; set; }
        [Display(Name = "Reviewer Email")]
        public string? ReviewerEmail { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
        public string? ReviewArea { get; set; }
        public bool Selected { get; set; }
    }
}
