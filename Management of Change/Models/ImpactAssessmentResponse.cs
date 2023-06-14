using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentResponse : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string ReviewType { get; set; }
        public string ChangeType { get; set; }
        public string? Reviewer { get; set; }
        public string? ReviewerEmail { get; set; }
        public bool Required { get; set; }
        public bool ReviewCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string? Comments { get; set; }
        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }
    }
}
