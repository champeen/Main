using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;
using System.Diagnostics;

namespace PtnWaiver.Controllers
{
    public class HomeController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;
        private readonly ILogger<HomeController> _logger;

        public HomeController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc, ILogger<HomeController> logger) : base(contextPtnWaiver, contextMoc)
        {
            _logger = logger;
            _contextMoc = contextMoc;
            _contextPtnWaiver = contextPtnWaiver;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsAdmin = _isAdmin;
            string username = _username;
            ViewBag.Username = username;
            ViewBag.UserDisplayName = _userDisplayName;

            DashboardViewModel dashboardVM = new DashboardViewModel();

            // GET USERS IN-PROGRESS PTNs
            dashboardVM.YourInProgressPtns = await _contextPtnWaiver.PTN.Where(m => m.CreatedUser == username && m.DeletedDate == null && (m.Status == "Draft" || m.Status == "Pending Approval" || m.Status == "Approved")).OrderByDescending(m => m.CreatedDate).ToListAsync();

            // GET USERS PTNs AWAITING REVIEW
            dashboardVM.PtnsAwaitingYourApproval = new List<PTN>();
            var openPtnReviews = await _contextPtnWaiver.PTN.Where(m => m.Status == "Pending Approval").OrderByDescending(m => m.CreatedDate).ToListAsync();
            foreach (var ptn in openPtnReviews)
            {
                var yourGroupApproversReview = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == ptn.Id && m.SourceTable == "PTN" && m.Status == null && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();
                if (yourGroupApproversReview.Count > 0)
                    dashboardVM.PtnsAwaitingYourApproval.Add(ptn);
            }

            // GET USERS IN-PROGRESS WAIVERS
            dashboardVM.YourInProgressWaivers = await _contextPtnWaiver.Waiver.Where(m => m.CreatedUser == username && m.DeletedDate == null && (m.Status == "Draft" || m.Status == "Pending Approval" || m.Status == "Approved")).OrderByDescending(m => m.CreatedDate).ToListAsync();

            // GET USERS WAIVERS AWAITING REVIEW
            dashboardVM.WaiversAwaitingYourApproval = new List<Waiver>();
            var openWaiverReviews = await _contextPtnWaiver.Waiver.Where(m => m.Status == "Pending Approval").OrderByDescending(m => m.CreatedDate).ToListAsync();
            foreach (var waiver in openWaiverReviews)
            {
                var yourGroupApproversReview = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == waiver.Id && m.SourceTable == "Waiver" && m.Status == null && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();
                if (yourGroupApproversReview.Count > 0)
                    dashboardVM.WaiversAwaitingYourApproval.Add(waiver);
            }

            return View(dashboardVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = message });
        }
    }
}