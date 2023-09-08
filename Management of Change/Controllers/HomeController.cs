using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Management_of_Change.ViewModels;
using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class HomeController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Management_of_ChangeContext context, ILogger<HomeController> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            string username = _username;
            ViewBag.Username = username;
            ViewBag.UserDisplayName = _userDisplayName;

            DashboardViewModel dashboardVM = new DashboardViewModel();

            // Get all active ChangeRequests for current user...
            dashboardVM.DraftMocs = await _context.ChangeRequest
                .Where(m => m.DeletedDate == null)
                .Where(m => m.Change_Owner == username)
                .Where(m => m.Change_Status == "Draft")
                .ToListAsync();

            // Get all incomplete Impact Assessments assigned to user...
            List<ChangeRequest> changeRequestsIA = await _context.ChangeRequest
                .Where(m => m.DeletedDate == null)
                .Where(m => m.Change_Status == "ImpactAssessmentReview")
                .ToListAsync();
            foreach(var changeRequest in changeRequestsIA)
            {
                changeRequest.ImpactAssessmentResponses = await _context.ImpactAssessmentResponse
                    .Where(m => m.ChangeRequestId == changeRequest.Id)
                    .Where(m => m.Username == username)
                    .Where(m => m.ReviewCompleted == false)
                    .ToListAsync();

                foreach (var impactAssessmentResponse in changeRequest.ImpactAssessmentResponses)
                {
                    impactAssessmentResponse.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
                        .Where(m => m.ImpactAssessmentResponseId == impactAssessmentResponse.Id)
                        .ToListAsync();
                }
            }
            dashboardVM.IncompleteImpactAssessments = changeRequestsIA.Where(m => m.ImpactAssessmentResponses.Count > 0).ToList();

            // Get all incomplete Final Approvals assigned to user...
            List<ChangeRequest> changeRequestsFA = await _context.ChangeRequest
                .Where(m => m.DeletedDate == null)
                .Where(m => m.Change_Status == "FinalApprovals")
                .ToListAsync();
            foreach (var changeRequest in changeRequestsFA)
            {
                changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse
                    .Where(m => m.ChangeRequestId == changeRequest.Id)
                    .Where(m => m.Username == username)
                    .Where(m => m.ReviewCompleted == false)
                    .ToListAsync();
            }
            dashboardVM.IncompleteFinalApprovals = changeRequestsFA.Where(m => m.ImplementationFinalApprovalResponses.Count > 0).ToList();

            // Get all the Open/In Progress Tasks associated with the user...
            dashboardVM.OpenTasks = await _context.Task
                .Where(m => m.Status == "Open" || m.Status == "In-Progress")
                .Where(m => m.AssignedToUser == username)
                .OrderBy(m => m.DueDate)
                .ToListAsync();

            return View(dashboardVM);
        }

        public IActionResult Privacy()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = message });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Unauthorized(string? message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = message });
        }
    }
}