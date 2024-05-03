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

        public HomeController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc, ILogger<HomeController> logger) : base(contextPtnWaiver,contextMoc)
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

            dashboardVM.YourInProgressPtns = await _contextPtnWaiver.PTN.Where(m => m.CreatedUser == username && m.DeletedDate == null && (m.Status == "Draft" || m.Status == "Pending Approval" || m.Status == "Approved")).ToListAsync();

            // get all open PTN reviews for this user...
            dashboardVM.PtnsAwaitingYourApproval = new List<PTN>();            
            var openReviews = await _contextPtnWaiver.GroupApproversReview.Where(m=>m.Status == null && m.SourceTable == "PTN" && (m.PrimaryApproverUsername ==  username || m.SecondaryApproverUsername == username)).ToListAsync();
            foreach (var openReview in openReviews)
            {
                var ptn = await _contextPtnWaiver.PTN.Where(m => m.Id == openReview.SourceId).FirstOrDefaultAsync();
                dashboardVM.PtnsAwaitingYourApproval.Add(ptn);
            }

            //dashboardVM.PtnsAwaitingYourApproval = await _contextPtnWaiver.PTN.Where(m => m.Status == "Pending Approval" && m.DeletedDate == null && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();

            dashboardVM.YourInProgressWaivers = await _contextPtnWaiver.Waiver.Where(m => m.CreatedUser == username && m.DeletedDate == null && (m.Status == "Draft" || m.Status == "Pending Approval" || m.Status == "Approved")).ToListAsync();

            // get all open Waiver reviews for this user...
            dashboardVM.WaiversAwaitingYourApproval = new List<Waiver>();
            var openWaiverReviews = await _contextPtnWaiver.GroupApproversReview.Where(m => m.Status == null && m.SourceTable == "Waiver" && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();
            foreach (var openReview in openWaiverReviews)
            {
                var waiver = await _contextPtnWaiver.Waiver.Where(m => m.Id == openReview.SourceId).FirstOrDefaultAsync();
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