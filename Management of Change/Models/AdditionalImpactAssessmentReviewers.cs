using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class AdditionalImpactAssessmentReviewers : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public int ChangeRequestId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewType { get; set; }
    }
}
