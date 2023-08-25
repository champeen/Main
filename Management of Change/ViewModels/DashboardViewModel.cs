using Management_of_Change.Models;
namespace Management_of_Change.ViewModels
{
    public class DashboardViewModel
    {
        public List<ChangeRequest>? DraftMocs { get; set; }
        public List<ChangeRequest>? IncompleteImpactAssessments { get; set; }
        public List<ChangeRequest>? IncompleteFinalApprovals { get; set; }
        public List<Models.Task>? OpenTasks { get; set; }
    }
}
