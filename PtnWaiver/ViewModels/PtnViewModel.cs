using PtnWaiver.Controllers;
using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class PtnViewModel
    {
        public PTN PTN { get; set; }
        public List<GroupApproversReview> GroupApproversReview { get; set; }
        public List<Attachment>? AttachmentsPtn { get; set; }

        public string? TabActiveDetail { get; set; }
        public string? TabActiveAttachmentsPtn { get; set; }
        public string? TabActivePtnApproval { get; set; }
        public string? TabActivePtnAdminApproval { get; set; }
        public string? TabActiveWaivers { get; set; }
        public string? TabActiveAttachmentsWaiver { get; set; }
        public string? TabSubmitPtnForApprovalDisabled { get; set; }
        public string? TabApprovePtnDisabled { get; set; }
        public string? TabActiveWaiversDisabled { get; set; }

        public string? FileAttachmentError { get; set; }
        
    }
}