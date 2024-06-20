using Management_of_Change.Models;
namespace Management_of_Change.ViewModels
{
    public class PccbVM
    {
        public PCCB PCCB { get; set; }
        public List<String>? Invitees { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public string? FileAttachmentError { get; set; }
    }
}
