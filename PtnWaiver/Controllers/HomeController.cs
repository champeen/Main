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
            dashboardVM.PtnsAwaitingYourApproval = await _contextPtnWaiver.PTN.Where(m => m.Status == "Pending Approval" && m.DeletedDate == null && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();
            dashboardVM.YourInProgressWaivers = await _contextPtnWaiver.Waiver.Where(m => m.CreatedUser == username && m.DeletedDate == null && (m.Status == "Draft" || m.Status == "Pending Approval" || m.Status == "Approved")).ToListAsync();
            dashboardVM.WaiversAwaitingYourApproval = await _contextPtnWaiver.Waiver.Where(m => m.Status == "Pending Approval" && m.DeletedDate == null && (m.PrimaryApproverUsername == username || m.SecondaryApproverUsername == username)).ToListAsync();

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