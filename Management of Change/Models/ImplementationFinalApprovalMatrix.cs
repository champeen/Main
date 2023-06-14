using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ImplementationFinalApprovalMatrix : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string ChangeType { get; set; }
        public string FinalReviewType { get; set; }
    }
}
