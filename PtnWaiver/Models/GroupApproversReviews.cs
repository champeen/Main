using System.ComponentModel.DataAnnotations;

namespace PtnWaiver.Models
{
    public class GroupApproversReview : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string SourceTable { get; set; }
        public string Group { get; set; }

        [Display(Name = "Primary Approver Username")]
        public string PrimaryApproverUsername { get; set; }
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

        [Display(Name = "Admin Username")]
        public string? AdminUsername { get; set; }

        public string? Status { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }
        [Display(Name = "Reviewed By")]
        public String? ReviewedBy { get; set; }

        public string? Comment { get; set; }
        public string? Order { get; set; }
    }
}
