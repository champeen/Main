using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Management_of_Change.ViewModels;
using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Humanizer;
using Azure.Identity;
using System.Collections.Immutable;

namespace Management_of_Change.Controllers
{
    public class HomeController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver, ILogger<HomeController> logger) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
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
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToListAsync();

            // Get all active Classification Reviews for current user...
            // fist get all active MoC's that are in 'ClassificationReview' status....
            dashboardVM.ClassificationReviews = await _context.ChangeRequest
                .Where(m => m.DeletedDate == null && m.Change_Status == "ClassificationReview" && (m.CreatedUser == username || _isAdmin))
                //.Where(m => m.Change_Owner == username)
                //.Where(m => m.Change_Status == "ClassificationReview")
                //.Where(m => m.CreatedUser == username || _isAdmin)
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToListAsync();

            // Get all active ChangeGradeReviews for current user...
            // fist get all active MoC's that are in 'ChangeGradeReview' status....
            dashboardVM.ChangeGradeReviews = new List<ChangeRequest>();
            List<ChangeRequest> usersActiveChangeRequests = await _context.ChangeRequest
                .Where(m => m.DeletedDate == null)
                //.Where(m => m.Change_Owner == username)
                .Where(m => m.Change_Status == "ChangeGradeReview")
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToListAsync();

            // now only get the ones that have current user as primary or secondary ChangeGradeReviewer....
            foreach (var rec in usersActiveChangeRequests)
            {
                ChangeArea changeArea = await _context.ChangeArea.Where(m => m.Description == rec.Area_of_Change).FirstOrDefaultAsync();
                if (changeArea != null)
                {
                    if (changeArea.ChangeGradePrimaryApproverUsername == _username || changeArea.ChangeGradeSecondaryApproverUsername == _username || rec.CreatedUser == _username)
                        dashboardVM.ChangeGradeReviews.Add(rec);
                }                        
            }

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
            dashboardVM.IncompleteImpactAssessments = changeRequestsIA
                .Where(m => m.ImpactAssessmentResponses.Count > 0)
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToList();

            // Get all MOCs with incomplete PCCB reviews....
            //List<PCCB> openPccbs = await _context.PCCB.Where(m => m.Status == "Open").ToListAsync();

            //var IncompletePccbMocs = _context.PCCB
            //    .Where(m => m.Status == "Open")
            //    .GroupBy(m => m.ChangeRequestId)
            //    .Select(m => m.Key).ToList();
            //dashboardVM.IncompletePccbReviews = _context.ChangeRequest.Where(m => IncompletePccbMocs.Contains(m.Id)).ToList();

            dashboardVM.IncompletePccbReviews = await _context.ChangeRequest.Where(m => m.Change_Status == "PccbReview" && (m.CreatedUser == username || _isAdmin)).ToListAsync();

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
            dashboardVM.IncompleteFinalApprovals = changeRequestsFA
                .Where(m => m.ImplementationFinalApprovalResponses.Count > 0)
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToList();

            // Get all the Open/In Progress Tasks associated with the user...
            dashboardVM.OpenTasks = await _context.Task
                .Where(m => m.Status == "Open" || m.Status == "In-Progress" || m.Status == "On Hold")
                .Where(m => m.AssignedToUser == username)
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.DueDate)
                .ToListAsync();

            // Get Count of Overdue Tasks grouped by User
            //dashboardVM.OverdueTasks = await _context.Task
            var overdueTasks = (await _context.Task
                .Where(m=>m.DueDate.Value.Date < DateTime.Now.Date && m.CompletionDate == null && m.Status != "On Hold" && m.Status != "Cancelled" && m.Status != "Complete")
                .GroupBy(m => m.AssignedToUserFullName)
                .Select(m => new { UserName = m.Key, Count = m.Count() })  
                .OrderByDescending(m => m.Count)
                .ThenBy(m=>m.UserName)
                //.ToListAsync()).Take(10);
                .ToListAsync());

            dashboardVM.OverdueTasks = new List<OverdueTasks>();
            foreach (var overdueTask in overdueTasks)
            {
                OverdueTasks rec = new OverdueTasks
                {
                    UserName = overdueTask.UserName,
                    Count = overdueTask.Count
                };
                dashboardVM.OverdueTasks.Add(rec);
            }

            ViewBag.Employees = getUserList();

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