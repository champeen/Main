using EHS.Models;

namespace EHS.ViewModels
{
    public class SegViewModel
    {
        public seg_risk_assessment seg_risk_assessments { get; set; }
        public List<Attachment> attachments { get; set; }
        public string FileAttachmentError { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }
    }
}
