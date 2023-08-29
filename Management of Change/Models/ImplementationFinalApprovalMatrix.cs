using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_of_Change.Models
{
    [Index("ChangeType", "FinalReviewType", IsUnique = true, Name = "IX_ChangeType_FinalReviewType_Index")]
    public class ImplementationFinalApprovalMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        //[Index("IX_ChangeType", 1, IsUnique = true)]
        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }
        //[Index("IX_FinalReviewType", IsUnique = true)]
        [Display(Name = "Final Review Type")]
        public string FinalReviewType { get; set; }
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
