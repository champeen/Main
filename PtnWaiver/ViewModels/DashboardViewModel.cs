using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class DashboardViewModel
    {
        public List<PTN>? YourInProgressPtns { get; set; }
        public List<Waiver>? YourInProgressWaivers { get; set; }
        public List<PTN>? AdminInProgressPtns { get; set; }
        public List<Waiver>? AdminInProgressWaivers { get; set; }
    }
}
