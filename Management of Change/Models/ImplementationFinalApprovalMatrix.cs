using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    [Index("ChangeType", "FinalReviewType", IsUnique = true, Name = "IX_ChangeType_FinalReviewType_Index")]
    public class ImplementationFinalApprovalMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        //[Index("IX_ChangeType", 1, IsUnique = true)]
        public string ChangeType { get; set; }
        //[Index("IX_FinalReviewType", IsUnique = true)]
        public string FinalReviewType { get; set; }
    }
}
