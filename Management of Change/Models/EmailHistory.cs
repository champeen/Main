using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class EmailHistory : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Sent To Name")]
        public string? SentToDisplayName { get; set; }
        [DisplayName("Sent To Username")]
        public string? SentToUsername { get; set; }
        [DisplayName("Sent To Email")]
        public string? SentToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public int? ChangeRequestId { get; set; }
        public int? ImpactAssessmentResponseId { get; set; }
        public int? ImplementationFinalApprovalResponseId { get; set; }
        public int? TaskId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
    }
}
