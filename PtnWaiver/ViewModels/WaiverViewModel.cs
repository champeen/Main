using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class WaiverViewModel
    {
        public Waiver Waiver { get; set; }
        public PTN Ptn { get; set; }
        public List<Attachment>? AttachmentsWaiver { get; set; }

        public string? TabActiveDetail { get; set; }
        public string? TabActiveAttachmentsWaiver { get; set; }
        public string? TabActiveWaiverApproval { get; set; }
        public string? TabActiveWaiverAdminApproval { get; set; }

        public string? Tab3Disabled { get; set; }
        public string? Tab4Disabled { get; set; }

        public bool ButtonSubmitForReview { get; set; }
        public string? FileAttachmentError { get; set; }
        
    }
}