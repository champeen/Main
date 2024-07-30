using Management_of_Change.Data;
using Management_of_Change.Migrations;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Threading.Tasks;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeRequestsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        //private readonly string AttachmentDirectory = @"C:\Applications\ManagementOfChange";
        //private readonly string AttachmentDirectory = @"\\aub1vdev-app01\ManagementOfChange\";
        //private readonly string AttachmentDirectory = @"\\BAY1VPRD-MOC01\ManagementOfChange\";

        public ChangeRequestsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ChangeRequests
        public async Task<IActionResult> Index(string statusFilter, string prevStatusFilter = null, string sort = null, string prevSort = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            // if no filter selected, keep previous
            if (statusFilter == null)
                statusFilter = prevStatusFilter;

            // Create Dropdown List of Status...
            var statusList = await _context.ChangeStatus.OrderBy(m => m.Order).ToListAsync();
            List<SelectListItem> statusDropdown = new List<SelectListItem>();
            SelectListItem item = new SelectListItem { Value = "AllCurrent", Text = "All Current (non closed/cancelled)" };
            if (statusFilter == "AllCurrent")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "All", Text = "All" };
            if (statusFilter == "All")
                item.Selected = true;
            statusDropdown.Add(item);
            foreach (var status in statusList)
            {
                item = new SelectListItem { Value = status.Status, Text = status.Description };
                if (item.Value == statusFilter)
                    item.Selected = true;
                else
                    item.Selected = false;
                statusDropdown.Add(item);
            }
            ViewBag.StatusList = statusDropdown;

            var requests = await _context.ChangeRequest.Where(m => m.DeletedDate == null).ToListAsync();

            switch (statusFilter)
            {
                case null:
                    ViewBag.PrevStatusFilter = "AllCurrent";
                    requests = requests.Where(m => m.Change_Status == "Draft" || m.Change_Status == "ChangeGradeReview" || m.Change_Status == "ImpactAssessmentReview" || m.Change_Status == "FinalApprovals" || m.Change_Status == "PccbReview" || m.Change_Status == "Implementation" || m.Change_Status == "Closeout").ToList();
                    break;
                case "All":
                    ViewBag.PrevStatusFilter = "All";
                    break;
                case "AllCurrent":
                    ViewBag.PrevStatusFilter = "AllCurrent";
                    requests = requests.Where(m => m.Change_Status == "Draft" || m.Change_Status == "ChangeGradeReview" || m.Change_Status == "ImpactAssessmentReview" || m.Change_Status == "FinalApprovals" || m.Change_Status == "PccbReview" || m.Change_Status == "Implementation" || m.Change_Status == "Closeout").ToList();
                    break;
                default:
                    requests = requests.Where(m => m.Change_Status == statusFilter).ToList();
                    ViewBag.PrevStatusFilter = statusFilter;
                    break;
            }

            // no sort selected, use previous sort...
            if (sort == null)
            {
                sort = prevSort;
                switch (sort)
                {
                    case null:
                        requests = requests.OrderBy(m => m.Priority).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = null;
                        break;
                    case "MocNumberAsc":
                        requests = requests.OrderBy(m => m.MOC_Number).ToList();
                        ViewBag.PrevSort = "MocNumberAsc";
                        break;
                    case "MocNumberDesc":
                        requests = requests.OrderByDescending(m => m.MOC_Number).ToList();
                        ViewBag.PrevSort = "MocNumberDesc";
                        break;
                    case "TitleAsc":
                        requests = requests.OrderBy(m => m.MOC_Number).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "TitleAsc";
                        break;
                    case "TitleDesc":
                        requests = requests.OrderByDescending(m => m.MOC_Number).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "TitleDesc";
                        break;
                    case "StatusAsc":
                        requests = requests.OrderBy(m => m.Change_Status_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "StatusAsc";
                        break;
                    case "StatusDesc":
                        requests = requests.OrderByDescending(m => m.Change_Status_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "StatusDesc";
                        break;
                    case "OwnerAsc":
                        requests = requests.OrderBy(m => m.Change_Owner_FullName).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "OwnerAsc";
                        break;
                    case "OwnerDesc":
                        requests = requests.OrderByDescending(m => m.Change_Owner_FullName).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "OwnerDesc";
                        break;
                    case "TypeAsc":
                        requests = requests.OrderBy(m => m.Change_Type).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "TypeAsc";
                        break;
                    case "TypeDesc":
                        requests = requests.OrderByDescending(m => m.Change_Type).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "TypeDesc";
                        break;
                    case "LevelAsc":
                        requests = requests.OrderBy(m => m.Change_Level).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "LevelAsc";
                        break;
                    case "LevelDesc":
                        requests = requests.OrderByDescending(m => m.Change_Level).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "LevelDesc";
                        break;
                    case "LocationAsc":
                        requests = requests.OrderBy(m => m.Location_Site).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "LocationAsc";
                        break;
                    case "LocationDesc":
                        requests = requests.OrderByDescending(m => m.Location_Site).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "LocationDesc";
                        break;
                    case "AreaAsc":
                        requests = requests.OrderBy(m => m.Area_of_Change).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "AreaAsc";
                        break;
                    case "AreaDesc":
                        requests = requests.OrderByDescending(m => m.Area_of_Change).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "AreaDesc";
                        break;
                    case "ProductLineAsc":
                        requests = requests.OrderBy(m => m.Proudct_Line).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "ProductLineAsc";
                        break;
                    case "ProductLineDesc":
                        requests = requests.OrderByDescending(m => m.Proudct_Line).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "ProductLineDesc";
                        break;
                    case "CompletionDateAsc":
                        requests = requests.OrderBy(m => m.Estimated_Completion_Date).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "CompletionDateAsc";
                        break;
                    case "CompletionDateDesc":
                        requests = requests.OrderByDescending(m => m.Estimated_Completion_Date).ThenBy(m => m.Estimated_Completion_Date).ToList();
                        ViewBag.PrevSort = "CompletionDateDesc";
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "MocNumber":
                        if (prevSort != null && prevSort == "MocNumberAsc")
                        {
                            requests = requests.OrderByDescending(m => m.MOC_Number).ToList();
                            ViewBag.PrevSort = "MocNumberDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.MOC_Number).ToList();
                            ViewBag.PrevSort = "MocNumberAsc";
                        }
                        break;
                    case "Title":
                        if (prevSort != null && prevSort == "TitleAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Title_Change_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "TitleDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Title_Change_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "TitleAsc";
                        }
                        break;
                    case "Status":
                        if (prevSort != null && prevSort == "StatusAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Change_Status_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "StatusDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Change_Status_Description).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "StatusAsc";
                        }
                        break;
                    case "Owner":
                        if (prevSort != null && prevSort == "OwnerAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Change_Owner_FullName).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "OwnerDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Change_Owner_FullName).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "OwnerAsc";
                        }
                        break;
                    case "Type":
                        if (prevSort != null && prevSort == "TypeAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Change_Type).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "TypeDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Change_Type).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "TypeAsc";
                        }
                        break;
                    case "Level":
                        if (prevSort != null && prevSort == "LevelAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Change_Level).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "LevelDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Change_Level).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "LevelAsc";
                        }
                        break;
                    case "Location":
                        if (prevSort != null && prevSort == "LocationAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Location_Site).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "LocationDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Location_Site).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "LocationAsc";
                        }
                        break;
                    case "Area":
                        if (prevSort != null && prevSort == "AreaAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Area_of_Change).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "AreaDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Area_of_Change).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "AreaAsc";
                        }
                        break;
                    case "ProductLine":
                        if (prevSort != null && prevSort == "ProductLineAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Proudct_Line).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "ProductLineDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Proudct_Line).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "ProductLineAsc";
                        }
                        break;
                    case "CompletionDate":
                        if (prevSort != null && prevSort == "CompletionDateAsc")
                        {
                            requests = requests.OrderByDescending(m => m.Estimated_Completion_Date).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "CompletionDateDesc";
                        }
                        else
                        {
                            requests = requests.OrderBy(m => m.Estimated_Completion_Date).ThenBy(m => m.Estimated_Completion_Date).ToList();
                            ViewBag.PrevSort = "CompletionDateAsc";
                        }
                        break;
                    default:
                        requests = requests.OrderBy(m => m.Priority).ThenBy(m => m.Change_Type).ToList();
                        ViewBag.PrevSort = null;
                        break;
                }
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(requests);
        }

        // GET: ChangeRequests/Details/5
        public async Task<IActionResult> Details(int? id, string? tab = "Details", string fileAttachmentError = null, string fileDownloadMessage = null, string recId = null, string questionsSaved = null, string changeGradeReviewError = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == id);

            if (changeRequest == null)
                return NotFound();

            ChangeRequestViewModel changeRequestViewModel = new ChangeRequestViewModel();
            changeRequestViewModel.FileAttachmentError = fileAttachmentError;
            ViewBag.ChangeGradeReviewError = changeGradeReviewError;

            // Get all the General MOC Responses associated with this request...
            changeRequest.GeneralMocResponses = await _context.GeneralMocResponses
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.Order)
                .ToListAsync();

            // Get all the Impact Assessment Responses associated with this request...
            changeRequest.ImpactAssessmentResponses = await _context.ImpactAssessmentResponse
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.ReviewType)
                .ThenBy(m => m.ChangeType)
                .ThenByDescending(m => m.ChangeArea)
                .ToListAsync();

            // Get all the Impact Assessment Responses Questions/Answers associated with this request...
            if (changeRequest.ImpactAssessmentResponses.Any())
            {
                foreach (var record in changeRequest.ImpactAssessmentResponses)
                {
                    record.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
                    .Where(m => m.ImpactAssessmentResponseId == record.Id)
                    .OrderBy(m => m.ReviewType)
                    .ThenBy(m => m.Order)
                    .ToListAsync();
                }
            }

            // Get all the Final Approval Responses associated with this request...
            changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.FinalReviewType)
                .ThenBy(m => m.ChangeType)
                .ToListAsync();

            // Get all the PCCB reviews for this change requests...
            changeRequest.PccbMeetings = await _context.PCCB
                .Where(m => m.ChangeRequestId == changeRequest.Id)
                .OrderBy(m => m.MeetingDateTime)
                .ToListAsync();

            //Get all PCCB Meeting Invitees...
            if (changeRequest.PccbMeetings.Any())
            {
                foreach (var record in changeRequest.PccbMeetings)
                {
                    record.Invitees = await _context.PccbInvitees
                        .Where(m => m.PccbId == record.Id)
                        .OrderBy(m => m.FullName)
                        .ToListAsync();
                }
            }

            ViewBag.ShowCloseoutButton = false;
            if (changeRequest.PccbMeetings.Count() > 0 && changeRequest.PccbMeetings.Where(m => m.Status != "Closed").Count() == 0)
                ViewBag.ShowCloseoutButton = true;

            // Get all the tasks associated with this ChangeRequest...
            List<Models.Task> tasks = await _context.Task
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.DueDate)
                .ThenBy(m => m.Id)
                .ToListAsync();

            // Get number of Open Pre-Implementation Tasks for this ChangeRequest
            ViewBag.OpenPreImplementationTasks = tasks
                .Where(m => m.Status == "Open" || m.Status == "In-Progress" || m.Status == "On Hold")
                .Where(m => m.ImplementationType == "Pre")
                .Count();

            // Get number of Open Tasks for this ChangeRequest
            ViewBag.OpenTasks = tasks
                .Where(m => m.Status == "Open" || m.Status == "In-Progress" || m.Status == "On Hold")
                .Count();

            // Get Employees to Notify full names instead of usernames...
            List<String> employeesToNotify = new List<string>();
            if (changeRequest.Additional_Notification != null && changeRequest.Additional_Notification.Count > 0)
            {
                foreach (var username in changeRequest.Additional_Notification)
                {
                    string fullName = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == username.ToLower()).Select(m => m.displayname).FirstOrDefaultAsync();
                    if (!String.IsNullOrWhiteSpace(fullName))
                        employeesToNotify.Add(fullName);
                }
            }
            ViewBag.EmployeesToNotify = employeesToNotify;

            changeRequestViewModel.ChangeRequest = changeRequest;
            changeRequestViewModel.Tasks = tasks;
            // disable tab 3 (ImpactAssessments) if General MOC Responses have not been completed...
            int countGMR = changeRequest.GeneralMocResponses.Where(m => m.Response == null).Count();
            changeRequestViewModel.Tab3Disabled = countGMR > 0 || changeRequestViewModel.ChangeRequest.Change_Status == "Draft" || changeRequestViewModel.ChangeRequest.Change_Status == "ChangeGradeReview" ? "disabled" : "";
            // disable tab4 (Final Review) if any Impact Assessment Responses have not been completed...
            int countIAR = changeRequest.ImpactAssessmentResponses.Where(m => m.ReviewCompleted == false).Count();
            changeRequestViewModel.Tab4Disabled = countIAR > 0 || (changeRequestViewModel.ChangeRequest.Change_Status == "Draft" || changeRequestViewModel.ChangeRequest.Change_Status == "ChangeGradeReview" || changeRequestViewModel.ChangeRequest.Change_Status == "ImpactAssessmentReview" || changeRequestViewModel.ChangeRequest.Change_Status == "PccbReview") ? "disabled" : "";
            // disable tab5 (Implementation) if any Final Approvals have not been completed...
            int countFA = changeRequest.ImplementationFinalApprovalResponses.Where(m => m.ReviewCompleted == false).Count();
            changeRequestViewModel.Tab5Disabled = countFA > 0 || (changeRequestViewModel.ChangeRequest.Change_Status == "Draft" || changeRequestViewModel.ChangeRequest.Change_Status == "ChangeGradeReview" || changeRequestViewModel.ChangeRequest.Change_Status == "ImpactAssessmentReview" || changeRequestViewModel.ChangeRequest.Change_Status == "FinalApprovals") ? "disabled" : "";
            // disable tab6 (Closeout/Complete) if change request is not in status of "Closeout" or "Closed"
            changeRequestViewModel.Tab6Disabled = changeRequest.Change_Status != "Closeout" && changeRequest.Change_Status != "Closed" ? "disabled" : "";
            // if change request is not in "Draft" status, and ChangeLevel is one that is required to review, then show the extra 'Change Level Review' stage....
            var changeLevel = await _context.ChangeLevel.Where(m => m.Level == changeRequest.Change_Level).FirstOrDefaultAsync();
            if (changeLevel == null)
                changeRequestViewModel.TabChangeGradeReviewDisplayed = "No";
            else
                changeRequestViewModel.TabChangeGradeReviewDisplayed = changeRequest.Change_Status != "Draft" && changeLevel.ChangeGradeReviewRequired == true ? "Yes" : "No";
            // PCCB Review Tab...
            changeRequestViewModel.TabPccbReviewDisplayed = changeLevel?.PccbReviewRequired == true ? "Yes" : "No";
            changeRequestViewModel.TabPccbReviewDisabled = changeRequest.Change_Status == "Draft" || changeRequest.Change_Status == "ChangeGradeReview" || changeRequest.Change_Status == "ImpactAssessmentReview" ? "disabled" : "";

            changeRequestViewModel.TabActiveDetail = "";
            changeRequestViewModel.TabActiveGeneralMocQuestions = "";
            changeRequestViewModel.TabActiveImpactAssessments = "";
            changeRequestViewModel.TabActivePccbReview = "";
            changeRequestViewModel.TabActiveFinalApprovals = "";
            changeRequestViewModel.TabActiveImplementation = "";
            changeRequestViewModel.TabActiveCloseoutComplete = "";
            changeRequestViewModel.TabActiveAttachments = "";
            changeRequestViewModel.TabActiveTasks = "";
            changeRequestViewModel.TabActiveChangeGradeReview = "";

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            switch (tab)
            {
                case null:
                    changeRequestViewModel.TabActiveDetail = "active";
                    break;
                case "":
                    changeRequestViewModel.TabActiveDetail = "active";
                    break;
                case "Details":
                    changeRequestViewModel.TabActiveDetail = "active";
                    break;
                case "GeneralMocQuestions":
                    changeRequestViewModel.TabActiveGeneralMocQuestions = "active";
                    break;
                case "ChangeGradeReview":
                    changeRequestViewModel.TabActiveChangeGradeReview = "active";
                    break;
                case "ImpactAssessments":
                    changeRequestViewModel.TabActiveImpactAssessments = "active";
                    break;
                case "PccbReview":
                    changeRequestViewModel.TabActivePccbReview = "active";
                    break;
                case "FinalApprovals":
                    changeRequestViewModel.TabActiveFinalApprovals = "active";
                    break;
                case "Implementation":
                    changeRequestViewModel.TabActiveImplementation = "active";
                    break;
                case "CloseoutComplete":
                    changeRequestViewModel.TabActiveCloseoutComplete = "active";
                    break;
                case "Attachments":
                    changeRequestViewModel.TabActiveAttachments = "active";
                    break;
                case "Tasks":
                    changeRequestViewModel.TabActiveTasks = "active";
                    break;
            }

            int count = changeRequest.GeneralMocResponses.Where(m => m.Response == null).Count();
            if (changeRequestViewModel.ChangeRequest.Change_Status == "Draft" && count == 0)
                changeRequestViewModel.ButtonSubmitForReview = true;
            else
                changeRequestViewModel.ButtonSubmitForReview = false;

            // Get all attachments    \\BAY1VPRD-MOC01\Management of Change\MOC-230707-1
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number)))
                path.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<ViewModels.Attachment> attachments = new List<Attachment>();
            foreach (FileInfo i in Files)
            {
                Attachment attachment = new Attachment
                {
                    Directory = i.DirectoryName,
                    Name = i.Name,
                    Extension = i.Extension,
                    FullPath = i.FullName,
                    CreatedDate = i.CreationTimeUtc.Date,
                    Size = Convert.ToInt32(i.Length)
                };
                attachments.Add(attachment);

                //var blah = i.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            changeRequestViewModel.Attachments = attachments.OrderBy(m => m.Name).ToList();

            //changeRequestViewModel.rec = "." + rec;
            changeRequestViewModel.IArecord = recId;
            changeRequestViewModel.ImplementationDisplayName = getUserDisplayName(changeRequest.Implementation_Username);
            changeRequestViewModel.CloseoutDisplayName = getUserDisplayName(changeRequest.Closeout_Username);
            changeRequestViewModel.CancelDisplayName = getUserDisplayName(changeRequest.Cancel_Username);
            changeRequestViewModel.CreatUserDisplayName = getUserDisplayName(changeRequest.CreatedUser);
            changeRequestViewModel.ModifiedUserDisplayName = getUserDisplayName(changeRequest.ModifiedUser);
            changeRequestViewModel.DeletedUserDisplayName = getUserDisplayName(changeRequest.DeletedUser);
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.QuestionsSaved = questionsSaved;

            ViewBag.UserCanReviewChangeGrade = false;
            // if change request is awaiting 'change grade review', get reviewer username...
            if (changeRequest.Change_Status == "ChangeGradeReview" && changeLevel != null && changeLevel.ChangeGradeReviewRequired == true)
                ViewBag.UserCanReviewChangeGrade = _context.ChangeArea.Where(m => m.ChangeGradePrimaryApproverUsername == _username || m.ChangeGradeSecondaryApproverUsername == _username).Any();

            changeRequestViewModel.employee = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == changeRequest.Change_Owner.ToLower()).FirstOrDefaultAsync();

            return View(changeRequestViewModel);
        }

        // GET: ChangeRequests/Create
        public async Task<IActionResult> Create(string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var changeOwner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == _username.ToLower()).FirstOrDefaultAsync();

            ChangeRequest changeRequest = new ChangeRequest
            {
                Change_Owner = _username,
                Change_Owner_FullName = changeOwner?.displayname,
                Change_Owner_Email = changeOwner?.mail,
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            changeRequest.Change_Status = await _context.ChangeStatus.OrderByDescending(cs => cs.Default).ThenBy(cs => cs.Order).ThenBy(cs => cs.Id).Select(cs => cs.Status).FirstOrDefaultAsync();
            changeRequest.Change_Status_Description = await _context.ChangeStatus.OrderByDescending(cs => cs.Default).ThenBy(cs => cs.Order).ThenBy(cs => cs.Id).Select(cs => cs.Description).FirstOrDefaultAsync();

            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Employees = getUserList();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Source = source;
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Change_Owner,Change_Owner_FullName,Change_Owner_Email,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,Cancel_Username,Cancel_Date,Cancel_Reason,Additional_Notification,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest, string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Valid Completion Date");

            if (changeRequest.Estimated_Completion_Date < DateTime.Today)
                ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            //if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
            //    ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // add General MOC Questions
                List<GeneralMocQuestions> questions = await _context.GeneralMocQuestions.OrderBy(m => m.Order).ToListAsync();
                if (questions.Count > 0)
                {
                    changeRequest.GeneralMocResponses = new List<GeneralMocResponses>();
                    foreach (var question in questions)
                    {
                        GeneralMocResponses response = new GeneralMocResponses
                        {
                            Question = question.Question,
                            Order = question.Order,
                            CreatedUser = _username,
                            CreatedDate = DateTime.Now
                        };
                        changeRequest.GeneralMocResponses.Add(response);
                    }
                }

                // Mark would like the MOC_Number to be in the format "MOC-YYMMDD(seq)"
                string mocNumber = "";
                for (int i = 1; i < 10000; i++)
                {
                    mocNumber = "MOC-" + DateTime.Now.ToString("yyMMdd") + "-" + i.ToString();
                    ChangeRequest record = await _context.ChangeRequest
                        .FirstOrDefaultAsync(m => m.MOC_Number == mocNumber);
                    if (record == null)
                        break;
                }
                changeRequest.MOC_Number = mocNumber;
                changeRequest.Request_Date = DateTime.Now.Date;
                _context.Add(changeRequest);
                await _context.SaveChangesAsync();

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number)))
                    path.Create();

                if (source != null && source == "Home")
                    return RedirectToAction("Index", "Home", new { });
                else
                    return RedirectToAction("Details", new { id = changeRequest.Id });
            }

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Employees = getUserList();
            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Source = source;

            return View(changeRequest);
        }

        public async Task<IActionResult> Clone(int id, string? tab = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return View("Index");

            ChangeRequest changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest == null)
                return View("Index");

            var changeOwner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == _username.ToLower()).FirstOrDefaultAsync();

            changeRequest.Change_Status = "Draft";
            changeRequest.Change_Owner = _username;
            changeRequest.Change_Owner_FullName = changeOwner?.displayname;
            changeRequest.Change_Owner_Email = changeOwner?.mail;
            changeRequest.Estimated_Completion_Date = null;
            changeRequest.CreatedUser = _username;
            changeRequest.CreatedDate = DateTime.Now;
            changeRequest.ModifiedDate = null;
            changeRequest.ModifiedUser = null;
            changeRequest.DeletedDate = null;
            changeRequest.DeletedUser = null;
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(cs => cs.Status == changeRequest.Change_Status).Select(cs => cs.Description).FirstOrDefaultAsync();

            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Employees = getUserList();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Source = "Home";
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.ClonedId = id;

            return View(changeRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloneCreate([Bind("Id,Change_Owner,Change_Owner_FullName,Change_Owner_Email,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,Additional_Notification,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest, int clonedId, string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Valid Completion Date");

            if (changeRequest.Estimated_Completion_Date < DateTime.Today)
                ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            //if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
            //    ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // add General MOC Questions
                List<GeneralMocResponses> oldResponses = await _context.GeneralMocResponses.Where(m => m.ChangeRequestId == clonedId).ToListAsync();
                if (oldResponses.Count > 0)
                {
                    changeRequest.GeneralMocResponses = new List<GeneralMocResponses>();
                    foreach (var oldResponse in oldResponses)
                    {
                        GeneralMocResponses response = new GeneralMocResponses
                        {
                            ChangeRequestId = clonedId,
                            Question = oldResponse.Question,
                            Response = oldResponse.Response,
                            Order = oldResponse.Order,
                            CreatedUser = _username,
                            CreatedDate = DateTime.Now
                        };
                        changeRequest.GeneralMocResponses.Add(response);
                    }
                }

                // Mark would like the MOC_Number to be in the format "MOC-YYMMDD(seq)"
                string mocNumber = "";
                for (int i = 1; i < 10000; i++)
                {
                    mocNumber = "MOC-" + DateTime.Now.ToString("yyMMdd") + "-" + i.ToString();
                    ChangeRequest record = await _context.ChangeRequest
                        .FirstOrDefaultAsync(m => m.MOC_Number == mocNumber);
                    if (record == null)
                        break;
                }
                changeRequest.MOC_Number = mocNumber;
                changeRequest.Request_Date = DateTime.Now.Date;
                _context.Add(changeRequest);
                await _context.SaveChangesAsync();

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number)))
                    path.Create();

                if (source != null && source == "Home")
                    return RedirectToAction("Index", "Home", new { });
                else
                    return RedirectToAction("Details", new { id = changeRequest.Id });
            }

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Employees = getUserList();
            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Source = "Home";
            ViewBag.ClonedId = clonedId;

            return View("Clone", changeRequest);
        }

        // GET: ChangeRequests/Edit/5
        public async Task<IActionResult> Edit(int? id, string tab = "Details")
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest.FindAsync(id);

            if (changeRequest == null)
                return NotFound();

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Users = getUserList();
            ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Tab = tab;
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MOC_Number,Change_Owner,Change_Owner_FullName,Change_Owner_Email,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Request_Date,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,Cancel_Username,Cancel_Date,Cancel_Reason,Additional_Notification,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id != changeRequest.Id)
                return NotFound();

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Valid Completion Date");

            // Dont check completion date on edit because if it was orignally put in fine, keep the original date pristine, dont make them have to change it!
            //if (changeRequest.Estimated_Completion_Date < DateTime.Today)
            //    ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            //if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
            //    ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

            var changeOwner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == changeRequest.Change_Owner.ToLower()).FirstOrDefaultAsync();
            changeRequest.Change_Owner_FullName = changeOwner?.displayname;
            changeRequest.Change_Owner_Email = changeOwner?.mail;

            if (ModelState.IsValid)
            {
                try
                {
                    changeRequest.ModifiedUser = _username;
                    changeRequest.ModifiedDate = DateTime.Now;
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeRequestExists(changeRequest.Id))
                        return NotFound();
                    else
                        throw;
                }
                //return RedirectToAction(nameof(Index));
                //if (ViewBag.Tab == "Index")
                //    return RedirectToAction("Index");
                //else
                return RedirectToAction("Details", new { id = id });
            }

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
            ViewBag.Users = getUserList();
            ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            //ViewBag.Tab = tab;
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMocQuestions(int id, [Bind("ChangeRequest, Tab3Disabled, Tab4Disabled, TabActiveDetail, TabActiveGeneralMocQuestions, TabActiveImpactAssessments, TabActiveFinalApprovals")] ChangeRequestViewModel changeRequestViewModel)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            foreach (GeneralMocResponses record in changeRequestViewModel.ChangeRequest.GeneralMocResponses)
            {
                record.ModifiedUser = _username;
                record.ModifiedDate = DateTime.Now;
                _context.Update(record);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = changeRequestViewModel.ChangeRequest.Id, tab = "GeneralMocQuestions", questionsSaved = "Yes" });
        }

        // GET: ChangeRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ChangeRequest == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.ChangeRequest' is null.");
            }
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest != null)
            {
                changeRequest.Change_Status = "Cancelled";
                changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Cancelled").Select(m => m.Description).FirstOrDefaultAsync();
                changeRequest.DeletedUser = _username;
                changeRequest.DeletedDate = DateTime.Now;
                _context.Update(changeRequest);
                await _context.SaveChangesAsync();
                //_context.ChangeRequest.Remove(changeRequest);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequest?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", new { id = id, tab = "Attachments", fileAttachmentError = "No File Has Been Selected For Upload" });

            var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest == null)
                return RedirectToAction("Index");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _context.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new { id = id, tab = "Attachments", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact MoC Admin to add, or change document to allowable type." });

            string filePath = Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number, fileAttachment.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }

            return RedirectToAction("Details", new { id = id, tab = "Attachments" });
        }

        public async Task<IActionResult> DownloadFile(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }

        public async Task<IActionResult> DownloadNewEquipmentChecklist(int id)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"wwwroot\Documents\MoC-New Equipment Checklist.xlsx");
            return File(fileBytes, "application/x-msdownload", @"MoC-New Equipment Checklist.xlsx");
        }

        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, tab = "Attachments" });
        }

        public async Task<IActionResult> CheckForChangeGradeReview(int id, string tab, string errorMessage = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            // Get change level and see if this Change Request needs a Change Grade review....
            var changeLevel = await _context.ChangeLevel.Where(m => m.Level == changeRequest.Change_Level).FirstOrDefaultAsync();
            if (changeLevel != null && changeLevel.ChangeGradeReviewRequired == true)
            {
                // CHANGE GRADE REVIEW NEEDED.  NEED TO SET STATUS TO 'SUBMITTED FOR CHANGE GRADE REVIEW', AND THEN GO BACK TO
                changeRequest.Change_Status = "ChangeGradeReview";
                changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(cs => cs.Status == changeRequest.Change_Status).Select(cs => cs.Description).FirstOrDefaultAsync();
                changeRequest.ModifiedDate = DateTime.Now;
                changeRequest.ModifiedUser = _username;
                _context.Update(changeRequest);
                await _context.SaveChangesAsync();

                // Send Email Out notifying person(s) responsible to review the Change Grade...
                string subject = @"Management of Change (MoC) - Change Grade Review.";
                string body = @"A Management of Change task has been created that needs to have its Change Grade reviewed. As it stands, this MoC will NOT go to PCCB review. Please look over the MoC and either accept the change grade as is, which will bypass PCCB review, or reject the change grade, which will notify the MoC writer via email that the change grade was rejected, changing the MoC status back to 'Draft'.  Please follow link below and review the change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                var changeArea = await _context.ChangeArea.Where(m => m.Description == changeRequest.Area_of_Change).FirstOrDefaultAsync();

                if (changeArea != null && changeArea.ChangeGradePrimaryApproverEmail != null)
                {
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, changeArea.ChangeGradePrimaryApproverEmail, changeArea.ChangeGradeSecondaryApproverEmail, null, changeRequest.Priority);
                    AddEmailHistory(changeRequest.Priority, subject, body, changeArea.ChangeGradePrimaryApproverFullName, changeArea.ChangeGradePrimaryApproverUsername, changeArea.ChangeGradePrimaryApproverEmail, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                }

                return RedirectToAction("Details", new { id = id, tab = "ChangeGradeReview" });
            }
            else
                // Change Grade Review is not needed. Go To Normal Process...
                return RedirectToAction("SubmitForReview", new { id = id, tab = tab, errorMessage = errorMessage });
        }

        public async Task<IActionResult> ApproveChangeGrade(int id, string tab, string errorMessage = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            changeRequest.Change_Status = "ImpactAssessmentReview";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(cs => cs.Status == changeRequest.Change_Status).Select(cs => cs.Description).FirstOrDefaultAsync();
            changeRequest.ChangeGradeApprovalUser = _username;
            changeRequest.ChangeGradeApprovalUserFullName = _userDisplayName;
            changeRequest.ChangeGradeApprovalDate = DateTime.Now;
            changeRequest.ChangeGradeRejectedUser = null;
            changeRequest.ChangeGradeRejectedUserFullName = null;
            changeRequest.ChangeGradeRejectedDate = null;
            changeRequest.ChangeGradeRejectedReason = null;
            changeRequest.ModifiedDate = DateTime.Now;
            changeRequest.ModifiedUser = _username;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email MoC Owner that their Change Request Grade was not accepted....
            string subject = @"Management of Change (MoC) - Change Grade Accepted.";
            string body = @"A Change Request has had its Change Grade approved. It is being moved onto Impact Assessment review.  Please follow link below and review the change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

            if (changeRequest != null && changeRequest.Change_Owner_Email != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, changeRequest.Change_Owner_Email, null, null, changeRequest.Priority);
                AddEmailHistory(changeRequest.Priority, subject, body, changeRequest.Change_Owner_FullName, changeRequest.Change_Owner_FullName, changeRequest.Change_Owner_Email, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }

            return RedirectToAction("SubmitForReview", new { id = id, tab = "ImpactAssessments", errorMessage = errorMessage });
        }

        public async Task<IActionResult> RejectChangeGrade([Bind("Id, ChangeRequest, ChangeGradeRejectedReason")] ChangeRequestViewModel changeRequestViewModel)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (changeRequestViewModel.ChangeRequest.Id == null || changeRequestViewModel.ChangeRequest.Id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(changeRequestViewModel.ChangeRequest.Id);
            if (changeRequest == null)
                return NotFound();

            if (changeRequestViewModel.ChangeRequest.ChangeGradeRejectedReason == null)
                return RedirectToAction("Details", new { id = changeRequestViewModel.ChangeRequest.Id, tab = "ChangeGradeReview", changeGradeReviewError = "Reject Reason Required" });

            changeRequest.Change_Status = "Draft";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(cs => cs.Status == changeRequest.Change_Status).Select(cs => cs.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedDate = DateTime.Now;
            changeRequest.ModifiedUser = _username;
            changeRequest.ChangeGradeRejectedUser = _username;
            changeRequest.ChangeGradeRejectedUserFullName = _userDisplayName;
            changeRequest.ChangeGradeRejectedDate = DateTime.Now;
            changeRequest.ChangeGradeRejectedReason = changeRequestViewModel.ChangeRequest.ChangeGradeRejectedReason;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email MoC Owner that their Change Request Grade was not accepted....
            string subject = @"Management of Change (MoC) - Change Grade Rejected.";
            string body = @"A Change Request has had its Change Grade rejected. As it stands, this MoC will NOT go to PCCB review.  Please follow link below and review the change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

            if (changeRequest != null && changeRequest.Change_Owner_Email != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, changeRequest.Change_Owner_Email, null, null, changeRequest.Priority);
                AddEmailHistory(changeRequest.Priority, subject, body, changeRequest.Change_Owner_FullName, changeRequest.Change_Owner_FullName, changeRequest.Change_Owner_Email, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }

            return RedirectToAction("Details", new { id = changeRequestViewModel.ChangeRequest.Id, tab = "Details" });
        }

        // This closes out 'GeneralMocQuestions' and moves to 'ImpactAssessments' stage
        public async Task<IActionResult> SubmitForReview(int id, string tab, string errorMessage = null)/* ChangeRequestViewModel changeRequestViewModel)*/
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            AdditionalImpactAssessmentReviewersVM vm = new AdditionalImpactAssessmentReviewersVM();
            vm.Tab = tab;
            vm.ChangeRequestId = id;
            vm.ChangeRequest = changeRequest;
            vm.EquipmentReviewerRequired = "No";
            vm.MaintenanceReviewerRequired = "No";
            vm.ErrorMessage = errorMessage;

            // See if Change Request requires an 'Equipment' Impact Assessment Reviewer...
            List<ImpactAssessmentMatrix> equipmentReviewTypes = await _context.ImpactAssessmentMatrix
                .Where(m => m.ChangeType == changeRequest.Change_Type && m.ReviewType == "Equipment")
                .ToListAsync();
            vm.EquipmentReviewers = new List<AdditionalImpactAssessmentReviewers>();
            if (equipmentReviewTypes.Any())
            {
                vm.EquipmentReviewerRequired = "Yes";
                var additionalReviewers = await _context.AdditionalImpactAssessmentReviewers.Where(m => m.ReviewType == "Equipment" && m.ChangeRequestId == id).ToListAsync();

                // see if there is already an equipment reviewer assigned that would be one of the base impact reviewers....
                foreach (var additionalReviewer in additionalReviewers)
                {
                    if (changeRequest.Area_of_Change == "Other")
                    {
                        List<ReviewType> found = await _context.ReviewType.Where(m => m.Type == "Equipment" && m.Username == additionalReviewer.Reviewer).ToListAsync();
                        if (found.Count > 0)
                            vm.EquipmentReviewerRequired = "No";
                    }
                    else
                    {
                        List<ReviewType> found = await _context.ReviewType.Where(m => m.Type == "Equipment" && m.ChangeArea == additionalReviewer.ReviewArea && m.Username == additionalReviewer.Reviewer).ToListAsync();
                        if (found.Count > 0)
                            vm.EquipmentReviewerRequired = "No";
                    }
                }

                if (vm.EquipmentReviewerRequired != null && vm.EquipmentReviewerRequired == "Yes")
                {
                    // get available to select from....
                    List<ReviewType> equipmentReviewers = new List<ReviewType>();
                    if (changeRequest.Area_of_Change == "Other")
                    {
                        equipmentReviewers = await _context.ReviewType.Where(m => m.Type == "Equipment").OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ToListAsync();
                    }
                    else
                    {
                        equipmentReviewers = await _context.ReviewType.Where(m => m.Type == "Equipment" && m.ChangeArea == changeRequest.Area_of_Change).OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ToListAsync();
                    }
                    foreach (var record in equipmentReviewers)
                    {
                        var found = await _context.AdditionalImpactAssessmentReviewers.Where(m => m.ReviewType == "Equipment" && m.Reviewer == record.Username && m.ChangeRequestId == changeRequest.Id).ToListAsync();
                        if (!found.Any())
                        {
                            AdditionalImpactAssessmentReviewers rec = new AdditionalImpactAssessmentReviewers
                            {
                                ChangeRequestId = changeRequest.Id,
                                ReviewType = record.Type,
                                ReviewArea = record.ChangeArea,
                                Reviewer = record.Username,
                                ReviewerEmail = record.Email,
                                ReviewerName = record.Reviewer,
                                Selected = false
                            };
                            vm.EquipmentReviewers.Add(rec);
                        }
                    }
                }
            }

            // See if Change Request requires a 'Maintenance & Reliability' Impact Assessment Reviewer...
            List<ImpactAssessmentMatrix> maintenanceReviewTypes = await _context.ImpactAssessmentMatrix
                .Where(m => m.ChangeType == changeRequest.Change_Type && m.ReviewType == "Maintenance & Reliability")
                .ToListAsync();
            vm.MaintenanceReviewers = new List<AdditionalImpactAssessmentReviewers>();
            if (maintenanceReviewTypes.Any())
            {
                vm.MaintenanceReviewerRequired = "Yes";
                var additionalReviewers = await _context.AdditionalImpactAssessmentReviewers.Where(m => m.ReviewType == "Maintenance & Reliability" && m.ChangeRequestId == id).ToListAsync();

                // see if there is already a Maintenance & Reliability reviewer assigned that would be one of the base impact reviewers....
                foreach (var additionalReviewer in additionalReviewers)
                {
                    if (changeRequest.Area_of_Change == "Other")
                    {
                        List<ReviewType> found = await _context.ReviewType.Where(m => m.Type == "Maintenance & Reliability" && m.Username == additionalReviewer.Reviewer).ToListAsync();
                        if (found.Count > 0)
                            vm.MaintenanceReviewerRequired = "No";
                    }
                    else
                    {
                        List<ReviewType> found = await _context.ReviewType.Where(m => m.Type == "Maintenance & Reliability" && m.ChangeArea == additionalReviewer.ReviewArea && m.Username == additionalReviewer.Reviewer).ToListAsync();
                        if (found.Count > 0)
                            vm.MaintenanceReviewerRequired = "No";
                    }
                }

                if (vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "Yes")
                {
                    // get available to select from....
                    List<ReviewType> maintenanceReviewers = new List<ReviewType>();
                    if (changeRequest.Area_of_Change == "Other")
                    {
                        maintenanceReviewers = await _context.ReviewType.Where(m => m.Type == "Maintenance & Reliability").OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ToListAsync();
                    }
                    else
                    {
                        maintenanceReviewers = await _context.ReviewType.Where(m => m.Type == "Maintenance & Reliability" && m.ChangeArea == changeRequest.Area_of_Change).OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ToListAsync();
                    }
                    foreach (var record2 in maintenanceReviewers)
                    {
                        var found = await _context.AdditionalImpactAssessmentReviewers.Where(m => m.ReviewType == "Maintenance & Reliability" && m.Reviewer == record2.Username && m.ChangeRequestId == changeRequest.Id).ToListAsync();
                        if (!found.Any())
                        {
                            AdditionalImpactAssessmentReviewers rec = new AdditionalImpactAssessmentReviewers
                            {
                                ChangeRequestId = changeRequest.Id,
                                ReviewType = record2.Type,
                                ReviewArea = record2.ChangeArea,
                                Reviewer = record2.Username,
                                ReviewerEmail = record2.Email,
                                ReviewerName = record2.Reviewer,
                                Selected = false
                            };
                            vm.MaintenanceReviewers.Add(rec);
                        }
                    }
                }
            }

            // If either an 'Equipment' reviewer or 'Maintenance & Reliability' reviewer are needed, we need to go to a new screen and add them first before closing out.
            if ((vm.EquipmentReviewerRequired != null && vm.EquipmentReviewerRequired == "Yes") || (vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "Yes"))
            {
                //vm.EquipmentReviewerRequired = "No";
                return View("SelectIAReviewers", vm);
            }

            // Close-out Draft and go to Impact Assessment Review...
            return RedirectToAction("CloseDraft", new { changeRequestId = vm.ChangeRequestId, tab = vm.Tab });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForReview2(int id, [Bind("AdditionalImpactAssessmentReviewers, ChangeRequestId, ChangeRequest, Tab, EquipmentReviewerRequired, MaintenanceReviewerRequired, EquipmentReviewers, MaintenanceReviewers, ChangeRequest")] AdditionalImpactAssessmentReviewersVM vm)
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            vm.ErrorMessage = null;
            if (vm.EquipmentReviewerRequired != null && vm.EquipmentReviewerRequired == "Yes")
            {
                List<AdditionalImpactAssessmentReviewers> reviewers = new List<AdditionalImpactAssessmentReviewers>();
                foreach (var record in vm.EquipmentReviewers)
                {
                    if (record.Selected == true)
                    {
                        var reviewer = new AdditionalImpactAssessmentReviewers
                        {
                            ChangeRequestId = vm.ChangeRequestId,
                            ReviewType = record.ReviewType,
                            ReviewArea = record.ReviewArea,
                            Reviewer = record.Reviewer,
                            ReviewerEmail = record.ReviewerEmail,
                            ReviewerName = record.ReviewerName,
                            CreatedDate = DateTime.Now,
                            CreatedUser = _username
                        };
                        _context.AdditionalImpactAssessmentReviewers.Add(reviewer);
                        vm.EquipmentReviewerRequired = "No";
                    }
                }
                if (vm.EquipmentReviewerRequired != null || vm.EquipmentReviewerRequired == "No")
                    await _context.SaveChangesAsync();
            }

            if (vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "Yes")
            {
                List<AdditionalImpactAssessmentReviewers> reviewers = new List<AdditionalImpactAssessmentReviewers>();
                foreach (var record in vm.MaintenanceReviewers)
                {
                    if (record.Selected == true)
                    {
                        var reviewer = new AdditionalImpactAssessmentReviewers
                        {
                            ChangeRequestId = vm.ChangeRequestId,
                            ReviewType = record.ReviewType,
                            ReviewArea = record.ReviewArea,
                            Reviewer = record.Reviewer,
                            ReviewerEmail = record.ReviewerEmail,
                            ReviewerName = record.ReviewerName,
                            CreatedDate = DateTime.Now,
                            CreatedUser = _username
                        };
                        _context.AdditionalImpactAssessmentReviewers.Add(reviewer);
                        vm.MaintenanceReviewerRequired = "No";
                    }
                }
                if (vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "No")
                    await _context.SaveChangesAsync();
            }

            // make sure if 1 reviewer is still required, that they have selected one...
            if (vm.EquipmentReviewerRequired != null && vm.EquipmentReviewerRequired == "Yes" && vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "Yes")
                vm.ErrorMessage = @"At least 1 Equipment Reviewer and 1 Maintenance Reviewer is required.";
            else if (vm.EquipmentReviewerRequired != null && vm.EquipmentReviewerRequired == "Yes")
                vm.ErrorMessage = @"At least 1 Equipment Reviewer is required.";
            else if (vm.MaintenanceReviewerRequired != null && vm.MaintenanceReviewerRequired == "Yes")
                vm.ErrorMessage = @"At least 1 Maintenance Reviewer is required.";

            if (vm.ErrorMessage != null)
                //return View("SelectIAReviewers", vm);
                return RedirectToAction("SubmitForReview", new { id = vm.ChangeRequestId, tab = vm.Tab, errorMessage = vm.ErrorMessage });

            // Close-out Draft and go to Impact Assessment Review...
            return RedirectToAction("CloseDraft", new { changeRequestId = vm.ChangeRequestId, tab = vm.Tab });
        }

        public async Task<IActionResult> CloseDraft(int changeRequestId, string tab)
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ChangeRequest changeRequest = await _context.ChangeRequest.FindAsync(changeRequestId);

            // add base Impact Assessment Responses from matrix
            List<ImpactAssessmentMatrix> impactAssessmentMatrix = await _context.ImpactAssessmentMatrix
                .Where(m => m.ChangeType == changeRequest.Change_Type)
                .OrderBy(m => m.ReviewType)
                .ThenBy(m => m.ChangeType)
                .ToListAsync();
            if (impactAssessmentMatrix.Count > 0)
            {
                changeRequest.ImpactAssessmentResponses = new List<ImpactAssessmentResponse>();
                foreach (var assessment in impactAssessmentMatrix)
                {
                    // Get ALL Review Types setup for this Change Type ... (*** only gets area of change for equipment, maintenance and reliability and quality)
                    List<ReviewType> reviews = await _context.ReviewType
                        .Where(rt =>
                            (rt.Type == assessment.ReviewType && rt.ChangeArea == null) ||
                            (rt.Type == assessment.ReviewType && assessment.ReviewType == "Quality" && rt.ChangeArea == changeRequest.Area_of_Change) ||
                            (rt.Type == assessment.ReviewType && assessment.ReviewType == "Test" && rt.ChangeArea == changeRequest.Area_of_Change))
                        .ToListAsync();

                    foreach (var review in reviews)
                    {
                        var found = await _context.ImpactAssessmentResponse.Where(m => m.ChangeRequestId == changeRequest.Id && m.ReviewType == review.Type && m.Reviewer == review.Reviewer).FirstOrDefaultAsync();
                        if (found == null)
                        {
                            ImpactAssessmentResponse response = new ImpactAssessmentResponse
                            {
                                ChangeRequestId = changeRequest.Id,
                                ReviewType = assessment.ReviewType,
                                ChangeType = assessment.ChangeType,
                                ChangeArea = review.ChangeArea,
                                Reviewer = review.Reviewer,
                                ReviewerEmail = review.Email,
                                Username = review.Username,
                                CreatedUser = _username,
                                CreatedDate = DateTime.Now
                            };
                            changeRequest.ImpactAssessmentResponses.Add(response);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            // add AdditonalImpactAssessmentReviewers ...
            List<AdditionalImpactAssessmentReviewers> reviewers = await _context.AdditionalImpactAssessmentReviewers.Where(m => m.ChangeRequestId == changeRequestId).ToListAsync();
            foreach (var review in reviewers)
            {
                // Make Sure the combination of Reviewer & ReviewType is not already added for this Change Request !!!!!
                var found = await _context.ImpactAssessmentResponse.Where(m => m.ChangeRequestId == changeRequest.Id && m.ReviewType == review.ReviewType && m.Username == review.Reviewer).ToListAsync();
                if (found.Count == 0)
                {
                    ImpactAssessmentResponse response = new ImpactAssessmentResponse
                    {
                        ChangeRequestId = changeRequest.Id,
                        ChangeType = changeRequest.Change_Type,
                        ReviewType = review.ReviewType,
                        ChangeArea = review.ReviewArea,
                        Reviewer = review.ReviewerName,
                        ReviewerEmail = review.ReviewerEmail,
                        Username = review.Reviewer,
                        CreatedUser = _username,
                        CreatedDate = DateTime.Now
                    };
                    // make sure record does not already exist...
                    var recFound = await _context.ImpactAssessmentResponse
                        .Where(m => m.ChangeRequestId == changeRequest.Id && m.ChangeType == changeRequest.Change_Type && m.Username == review.Reviewer && m.ReviewType == review.ReviewType && m.ChangeArea == review.ReviewArea).FirstOrDefaultAsync();
                    if (recFound == null)
                    {
                        changeRequest.ImpactAssessmentResponses.Add(response);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // add Impact Assessment Response Quesion/Answers
            if (changeRequest.ImpactAssessmentResponses != null && changeRequest.ImpactAssessmentResponses.Count > 0)
            {
                foreach (var record in changeRequest.ImpactAssessmentResponses)
                {
                    record.ImpactAssessmentResponseAnswers = new List<ImpactAssessmentResponseAnswer>();

                    List<ImpactAssessmentResponseQuestions> IARQuestions = await _context.ImpactAssessmentResponseQuestions.Where(m => m.ReviewType == record.ReviewType).ToListAsync();

                    // if there are no Impact Assessment questions, then set all answered as true
                    if (IARQuestions == null || IARQuestions.Count == 0)
                        record.QuestionsAnswered = true;
                    // else, setup all the questions to be answered...
                    else
                    {
                        foreach (var question in IARQuestions)
                        {
                            ImpactAssessmentResponseAnswer rec = new ImpactAssessmentResponseAnswer
                            {
                                ImpactAssessmentResponseId = record.Id,
                                ReviewType = record.ReviewType,
                                Question = question.Question,
                                Order = question.Order,
                                CreatedUser = _username,
                                CreatedDate = DateTime.Now
                            };
                            // make sure record does not already exist...
                            var recFound = await _context.ImpactAssessmentResponseAnswer
                                .Where(m => m.ImpactAssessmentResponseId == record.Id && m.ReviewType == record.ReviewType && m.Question == question.Question).FirstOrDefaultAsync();
                            if (recFound == null)
                            {
                                record.ImpactAssessmentResponseAnswers.Add(rec);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            // add Implementation Final Approval Responses
            List<ImplementationFinalApprovalMatrix> implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix
                .Where(m => m.ChangeType == changeRequest.Change_Type)
                .OrderBy(m => m.FinalReviewType)
                .ThenBy(m => m.ChangeType)
                .ToListAsync();
            if (implementationFinalApprovalMatrix.Count > 0)
            {
                changeRequest.ImplementationFinalApprovalResponses = new List<ImplementationFinalApprovalResponse>();
                foreach (var assessment in implementationFinalApprovalMatrix)
                {
                    FinalReviewType review = _context.FinalReviewType.Where(m => m.Type == assessment.FinalReviewType).FirstOrDefault();
                    if (review != null)
                    {
                        ImplementationFinalApprovalResponse response = new ImplementationFinalApprovalResponse
                        {
                            ChangeRequestId = changeRequest.Id,
                            FinalReviewType = assessment.FinalReviewType,
                            ChangeType = assessment.ChangeType,
                            Reviewer = review.Reviewer,
                            ReviewerEmail = review.Email,
                            Username = review.Username,
                            CreatedUser = _username,
                            CreatedDate = DateTime.Now
                        };
                        // do not allow duplicate Final approvals to be added
                        var recFound = await _context.ImplementationFinalApprovalResponse
                            .Where(m => m.ChangeRequestId == changeRequest.Id && m.FinalReviewType == assessment.FinalReviewType && m.Username == review.Username).FirstOrDefaultAsync();
                        if (recFound == null)
                        {
                            changeRequest.ImplementationFinalApprovalResponses.Add(response);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            // Update ChangeRequest...
            changeRequest.Change_Status = "ImpactAssessmentReview";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "ImpactAssessmentReview").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.Now;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            if (changeRequest.ImpactAssessmentResponses != null)
            {
                // Email All Users ImpactResponse Review/Approval links...
                foreach (var record in changeRequest.ImpactAssessmentResponses)
                {
                    string subject = @"Management of Change (MoC) - Impact Assessment Response Needed";
                    string body = @"Your Impact Assessment Response review is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, record.ReviewerEmail, null, null, changeRequest.Priority);
                    AddEmailHistory(changeRequest.Priority, subject, body, record.Reviewer, record.Username, record.ReviewerEmail, changeRequest.Id, record.Id, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                }
            }
            return RedirectToAction("Details", new { id = changeRequestId, tab = "ImpactAssessments" });
        }

        // This closes out 'ImpactAssessments' and moves to 'FinalApprovals' stage
        public async Task<IActionResult> MarkImpactAssessmentComplete(int id, string tab = "ImpactAssessments", string rec = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // get ImpactAssessmentResponse record...
            ImpactAssessmentResponse impactAssessmentResponse = await _context.ImpactAssessmentResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (impactAssessmentResponse == null)
                return NotFound();

            // Mark the Impact Assessment Response as complete...
            impactAssessmentResponse.ReviewCompleted = true;
            impactAssessmentResponse.DateCompleted = DateTime.Now;
            impactAssessmentResponse.ModifiedUser = _username;
            impactAssessmentResponse.ModifiedDate = DateTime.Now;
            _context.Update(impactAssessmentResponse);
            await _context.SaveChangesAsync();

            // See if all ImpactAssessments for this MoC are now complete - if so advance to next stage...
            bool foundIncomplete = await _context.ImpactAssessmentResponse
                .Where(m => m.ChangeRequestId == impactAssessmentResponse.ChangeRequestId)
                .Where(m => m.ReviewCompleted == null || m.ReviewCompleted == false)
                .AnyAsync();

            if (foundIncomplete == false)
            {
                // Get the Change Request and change the Status to the next step...
                var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == impactAssessmentResponse.ChangeRequestId);
                if (changeRequest == null)
                    return NotFound();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// See if change request needs to go to PCCB Review OR Final Review Next............
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ChangeLevel changeGrade = await _context.ChangeLevel.Where(m => m.Level == changeRequest.Change_Level).FirstOrDefaultAsync();
                if (changeGrade == null) return NotFound();

                if (changeGrade.PccbReviewRequired == true)
                {
                    changeRequest.Change_Status = "PccbReview";
                    changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "PccbReview").Select(m => m.Description).FirstOrDefaultAsync();
                    changeRequest.ModifiedDate = DateTime.Now;
                    changeRequest.ModifiedUser = _username;
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();

                    // Email all admins with 'Approver' rights that this Change Request has been submitted for Implementation....
                    var adminApproverList = await _context.Administrators.Where(m => m.Approver == true).ToListAsync();
                    foreach (var record in adminApproverList)
                    {
                        var adminToNotify = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == record.Username.ToLower()).FirstOrDefaultAsync();
                        string subject = @"Management of Change (MoC) - Submitted for PCCB Review";
                        string body = @"Change Request has been submitted for PCCB Review. Please follow link below and setup/manage the PCCB Review for the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                        // Send Email...
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, adminToNotify?.mail, null, null, changeRequest.Priority);

                        // Log that Email was Sent...
                        AddEmailHistory(changeRequest.Priority, subject, body, adminToNotify?.displayname, record.Username, adminToNotify?.mail, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                    }

                    return RedirectToAction("Details", new { id = impactAssessmentResponse.ChangeRequestId, tab = "PccbReview", rec = rec });
                }
                else   // THE BELOW PART WILL BE NEEDED AFTER APPROVAL OF PCCB REVIEW !!!!!!!!
                {
                    changeRequest.Change_Status = "FinalApprovals";
                    changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "FinalApprovals").Select(m => m.Description).FirstOrDefaultAsync();
                    changeRequest.ModifiedDate = DateTime.Now;
                    changeRequest.ModifiedUser = _username;
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();

                    changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse
                         .Where(m => m.ChangeRequestId == changeRequest.Id)
                         .ToListAsync();

                    // Email All Users Implementation Final Approval links...
                    foreach (var record in changeRequest.ImplementationFinalApprovalResponses)
                    {
                        string subject = @"Management of Change (MoC) - Final Approval Needed";
                        string body = @"Your Final Approval/Review is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                        Initialization.EmailProviderSmtp.SendMessage(subject, body, record.ReviewerEmail, null, null, changeRequest.Priority);
                        AddEmailHistory(changeRequest.Priority, subject, body, record.Reviewer, record.Username, record.ReviewerEmail, changeRequest.Id, null, record.Id, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                    }
                    return RedirectToAction("Details", new { id = impactAssessmentResponse.ChangeRequestId, tab = "FinalApprovals", rec = rec });
                }
            }
            return RedirectToAction("Details", new { id = impactAssessmentResponse.ChangeRequestId, tab = "ImpactAssessments", rec = rec });
        }

        public async Task<IActionResult> AssessmentReminder(int id, string tab = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse.FindAsync(id);
            if (impactAssessmentResponse == null)
                return RedirectToAction("Index");

            var changeRequest = await _context.ChangeRequest.FindAsync(impactAssessmentResponse.ChangeRequestId);
            if (changeRequest == null)
                return RedirectToAction("Index");

            // Send Email Out notifying the person who is assigned the task
            string subject = @"Management of Change (MoC) - Impact Assessment Response Reminder.";
            string body = @"REMINDER! A Management of Change Impact Assessment has been assigned to you.  Please follow link below and review your impact assessment. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";
            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == impactAssessmentResponse.Username.ToLower()).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, null, null, changeRequest.Priority);
                var emailHistory = AddEmailHistory(changeRequest.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, changeRequest.Id, impactAssessmentResponse.Id, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }
            return RedirectToAction("Details", new { id = changeRequest.Id, tab = tab });
        }

        public async Task<IActionResult> FinalApprovalReminder(int id, string tab = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FindAsync(id);
            if (implementationFinalApprovalResponse == null)
                return RedirectToAction("Index");

            var changeRequest = await _context.ChangeRequest.FindAsync(implementationFinalApprovalResponse.ChangeRequestId);
            if (changeRequest == null)
                return RedirectToAction("Index");

            // Send Email Out notifying the person who is assigned the task
            string subject = @"Management of Change (MoC) - Final Approval Reminder.";
            string body = @"REMINDER! A Management of Change Final Approval has been assigned to you.  Please follow link below and review your final approval. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";
            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == implementationFinalApprovalResponse.Username.ToLower()).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, null, null, changeRequest.Priority);
                var emailHistory = AddEmailHistory(changeRequest.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, changeRequest.Id, null, implementationFinalApprovalResponse.Id, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }
            return RedirectToAction("Details", new { id = changeRequest.Id, tab = tab });
        }

        public async Task<IActionResult> ClosePccbReview(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // There are no incomplete final approvals.  Advance to next stage.
            var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest != null)
            {
                changeRequest.Change_Status = "FinalApprovals";
                changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "FinalApprovals").Select(m => m.Description).FirstOrDefaultAsync();
                changeRequest.ModifiedDate = DateTime.Now;
                changeRequest.ModifiedUser = _username;
                _context.Update(changeRequest);
                await _context.SaveChangesAsync();
            }

            changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse
                .Where(m => m.ChangeRequestId == changeRequest.Id)
                .ToListAsync();

            // Email All Users Implementation Final Approval links...
            foreach (var record in changeRequest.ImplementationFinalApprovalResponses)
            {
                string subject = @"Management of Change (MoC) - Final Approval Needed";
                string body = @"Your Final Approval/Review is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                Initialization.EmailProviderSmtp.SendMessage(subject, body, record.ReviewerEmail, null, null, changeRequest.Priority);
                AddEmailHistory(changeRequest.Priority, subject, body, record.Reviewer, record.Username, record.ReviewerEmail, changeRequest.Id, null, record.Id, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }
            return RedirectToAction("Details", new { id = id, tab = "FinalApprovals" });
        }

        // This closes out 'FinalApprovals' and moves to 'Implementation' phase
        public async Task<IActionResult> CompleteFinalReview(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the FinalApproval record....
            ImplementationFinalApprovalResponse implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (implementationFinalApprovalResponse == null)
                return NotFound();

            implementationFinalApprovalResponse.ReviewCompleted = true;
            implementationFinalApprovalResponse.DateCompleted = DateTime.Now;
            implementationFinalApprovalResponse.ModifiedDate = DateTime.Now;
            implementationFinalApprovalResponse.ModifiedUser = _username;
            _context.Update(implementationFinalApprovalResponse);
            await _context.SaveChangesAsync();

            // check to see if all final reviews are complete for this change request.  If so, advanced to next stage/status...
            bool found = await _context.ImplementationFinalApprovalResponse
                .Where(m => m.ChangeRequestId == implementationFinalApprovalResponse.ChangeRequestId)
                .Where(m => m.ReviewCompleted == null || m.ReviewCompleted == false)
                .AnyAsync();

            if (!found)
            {
                // There are no incomplete final approvals.  Advance to next stage.
                var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == implementationFinalApprovalResponse.ChangeRequestId);
                if (changeRequest != null)
                {
                    changeRequest.Change_Status = "Implementation";
                    changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Implementation").Select(m => m.Description).FirstOrDefaultAsync();
                    changeRequest.ModifiedDate = DateTime.Now;
                    changeRequest.ModifiedUser = _username;
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();

                    // Email all admins with 'Approver' rights that this Change Request has been submitted for Implementation....
                    var adminApproverList = await _context.Administrators.Where(m => m.Approver == true).ToListAsync();
                    foreach (var record in adminApproverList)
                    {
                        var adminToNotify = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == record.Username.ToLower()).FirstOrDefaultAsync();
                        string subject = @"Management of Change (MoC) - Submitted for Implementation";
                        string body = @"Change Request has been submitted for implementation. All pre-implementation tasks will need to be completed to move forward. Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                        // Send Email...
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, adminToNotify?.mail, null, null, changeRequest.Priority);

                        // Log that Email was Sent...
                        AddEmailHistory(changeRequest.Priority, subject, body, adminToNotify?.displayname, record.Username, adminToNotify?.mail, changeRequest.Id, null, implementationFinalApprovalResponse.Id, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                    }
                }
            }
            return RedirectToAction("Details", new { id = implementationFinalApprovalResponse.ChangeRequestId, tab = "FinalApprovals" });
        }

        // This closes out 'Implementation' stage and moves to 'Closeout/Complete' stage
        public async Task<IActionResult> CloseoutImplementation(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            changeRequest.Change_Status = "Closeout";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Closeout").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.Now;
            changeRequest.Implementation_Approval_Date = DateTime.Now;
            changeRequest.Implementation_Username = _username;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email all admins with 'Approver' rights that this Change Request has been submitted for Implementation....
            var adminApproverList = await _context.Administrators.Where(m => m.Approver == true).ToListAsync();
            __mst_employee admin = new __mst_employee();
            string subject;
            string body;
            foreach (var record in adminApproverList)
            {
                admin = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == record.Username.ToLower()).FirstOrDefaultAsync();
                subject = @"Management of Change (MoC) - Submitted for Closeout";
                body = @"Change Request has been submitted for Closeout. All post-implementation tasks will need to be completed to move forward. Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                Initialization.EmailProviderSmtp.SendMessage(subject, body, admin?.mail, null, null, changeRequest.Priority);
                AddEmailHistory(changeRequest.Priority, subject, body, admin?.displayname, record.Username, admin?.mail, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
            }

            // Email all employees that MoC Writer wants notified that this has been approved for implementation so they are aware of the change....
            if (changeRequest.Additional_Notification != null && changeRequest.Additional_Notification.Count > 0)
            {
                foreach (var username in changeRequest.Additional_Notification)
                {
                    var employeeToNotify = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == username.ToLower()).FirstOrDefaultAsync();
                    subject = @"Management of Change (MoC) - Submitted for Implementation";
                    body = @"Change Request has been submitted for implementation. This is for notification purposes only. This may affect your job process, so please read through change request. Follow link below and review the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                    // Send Email...
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, employeeToNotify?.mail, null, null, changeRequest.Priority);

                    // Log that Email was Sent...
                    AddEmailHistory(changeRequest.Priority, subject, body, employeeToNotify?.displayname, username, employeeToNotify?.mail, changeRequest.Id, null, null, null, "ChangeRequest", changeRequest.Change_Status, DateTime.Now, _username);
                }
            }

            // Create a task for the ChangeRequest Owner to notify Administrator when ChangeRequest has been fully implemented
            var mocOwner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == changeRequest.Change_Owner.ToLower()).FirstOrDefaultAsync();
            Models.Task task = new Models.Task
            {
                ChangeRequestId = changeRequest.Id,
                MocNumber = changeRequest.MOC_Number,
                ImplementationType = @"Post",
                Status = @"Open",
                Priority = changeRequest.Priority,
                AssignedByUser = changeRequest.Change_Owner,
                AssignedToUser = changeRequest.Change_Owner,
                AssignedByUserEmail = mocOwner?.mail,
                AssignedByUserFullName = mocOwner?.displayname,
                Title = @"Implementation Completion Notification",
                Description = @"Notify MoC admin when this MoC is completely implemented.",
                DueDate = DateTime.Now.AddMonths(1),
                CreatedUser = changeRequest.Change_Owner,
                CreatedDate = DateTime.Now
            };
            _context.Add(task);
            await _context.SaveChangesAsync();

            // Send Email Out notifying the person who is assigned the task
            subject = @"Management of Change (MoC) - Task Assigned.";
            body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>Task Title: </strong>" + task.Title + @"<br/><strong>Task Description: </strong>" + task.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedToUser.ToLower()).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, null, null, task.Priority);
                AddEmailHistory(task.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, task.ChangeRequestId, null, null, task.Id, "Task", task.Status, DateTime.Now, task.CreatedUser);
            }

            return RedirectToAction("Details", new { id = changeRequest.Id, tab = "Implementation" });
        }

        // This Closes-Out/completes the Change Request
        public async Task<IActionResult> CloseoutComplete(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            changeRequest.Change_Status = "Closed";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Closed").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.Now;
            changeRequest.Closeout_Date = DateTime.Now;
            changeRequest.Closeout_Username = _username;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email the ChangeRequst Owner that the change request has been closed
            var owner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == changeRequest.Change_Owner.ToLower()).FirstOrDefaultAsync();
            string subject = @"Management of Change (MoC) - Change Request Completed/Closed";
            string body = @"Change Request has been Closed-Out/Completed.<br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

            Initialization.EmailProviderSmtp.SendMessage(subject, body, owner?.mail, null, null, changeRequest.Priority);
            AddEmailHistory(changeRequest.Priority, subject, body, owner?.displayname, changeRequest.Change_Owner, owner?.mail, changeRequest.Id, null, null, null, "Task", changeRequest.Change_Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = changeRequest.Id, tab = "CloseoutComplete" });
        }

        public async Task<IActionResult> CreateTask(int changeRequestId)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            Models.Task task = new Models.Task
            {
                ChangeRequestId = changeRequestId,
                AssignedByUser = _username,
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            ViewBag.Users = getUserList();
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask([Bind("Id,ChangeRequestId,ImplementationType,MocNumber,Status,Priority,AssignedToUser,AssignedByUser,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (task.DueDate == null)
                ModelState.AddModelError("DueDate", "Must Include a Valid Completion Date");

            if (task.DueDate < DateTime.Today)
                ModelState.AddModelError("DueDate", "Date Cannot Be In The Past");

            if (task.Status == "On Hold" && String.IsNullOrWhiteSpace(task.OnHoldReason))
                ModelState.AddModelError("OnHoldReason", "If Task Status is 'On Hold', Reason is required.");

            if (task.ChangeRequestId != null && task.ImplementationType == null)
                ModelState.AddModelError("ImplementationType", "If task is part of a Change Request, Implementation Type is required.");

            if (task.ChangeRequestId != null)
            {
                // cannot create a Task for a Change Request after it is at a certain status....
                ChangeRequest changeRequest = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).FirstOrDefaultAsync();

                if (changeRequest != null)
                {
                    if ((task.ImplementationType == "Pre") &&
                        (changeRequest.Change_Status == "Closeout" || changeRequest.Change_Status == "Closed"))
                        ModelState.AddModelError("ImplementationType", "Task Status is beyond the stage to add a Pre Implementation Task.");
                    if ((task.ImplementationType == "Post") &&
                        (changeRequest.Change_Status == "Closed"))
                        ModelState.AddModelError("ImplementationType", "Task Status is beyond the stage to add a Post Implementation Task.");
                    if (changeRequest.Change_Status == "Cancelled")
                        ModelState.AddModelError("ImplementationType", "Cannot Create a Task under a Cancelled Change Request.");
                }
            }

            if (ModelState.IsValid)
            {
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();

                // get assigned-to person info....
                var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedToUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedToUserFullName = toPerson?.displayname;
                task.AssignedToUserEmail = toPerson?.mail;

                // get assigned-by person info....
                var fromPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedByUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedByUserFullName = fromPerson?.displayname;
                task.AssignedByUserEmail = fromPerson?.mail;

                _context.Add(task);
                await _context.SaveChangesAsync();

                // Send Email Out notifying the person who is assigned the task
                string subject = @"Management of Change (MoC) - Impact Assessment Response Task Assigned.";
                string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>MoC Title: </strong>" + task.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                if (toPerson != null)
                {
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson.mail, null, null, task.Priority);
                    AddEmailHistory(task.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, task.ChangeRequestId, null, null, task.Id, "Task", task.Status, DateTime.Now, _username);
                }
                return RedirectToAction("Details", "ChangeRequests", new { Id = task.ChangeRequestId, Tab = "Tasks" });
            }
            ViewBag.Users = getUserList();

            return View(task);
        }
    }
}
