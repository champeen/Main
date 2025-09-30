using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class PTN : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Doc Id")]
        public string? DocId { get; set; }
        [Display(Name = "Originating Group")]
        public string OriginatingGroup { get; set; }
        [Display(Name = "Product Size")]        
        public string ProductSize { get; set; }
        public string? OriginatorInitials { get; set; }
        public string? OriginatorYear { get; set; }
        public string? SerialNumber { get; set; }
        //public string SubjectType { get; set; }
        [Display(Name = "Subject Type(s)")]
        public List<string> SubjectType { get; set; }
        public string Title { get; set; }
        [Display(Name = "Department Approver")]
        public List<string> GroupApprover { get; set; }
        [Display(Name = "PTR Number (Completed)")]
        public string? PtrNumber { get; set; }
        [Display(Name = "Link to original PDF copy")]
        public string? PdfLocation { get; set; }
        public string Status { get; set; }
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
        [Display(Name = "Rejected Before Submission To Admin")]
        public bool? RejectedBeforeSubmission { get; set; }
        [Display(Name = "Rejected By Admin")]
        public bool? RejectedByApprover { get; set; }
        [Display(Name = "Rejected Reason")]
        public string? RejectedReason { get; set; }
        public string? SubmittedForApprovalUser { get; set; }
        public string? SubmittedForApprovalUserFullName { get; set; }
        public DateTime? SubmittedForApprovalDate { get; set; }
        public string? ApprovedByUser { get; set; }
        public string? ApprovedByUserFullName { get; set; }
        public DateTime? ApprovedByDate { get; set; }
        public string? CompletedBylUser { get; set; }
        public string? CompletedBylUserFullName { get; set; }
        public DateTime? CompletedByDate { get; set; }
        [Display(Name = "Check If PTN is Wafering Department Specific")]
        public bool isWaferingDepartment { get; set; }


        public virtual List<Waiver>? Waivers { get; set; }
    }
}
