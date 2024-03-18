using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class DashboardViewModel
    {
        public List<PTN>? YourInProgressPtns { get; set; }
        public List<Waiver>? YourInProgressWaivers { get; set; }
        public List<PTN>? PtnsAwaitingYourApproval { get; set; }
        public List<Waiver>? WaiversAwaitingYourApproval { get; set; }
    }
}
