using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    public class ImplementationFinalApprovalResponse : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }
        [Display(Name = "Final Review Type")]
        public string FinalReviewType { get; set; }
        public string? Reviewer { get; set; }
        [Display(Name = "Reviewer Email")]
        [EmailAddress]
        public string? ReviewerEmail { get; set; }
        public string? Username { get; set; }
        [Display(Name = "Review Result")]
        public string? ReviewResult { get; set; }
        [Display(Name = "Review Completed")]
        public bool ReviewCompleted { get; set; }
        [Display(Name = "Date Completed")]
        public DateTime? DateCompleted { get; set; }
        public string? Comments { get; set; }
        [ForeignKey("ChangeRequest")]
        public int ChangeRequestId { get; set; }
    }
}
