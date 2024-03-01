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
        public string Area { get; set; }
        [Display(Name = "Subject Type")]
        public string SubjectType { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        [Display(Name = "TIS Number (Completed)")]
        public string? TisNumber { get; set; }
        [Display(Name = "Link to original PDF copy")]
        public string? PdfLocation { get; set; }
        public string Status { get; set; }
        [Display(Name = "Roadblocks (if open > 90 days)")]
        public string? Roadblocks { get; set; }
        public string? SubmittedForAdminApprovalUser { get; set; }
        public string? SubmittedForAdminApprovalUserFullName { get; set; }
        public DateTime? SubmittedForAdminApprovalDate { get; set; }
        public string? ApprovedByAdminlUser { get; set; }
        public string? ApprovedByAdminlUserFullName { get; set; }
        public DateTime? ApprovedByAdminDate { get; set; }
        public string? CompletedBylUser { get; set; }
        public string? CompletedBylUserFullName { get; set; }
        public DateTime? CompletedByDate { get; set; }


        public virtual List<Waiver>? Waivers { get; set; }
    }
}
