using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class PTN : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Doc Id")]
        public string? DocId { get; set; }
        [Display(Name = "PTN/PIN")]
        public string PtnPin { get; set; }
        [Display(Name = "Subject Type")]
        public string SubjectType { get; set; }
        public string Title { get; set; }
        [Display(Name = "Group/Approver")]
        public string GroupApprover { get; set; }
        [Display(Name = "PTR Number (Completed)")]
        public string? PtrNumber { get; set; }
        [Display(Name = "Link to original PDF copy")]
        public string? PdfLocation { get; set; }
        public string Status { get; set; }
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
        [Display(Name = "Primary Approver Username")]
        public string? PrimaryApproverUsername { get; set; }
        [Display(Name = "Primary Approver")]
        public string? PrimaryApproverFullName { get; set; }
        [Display(Name = "Primary Approver Email")]
        public string? PrimaryApproverEmail { get; set; }
        [Display(Name = "Primary Approver Title")]
        public string? PrimaryApproverTitle { get; set; }
        [Display(Name = "Secondary Approver Username")]
        public string? SecondaryApproverUsername { get; set; }
        [Display(Name = "Secondary Approver")]
        public string? SecondaryApproverFullName { get; set; }
        [Display(Name = "Secondary Approver Email")]
        public string? SecondaryApproverEmail { get; set; }
        [Display(Name = "Secondary Approver Title")]
        public string? SecondaryApproverTitle { get; set; }
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


        public virtual List<Waiver>? Waivers { get; set; }
    }
}
