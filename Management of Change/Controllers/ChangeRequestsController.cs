using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using Management_of_Change.Utilities;
using Syroot.Windows.IO;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Security.AccessControl;
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
            //string username = User.Identity.Name != null ? User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1) : Environment.UserName;
        }

        // GET: ChangeRequests
        public async Task<IActionResult> Index(string statusFilter)
        {
            // TEST MJWII // ChangeRequest = 4 // ImpactAssessmentResponse = 1 // ImpactAssessmentResponseAnswers = 1-16
            // see if all of this ImpactAssessmentResponses have questions answered from reviewer.  If so, mark as complete...
            //            bool found = await _context.ImpactAssessmentResponseAnswer.Where(m => m.ImpactAssessmentResponseId == 1).Where(m => m.Action != null).AnyAsync();
            // here we set the ImpactAssessmentResponse for the reviewer as 'Complete'

            // see if all of this ChangeRequests ImpactAssessmentResponses are complete.  If so, promote/change status of the ChangeRequest...
            //            bool found2 = await _context.ImpactAssessmentResponse.Where(m => m.ChangeRequestId == 4).Where(m => m.ReviewCompleted != true).AnyAsync();
            // here we would set the status of the request to 'Awaiting Completion of Pre-Implementation Tasks'
            // END TEST MJWII

            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            // Create Dropdown List of Status...
            var statusList = await _context.ChangeStatus.OrderBy(m => m.Order).ToListAsync();
            List<SelectListItem> statusDropdown = new List<SelectListItem>();
            foreach (var status in statusList)
            {
                SelectListItem item = new SelectListItem { Value = status.Description, Text = status.Description};
                statusDropdown.Add(item);
            }
            ViewBag.StatusList = statusDropdown;

            var requests = from m in _context.ChangeRequest
                           select m;
            requests = requests.Where(r => r.DeletedDate == null);

            switch (statusFilter)
            {
                case null:
                    break;
                case "All":
                    break;
                default:
                    requests = requests.Where(m => m.Change_Status == statusFilter);
                    break;
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View("Index", await requests
                .OrderBy(m => m.Priority)
                .ThenBy(m => m.Estimated_Completion_Date)
                .ToListAsync());
        }

        // GET: ChangeRequests/Details/5
        public async Task<IActionResult> Details(int? id, string? tab = "Details", string fileAttachmentError = null, string fileDownloadMessage = null)
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

            changeRequestViewModel.ChangeRequest = changeRequest;
            changeRequestViewModel.Tasks = tasks;
            // disable tab 3 (ImpactAssessments) if General MOC Responses have not been completed...
            int countGMR = changeRequest.GeneralMocResponses.Where(m => m.Response == null).Count();
            changeRequestViewModel.Tab3Disabled = countGMR > 0 || changeRequestViewModel.ChangeRequest.Change_Status == "Draft" ? "disabled" : "";
            // disable tab4 (Final Review) if any Impact Assessment Responses have not been completed...
            int countIAR = changeRequest.ImpactAssessmentResponses.Where(m => m.ReviewCompleted == false).Count();
            changeRequestViewModel.Tab4Disabled = countIAR > 0 || (changeRequestViewModel.ChangeRequest.Change_Status == "Draft" || changeRequestViewModel.ChangeRequest.Change_Status == "ImpactAssessmentReview") ? "disabled" : "";
            // disable tab5 (Implementation) if any Final Approvals have not been completed...
            int countFA = changeRequest.ImplementationFinalApprovalResponses.Where(m => m.ReviewCompleted == false).Count();
            changeRequestViewModel.Tab5Disabled = countFA > 0 || (changeRequestViewModel.ChangeRequest.Change_Status == "Draft" || changeRequestViewModel.ChangeRequest.Change_Status == "ImpactAssessmentReview" || changeRequestViewModel.ChangeRequest.Change_Status == "FinalApprovals") ? "disabled" : "";
            // disable tab6 (Closeout/Complete) if change request is not in status of "Closeout" or "Closed"
            changeRequestViewModel.Tab6Disabled = changeRequest.Change_Status != "Closeout" && changeRequest.Change_Status != "Closed" ? "disabled" : "";

            changeRequestViewModel.TabActiveDetail = "";
            changeRequestViewModel.TabActiveGeneralMocQuestions = "";
            changeRequestViewModel.TabActiveImpactAssessments = "";
            changeRequestViewModel.TabActiveFinalApprovals = "";
            changeRequestViewModel.TabActiveImplementation = "";
            changeRequestViewModel.TabActiveCloseoutComplete = "";
            changeRequestViewModel.TabActiveAttachments = "";
            changeRequestViewModel.TabActiveTasks = "";

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
                case "ImpactAssessments":
                    changeRequestViewModel.TabActiveImpactAssessments = "active";
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
            List<ViewModels.Attachment> attachments = new List<ViewModels.Attachment>();
            foreach (FileInfo i in Files)
            {
                ViewModels.Attachment attachment = new ViewModels.Attachment
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

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequestViewModel);
        }

        // GET: ChangeRequests/Create
        public async Task<IActionResult> Create(string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ChangeRequest changeRequest = new ChangeRequest
            {
                Change_Owner = _username,
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            changeRequest.Change_Status = await _context.ChangeStatus.OrderByDescending(cs => cs.Default).ThenBy(cs => cs.Order).ThenBy(cs => cs.Id).Select(cs => cs.Status).FirstOrDefaultAsync();
            changeRequest.Change_Status_Description = await _context.ChangeStatus.OrderByDescending(cs => cs.Default).ThenBy(cs => cs.Order).ThenBy(cs => cs.Id).Select(cs => cs.Description).FirstOrDefaultAsync();

            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
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
        public async Task<IActionResult> Create([Bind("Id,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,Cancel_Username,Cancel_Date,Cancel_Reason,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest, string source=null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Completion Date");

            if (changeRequest.Estimated_Completion_Date < DateTime.Today)
                ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
                ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

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
                            CreatedDate = DateTime.UtcNow
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

                if (source == "Home")
                    return RedirectToAction("Index","Home", new {});
                else
                    return RedirectToAction("Details", new { id = changeRequest.Id });
            }

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
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

            changeRequest.Change_Status = "Draft";
            changeRequest.Change_Owner = _username;
            changeRequest.Estimated_Completion_Date = null;
            changeRequest.CreatedUser = _username;
            changeRequest.CreatedDate = DateTime.UtcNow;
            changeRequest.ModifiedDate = null;
            changeRequest.ModifiedUser = null;
            changeRequest.DeletedDate = null;
            changeRequest.DeletedUser = null;
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(cs => cs.Status == changeRequest.Change_Status).Select(cs => cs.Description).FirstOrDefaultAsync();

            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
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
        public async Task<IActionResult> CloneCreate([Bind("Id,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest, int clonedId, string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Completion Date");

            if (changeRequest.Estimated_Completion_Date < DateTime.Today)
                ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
                ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

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
                            CreatedDate = DateTime.UtcNow
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

                if (source == "Home")
                    return RedirectToAction("Index", "Home", new { });
                else
                    return RedirectToAction("Details", new { id = changeRequest.Id });
            }

            // Persist Dropdown Selection Lists
            ViewBag.Types = getChangeTypes();
            ViewBag.Levels = getChangeLevels();
            ViewBag.PTNs = getPtnNumbers();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,MOC_Number,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Change_Status_Description,Priority,Request_Date,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,PTN_Number,Waiver_Number,CMT_Number,Implementation_Approval_Date,Implementation_Username,Closeout_Date,Closeout_Username,Cancel_Username,Cancel_Date,Cancel_Reason,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id != changeRequest.Id)
                return NotFound();

            if (changeRequest.Estimated_Completion_Date == null)
                ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Completion Date");

            // Dont check completion date on edit because if it was orignally put in fine, keep the original date pristine, dont make them have to change it!
            //if (changeRequest.Estimated_Completion_Date < DateTime.Today)
            //    ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
                ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

            if (ModelState.IsValid)
            {
                try
                {
                    changeRequest.ModifiedUser = _username;
                    changeRequest.ModifiedDate = DateTime.UtcNow;
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
            ViewBag.Types = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
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

            foreach (GeneralMocResponses record in changeRequestViewModel.ChangeRequest.GeneralMocResponses)
            {
                record.ModifiedUser = _username;
                record.ModifiedDate = DateTime.UtcNow;
                _context.Update(record);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = changeRequestViewModel.ChangeRequest.Id, tab = "GeneralMocQuestions" });
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
                changeRequest.DeletedDate = DateTime.UtcNow;
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

        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, tab = "Attachments" });
        }

        // This closes out 'GeneralMocQuestions' and moves to 'ImpactAssessments' stage
        public async Task<IActionResult> SubmitForReview(int id, ChangeRequestViewModel changeRequestViewModel)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            // add Impact Assessment Responses
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
                    // Get ALL Review Types setup for this Change Type ...
                    List<ReviewType> reviews = await _context.ReviewType
                        .Where(rt => (rt.Type == assessment.ReviewType && rt.ChangeArea == null) || (rt.Type == assessment.ReviewType && rt.ChangeArea == changeRequest.Area_of_Change))                        
                        .ToListAsync();
                    foreach (var review in reviews)
                    {
                        ImpactAssessmentResponse response = new ImpactAssessmentResponse
                        {
                            ReviewType = assessment.ReviewType,
                            ChangeType = assessment.ChangeType,
                            ChangeArea = review.ChangeArea,
                            Reviewer = review.Reviewer,
                            ReviewerEmail = review.Email,
                            Username = review.Username,
                            CreatedUser = _username,
                            CreatedDate = DateTime.UtcNow
                        };
                        changeRequest.ImpactAssessmentResponses.Add(response);
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
                                ReviewType = record.ReviewType,
                                Question = question.Question,
                                Order = question.Order,
                                CreatedUser = _username,
                                CreatedDate = DateTime.UtcNow
                            };
                            record.ImpactAssessmentResponseAnswers.Add(rec);  //NEED TO INSTANTIATE HERE!!!
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
                            FinalReviewType = assessment.FinalReviewType,
                            ChangeType = assessment.ChangeType,
                            Reviewer = review.Reviewer,
                            ReviewerEmail = review.Email,
                            Username = review.Username,
                            CreatedUser = _username,
                            CreatedDate = DateTime.UtcNow
                        };
                        changeRequest.ImplementationFinalApprovalResponses.Add(response);
                    }
                }
            }

            // Update ChangeRequest...
            changeRequest.Change_Status = "ImpactAssessmentReview";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "ImpactAssessmentReview").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.UtcNow;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email All Users ImpactResponse Review/Approval links...
            foreach (var record in changeRequest.ImpactAssessmentResponses)
            {
                string subject = @"Management of Change (MoC) - Impact Assessment Response Needed";
                string body = @"Your Impact Assessment Response review is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                Initialization.EmailProviderSmtp.SendMessage(subject, body, record.ReviewerEmail, null, null, changeRequest.Priority);

                EmailHistory emailHistory = new EmailHistory
                {
                    Subject = subject,
                    Body = body,
                    SentToDisplayName = record.Reviewer,
                    SentToUsername = record.Username,
                    SentToEmail = record.ReviewerEmail,
                    ChangeRequestId = changeRequest.Id,
                    ImpactAssessmentResponseId = record.Id,
                    Type = "ChangeRequest",
                    Status = changeRequest.Change_Status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = _username
                };
                _context.Add(emailHistory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = id, tab = "GeneralMocQuestions" });
        }

        // This closes out 'ImpactAssessments' and moves to 'FinalApprovals' stage
        public async Task<IActionResult> MarkImpactAssessmentComplete(int id, string tab = "ImpactAssessments")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return NotFound();

            // get ImpactAssessmentResponse record...
            ImpactAssessmentResponse impactAssessmentResponse = await _context.ImpactAssessmentResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (impactAssessmentResponse == null)
                return NotFound();

            // Mark the Impact Assessment Response as complete...
            impactAssessmentResponse.ReviewCompleted = true;
            impactAssessmentResponse.DateCompleted = DateTime.UtcNow;
            impactAssessmentResponse.ModifiedUser = _username;
            impactAssessmentResponse.ModifiedDate = DateTime.UtcNow;
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

                changeRequest.Change_Status = "FinalApprovals";
                changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "FinalApprovals").Select(m => m.Description).FirstOrDefaultAsync();
                changeRequest.ModifiedDate = DateTime.UtcNow;
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
                    string body = @"Your Final Approval/Review is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, record.ReviewerEmail, null, null, changeRequest.Priority);

                    EmailHistory emailHistory = new EmailHistory
                    {
                        Subject = subject,
                        Body = body,
                        SentToDisplayName = record.Reviewer,
                        SentToUsername = record.Username,
                        SentToEmail = record.ReviewerEmail,
                        ChangeRequestId = changeRequest.Id,
                        ImplementationFinalApprovalResponseId = record.Id,
                        Type = "ChangeRequest",
                        Status = changeRequest.Change_Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUser = _username
                    };
                    _context.Add(emailHistory);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Details", new { id = impactAssessmentResponse.ChangeRequestId, tab = "ImpactAssessments" });
        }

        // This closes out 'FinalApprovals' and moves to 'Implementation' phase
        public async Task<IActionResult> CompleteFinalReview(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return NotFound();

            // Get the FinalApproval record....
            ImplementationFinalApprovalResponse implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (implementationFinalApprovalResponse == null)
                return NotFound();

            implementationFinalApprovalResponse.ReviewCompleted = true;
            implementationFinalApprovalResponse.DateCompleted = DateTime.UtcNow;
            implementationFinalApprovalResponse.ModifiedDate = DateTime.UtcNow;
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
                    changeRequest.ModifiedDate = DateTime.UtcNow;
                    changeRequest.ModifiedUser = _username;
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();

                    // Email all admins with 'Approver' rights that this Change Request has been submitted for Implementation....
                    // TODO MJWII
                    var adminApproverList = await _context.Administrators.Where(m => m.Approver == true).ToListAsync();
                    foreach (var record in adminApproverList)
                    {
                        var admin = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == record.Username).FirstOrDefaultAsync();
                        string subject = @"Management of Change (MoC) - Submitted for Implementation";
                        string body = @"Change Request has been submitted for implementation. All pre-implementation tasks will need to be completed to move forward. Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, admin.mail, null, null, changeRequest.Priority);

                        EmailHistory emailHistory = new EmailHistory
                        {
                            Subject = subject,
                            Body = body,
                            SentToDisplayName = admin.displayname,
                            SentToUsername = record.Username,
                            SentToEmail = admin.mail,
                            ChangeRequestId = changeRequest.Id,
                            Type = "ChangeRequest",
                            Status = changeRequest.Change_Status,
                            ImplementationFinalApprovalResponseId = implementationFinalApprovalResponse.Id,
                            CreatedDate = DateTime.UtcNow,
                            CreatedUser = _username
                        };
                        _context.Add(emailHistory);
                        await _context.SaveChangesAsync();
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

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            changeRequest.Change_Status = "Closeout";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Closeout").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.UtcNow;
            changeRequest.Implementation_Approval_Date = DateTime.UtcNow;
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
                admin = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == record.Username).FirstOrDefaultAsync();
                subject = @"Management of Change (MoC) - Submitted for Closeout";
                body = @"Change Request has been submitted for Closeout. All post-implementation tasks will need to be completed to move forward. Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                Initialization.EmailProviderSmtp.SendMessage(subject, body, admin.mail, null, null, changeRequest.Priority);

                EmailHistory emailHistory = new EmailHistory
                {
                    Subject = subject,
                    Body = body,
                    SentToDisplayName = admin.displayname,
                    SentToUsername = record.Username,
                    SentToEmail = admin.mail,
                    ChangeRequestId = changeRequest.Id,
                    Type = "ChangeRequest",
                    Status = changeRequest.Change_Status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = _username
                };
                _context.Add(emailHistory);
                await _context.SaveChangesAsync();
            }

            // Create a task for the ChangeRequest Owner to notify Administrator when ChangeRequest has been fully implemented
            Models.Task task = new Models.Task
            {
                ChangeRequestId = changeRequest.Id,
                MocNumber = changeRequest.MOC_Number,
                ImplementationType = @"Post",
                Status = @"Open",
                Priority = changeRequest.Priority,
                AssignedByUser = changeRequest.Change_Owner,
                AssignedToUser = changeRequest.Change_Owner,
                Title = @"Implementation Completion Notification",
                Description = @"Notify MoC admin when this MoC is completely implemented.",
                DueDate = DateTime.UtcNow.AddMonths(1),
                CreatedUser = changeRequest.Change_Owner,
                CreatedDate = DateTime.UtcNow
            };
            _context.Add(task);
            await _context.SaveChangesAsync();

            // Send Email Out notifying the person who is assigned the task
            subject = @"Management of Change (MoC) - Task Assigned.";
            body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>Task Title: </strong>" + task.Title + @"<br/><strong>Task Description: </strong>" + task.Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == task.AssignedToUser).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson.mail, null, null, task.Priority);

                EmailHistory emailHistory = new EmailHistory
                {
                    Subject = subject,
                    Body = body,
                    SentToDisplayName = toPerson.displayname,
                    SentToUsername = toPerson.onpremisessamaccountname,
                    SentToEmail = toPerson.mail,
                    ChangeRequestId = task.ChangeRequestId,
                    TaskId = task.Id,
                    Type = "Task",
                    Status = task.Status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = task.CreatedUser
                };
                _context.Add(emailHistory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = changeRequest.Id, tab = "Implementation" });
        }

        // This Closes-Out/completes the Change Request
        public async Task<IActionResult> CloseoutComplete(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            changeRequest.Change_Status = "Closed";
            changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Closed").Select(m => m.Description).FirstOrDefaultAsync();
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.UtcNow;
            changeRequest.Closeout_Date = DateTime.UtcNow;
            changeRequest.Closeout_Username = _username;
            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            // Email the ChangeRequst Owner that the change request has been closed
            var owner = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == changeRequest.Change_Owner).FirstOrDefaultAsync();
            string subject = @"Management of Change (MoC) - Change Request Completed/Closed";
            string body = @"Change Request has been Closed-Out/Completed.<br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, owner.mail, null, null, changeRequest.Priority);

            EmailHistory emailHistory = new EmailHistory
            {
                Subject = subject,
                Body = body,
                SentToDisplayName = owner.displayname,
                SentToUsername = changeRequest.Change_Owner,
                SentToEmail = owner.mail,
                ChangeRequestId = changeRequest.Id,
                Type = "ChangeRequest",
                Status = changeRequest.Change_Status,
                CreatedDate = DateTime.UtcNow,
                CreatedUser = _username
            };
            _context.Add(emailHistory);
            await _context.SaveChangesAsync();

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
                CreatedDate = DateTime.UtcNow
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

            if (task.DueDate == null)
                ModelState.AddModelError("DueDate", "Must Include a Completion Date");

            if (task.DueDate < DateTime.Today)
                ModelState.AddModelError("DueDate", "Date Cannot Be In The Past");

            if (task.Status == "On Hold" && String.IsNullOrWhiteSpace(task.OnHoldReason))
                ModelState.AddModelError("OnHoldReason", "If Task Status is 'On Hold', Reason is required.");

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

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
                _context.Add(task);
                await _context.SaveChangesAsync();

                // Send Email Out notifying the person who is assigned the task
                string subject = @"Management of Change (MoC) - Impact Assessment Response Task Assigned.";
                string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>MoC Title: </strong>" + task.Title + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == task.AssignedToUser).FirstOrDefaultAsync();
                if (toPerson != null)
                {
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson.mail, null, null, task.Priority);

                    EmailHistory emailHistory = new EmailHistory
                    {
                        Subject = subject,
                        Body = body,
                        SentToDisplayName = toPerson.displayname,
                        SentToUsername = toPerson.onpremisessamaccountname,
                        SentToEmail = toPerson.mail,
                        ChangeRequestId = task.ChangeRequestId,
                        TaskId = task.Id,
                        Type = "Task",
                        Status = task.Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUser = _username
                    };
                    _context.Add(emailHistory);
                    await _context.SaveChangesAsync();
                }                
                return RedirectToAction("Details", "ChangeRequests", new { Id = task.ChangeRequestId, Tab = "Tasks" });
            }
            ViewBag.Users = getUserList();

            return View(task);
        }
    }
}
