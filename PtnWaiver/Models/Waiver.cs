using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PtnWaiver.Models
{
    public class Waiver : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Waiver Number")]
        public string? WaiverNumber { get; set; }
        [Display(Name = "Revision Number")]
        public int RevisionNumber { get; set; }
        [Display(Name = "POR or Project")]
        public string? PorProject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        [Display(Name = "Product/Process")]
        public List<string> ProductProcess { get; set; }
        [Display(Name = "Group/Approver(s)")]
        public List<string> GroupApprover { get; set; }
        [Display(Name = "Date Closed")]
        [DataType(DataType.Date)]
        public DateTime? DateClosed { get; set; }
        [Display(Name = "Corrective Action Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? CorrectiveActionDueDate { get; set; }

        [Display(Name = "Rejected Before Submission To Admin")]
        public bool? RejectedBeforeSubmission { get; set; }
        [Display(Name = "Rejected By Admin")]
        public bool? RejectedByApprover { get; set; }
        [Display(Name = "Rejected Reason")]
        public string? RejectedReason { get; set; }
        public string? SubmittedForApprovalUser { get; set; }
        public string? SubmittedForApprovalUserFullName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? SubmittedForApprovalDate { get; set; }
        public string? ApprovedByUser { get; set; }
        public string? ApprovedByUserFullName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ApprovedByDate { get; set; }
        public string? CompletedBylUser { get; set; }
        public string? CompletedBylUserFullName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CompletedByDate { get; set; }
        public bool? IsMostCurrentWaiver { get; set; }
        [Display(Name = "Material Detail Notes")]
        public string? MaterialDetailNotes { get; set; }
        [Display(Name = "Additional Email Notification(s) of Material Details")]
        public List<string>? AdditionalEmailNotificationsOfMaterialDetails { get; set; }

        [ForeignKey("PTN")]
        public int PTNId { get; set; }
        [Display(Name ="PTN Doc Id")]
        public string? PtnDocId { get; set; }
        public virtual PTN? PTN {  get; set; }
    }
}
