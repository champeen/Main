using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImpactAssessmentResponse : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Review Type")]
        public string ReviewType { get; set; }
        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }
        public string? Reviewer { get; set; }
        [Display(Name = "Reviewer Email")]
        [EmailAddress]
        public string? ReviewerEmail { get; set; }
        public string? Username { get; set; }
        public bool QuestionsAnswered { get; set; }
        [Display(Name = "Review Completed")]
        public bool ReviewCompleted { get; set; }
        [Display(Name = "Date Completed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateCompleted { get; set; }
        public string? Comments { get; set; }
        public List<ImpactAssessmentResponseAnswer>? ImpactAssessmentResponseAnswers { get; set; }
        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }

    }
}
