using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class WaiverViewModel
    {
        public Waiver Waiver { get; set; }
        public PTN Ptn { get; set; }
        public List<GroupApproversReview> GroupApproversReview { get; set; }
        public List<Attachment>? AttachmentsWaiver { get; set; }
        public List<Attachment>? AttachmentsWaiverMaterialDetail { get; set; }

        public string? TabActiveDetail { get; set; }
        public string? TabActiveAttachmentsWaiver { get; set; }
        public string? TabActiveWaiverApproval { get; set; }
        public string? TabActiveWaiverAdminApproval { get; set; }
        public string? TabActiveWaiverMaterialDetails { get; set; }
        public string? TabSubmitWaiverForApprovalDisabled { get; set; }
        public string? TabApproveWaiverDisabled { get; set; }
        public string? TabWaiverMaterialDetailsDisabled { get; set; }

        public bool ButtonSubmitForReview { get; set; }
        public string? FileAttachmentError { get; set; }
        
    }
}