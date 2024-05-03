using PtnWaiver.Controllers;
using PtnWaiver.Models;

namespace PtnWaiver.ViewModels
{
    public class GroupApproverReviewVM
    {
        public PTN PTN { get; set; }
        public Waiver Waiver { get; set; }
        public GroupApproversReview GroupApproversReview { get; set; }
    }
}