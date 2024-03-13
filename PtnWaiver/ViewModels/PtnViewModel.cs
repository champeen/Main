using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class PtnViewModel
    {
        public PTN PTN { get; set; }
        public List<Attachment>? AttachmentsPtn { get; set; }

        public string? TabActiveDetail { get; set; }
        public string? TabActiveAttachmentsPtn { get; set; }
        public string? TabActivePtnApproval { get; set; }
        public string? TabActivePtnAdminApproval { get; set; }
        public string? TabActiveWaivers { get; set; }
        public string? TabActiveAttachmentsWaiver { get; set; }

        public string? Tab3Disabled { get; set; }
        public string? Tab4Disabled { get; set; }
        public string? Tab5Disabled { get; set; }
        public string? Tab6Disabled { get; set; }

        public string? FileAttachmentError { get; set; }
        
    }
}