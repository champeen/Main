using Management_of_Change.Models;
namespace Management_of_Change.ViewModels
{
    public class ChangeRequestViewModel
    {
        public ChangeRequest? ChangeRequest { get; set; }
        public string? Tab3Disabled { get; set; }
        public string? Tab4Disabled { get; set; }
        public string? TabActiveDetail { get; set; }
        public string? TabActiveGeneralMocQuestions { get; set; }
        public string? TabActiveImpactAssessments { get; set; }
        public string? TabActiveFinalApprovals { get; set; }

    }
}
