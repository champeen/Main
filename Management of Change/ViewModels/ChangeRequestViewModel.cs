using Management_of_Change.Models;
namespace Management_of_Change.ViewModels
{
    public class ChangeRequestViewModel
    {
        public ChangeRequest? ChangeRequest { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public List<Models.Task> Tasks { get; set; }
        public string? Tab3Disabled { get; set; }
        public string? Tab4Disabled { get; set; }
        public string? Tab5Disabled { get; set; }
        public string? Tab6Disabled { get; set; }
        public string? TabActiveDetail { get; set; }
        public string? TabActiveGeneralMocQuestions { get; set; }
        public string? TabActiveImpactAssessments { get; set; }
        public string? TabActiveFinalApprovals { get; set; }
        public string? TabActiveImplementation { get; set; }
        public string? TabActiveCloseoutComplete { get; set; }
        public string? TabActiveAttachments { get; set; }
        public string? TabActiveTasks { get; set; }
        public bool ButtonSubmitForReview { get; set; }
        public string? FileAttachmentError { get; set; }
        public string? IArecord { get; set; }
    }
}
