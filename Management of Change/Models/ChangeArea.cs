using System.ComponentModel.DataAnnotations;

namespace Management_of_Change.Models
{
    public class ChangeArea : TimeStamps
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        [Display(Name = "Change Grade Primary Approver Username")]
        public string? ChangeGradePrimaryApproverUsername { get; set; }
        [Display(Name = "Change Grade Primary Approver")]
        public string? ChangeGradePrimaryApproverFullName { get; set; }
        [Display(Name = "Change Grade Primary Approver Email")]
        public string? ChangeGradePrimaryApproverEmail { get; set; }
        [Display(Name = "Change Grade Primary Approver Title")]
        public string? ChangeGradePrimaryApproverTitle { get; set; }
        [Display(Name = "Change Grade Secondary Approver Username")]
        public string? ChangeGradeSecondaryApproverUsername { get; set; }
        [Display(Name = "Change Grade Secondary Approver")]
        public string? ChangeGradeSecondaryApproverFullName { get; set; }
        [Display(Name = "Change Grade Secondary Approver Email")]
        public string? ChangeGradeSecondaryApproverEmail { get; set; }
        [Display(Name = "Change Grade Secondary Approver Title")]
        public string? ChangeGradeSecondaryApproverTitle { get; set; }
        public string? Order { get; set; }
    }
}
