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
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeRequestsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        //private readonly string AttachmentDirectory = @"C:\Applications\ManagementOfChange";
        private readonly string AttachmentDirectory = @"\\aub1vdev-app01\ManagementOfChange\";
        public ChangeRequestsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
            //string username = User.Identity.Name != null ? User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1) : Environment.UserName;
        }

        // GET: ChangeRequests
        public async Task<IActionResult> Index(string timestampFilter)
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

            //ViewBag.UserName2 = Environment.UserDomainName;
            //ViewBag.UserName3 = Environment.UserName;
            //ViewBag.UserName6 = WindowsIdentity.GetCurrent().Name;
            //ViewBag.UserName7 = WindowsIdentity.GetCurrent().Owner;
            //ViewBag.UserName8 = WindowsIdentity.GetCurrent().User;
            //ViewBag.UserName9 = WindowsIdentity.GetCurrent().AccessToken;
            //ViewBag.UserName10 = WindowsIdentity.GetCurrent().Actor;
            //ViewBag.UserName11 = WindowsIdentity.GetCurrent().AuthenticationType;

            //ViewBag.UserName16 = WindowsIdentity.GetCurrent().ImpersonationLevel;
            //ViewBag.UserName17 = WindowsIdentity.GetCurrent().IsAnonymous;
            //ViewBag.UserName18 = WindowsIdentity.GetCurrent().IsAuthenticated;
            //ViewBag.UserName19 = WindowsIdentity.GetCurrent().IsGuest;
            //ViewBag.UserName20 = WindowsIdentity.GetCurrent().IsSystem;

            //ViewBag.UserName26 = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //ViewBag.UserName27 = User.FindFirstValue(ClaimTypes.Name);
            //ViewBag.UserName28 = User.Identity.Name;

            ////Request.ServerVariables["LOGON_USER"]
            //ViewBag.UserName29 = Request.HttpContext.User?.Identity?.Name;
            //ViewBag.UserName30 = Environment.GetEnvironmentVariable("USERNAME");

            //AppDomain appDomain = Thread.GetDomain();
            //appDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            //WindowsPrincipal windowsPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            //ViewBag.UserName31 = windowsPrincipal.Identity.Name;
            //ViewBag.UserName32 = User.Identity.Name != null ? User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1) : Environment.UserName;

            ViewBag.activeRecordList = new List<String> { "Current", "Deleted", "All" };
            //ViewBag.activeRecordList2 = new List<String> { "All","Current","Deleted" };

            var requests = from m in _context.ChangeRequest
                           select m;

            switch (timestampFilter)
            {
                case null:
                    requests = requests.Where(r => r.DeletedDate == null);
                    break;
                case "All":
                    break;
                case "Current":
                    requests = requests.Where(r => r.DeletedDate == null);
                    break;
                case "Deleted":
                    requests = requests.Where(r => r.DeletedDate != null);
                    break;
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View("Index", await requests.OrderBy(r => r.Id).ToListAsync());
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

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);

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

            changeRequestViewModel.ChangeRequest = changeRequest;
            changeRequestViewModel.Tasks = tasks;
            // disable tab 3 if General MOC Responses have not been completed...
            int countGMR = changeRequest.GeneralMocResponses.Where(m => m.Response == null).Count();
            changeRequestViewModel.Tab3Disabled = countGMR > 0 || changeRequestViewModel.ChangeRequest.Change_Status == "Draft" ? "disabled" : "";
            // disable tab4 (final review) if any Impact Assessment Responses have not been completed...
            int countIAR = changeRequest.ImpactAssessmentResponses.Where(m => m.ReviewCompleted == false).Count();
            changeRequestViewModel.Tab4Disabled = countIAR > 0 ? "disabled" : "";

            changeRequestViewModel.TabActiveDetail = "";
            changeRequestViewModel.TabActiveGeneralMocQuestions = "";
            changeRequestViewModel.TabActiveImpactAssessments = "";
            changeRequestViewModel.TabActiveFinalApprovals = "";
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

            // Get all attachments
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(AttachmentDirectory, changeRequest.MOC_Number));

            if (!Directory.Exists(Path.Combine(AttachmentDirectory, changeRequest.MOC_Number)))
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
            }
            changeRequestViewModel.Attachments = attachments.OrderBy(m => m.Name).ToList();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequestViewModel);
        }

        // GET: ChangeRequests/Create
        public async Task<IActionResult> Create()
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

            // Persist Dropdown Selection Lists
            ViewBag.Levels = await _context.ChangeLevel.OrderBy(m => m.Order).Select(m => m.Level).ToListAsync();
            //ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            if (ModelState.IsValid)
            {
                // make sure valid Username
                ErrorViewModel errorViewModel = CheckAuthorization();
                if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                    return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

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
                        ReviewType review = _context.ReviewType.Where(m => m.Type == assessment.ReviewType).FirstOrDefault();
                        if (review != null)
                        {
                            ImpactAssessmentResponse response = new ImpactAssessmentResponse
                            {
                                ReviewType = assessment.ReviewType,
                                ChangeType = assessment.ChangeType,
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

                        if (IARQuestions != null && IARQuestions.Count > 0)
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
                                CreatedUser = _username,
                                CreatedDate = DateTime.UtcNow
                            };
                            changeRequest.ImplementationFinalApprovalResponses.Add(response);
                        }
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

                DirectoryInfo path = new DirectoryInfo(Path.Combine(AttachmentDirectory, changeRequest.MOC_Number));
                if (!Directory.Exists(Path.Combine(AttachmentDirectory, changeRequest.MOC_Number)))
                    path.Create();

                return RedirectToAction("Details", new { id = changeRequest.Id });
            }
            return View(changeRequest);
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
            ViewBag.Levels = await _context.ChangeLevel.OrderBy(m => m.Order).Select(m => m.Level).ToListAsync();
            ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Tab = tab;

            // Create Dropdown List of Users...
            var userList = await _context.__mst_employee
                .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                .Where(m => m.accountenabled == true)
                .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                .OrderBy(m => m.displayname)
                .ThenBy(m => m.onpremisessamaccountname)
                .ToListAsync();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (var user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.onpremisessamaccountname, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };

                if (user.onpremisessamaccountname == changeRequest.Change_Owner)
                    item.Selected = true;
                users.Add(item);
            }
            ViewBag.Users = users;


            return View(changeRequest);
        }

        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MOC_Number,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Request_Date,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id != changeRequest.Id)
                return NotFound();

            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(changeRequest);
        }


        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMocQuestions(int id, [Bind("ChangeRequest, Tab3Disabled, Tab4Disabled, TabActiveDetail, TabActiveGeneralMocQuestions, TabActiveImpactAssessments, TabActiveFinalApprovals")] ChangeRequestViewModel changeRequestViewModel)
        {
            foreach (GeneralMocResponses record in changeRequestViewModel.ChangeRequest.GeneralMocResponses)
            {
                record.ModifiedUser = _username;
                record.ModifiedDate = DateTime.UtcNow;
                _context.Update(record);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = changeRequestViewModel.ChangeRequest.Id, tab = "GeneralMocQuestions" });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForReview(int id, ChangeRequestViewModel changeRequestViewModel)
        {
            if (id == null || id == 0)
                return NotFound();

            // Get the Change Request
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
                return NotFound();

            // Get all the General MOC Responses associated with this request...
            changeRequest.GeneralMocResponses = await _context.GeneralMocResponses.Where(m => m.ChangeRequestId == id).ToListAsync();

            // Get all the Impact Assessment Responses associated with this request...
            changeRequest.ImpactAssessmentResponses = await _context.ImpactAssessmentResponse.Where(m => m.ChangeRequestId == id).ToListAsync();

            // Get all the Impact Assessment Responses Questions/Answers associated with this request...
            if (changeRequest.ImpactAssessmentResponses.Any())
            {
                foreach (var record in changeRequest.ImpactAssessmentResponses)
                {
                    record.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer.Where(m => m.ImpactAssessmentResponseId == record.Id).ToListAsync();
                }
            }

            // Get all the Final Approval Responses associated with this request...
            changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse.Where(m => m.ChangeRequestId == id).ToListAsync();

            changeRequest.Change_Status = "Submitted for Impact Assessment Review";
            changeRequest.ModifiedUser = _username;
            changeRequest.ModifiedDate = DateTime.UtcNow;

            _context.Update(changeRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tab = "GeneralMocQuestions" });
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
                changeRequest.Change_Status = "Killed";
                changeRequest.DeletedUser = _username;
                changeRequest.DeletedDate = DateTime.UtcNow;
                _context.Update(changeRequest);
                //_context.ChangeRequest.Remove(changeRequest);
            }

            await _context.SaveChangesAsync();
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

            string filePath = Path.Combine(AttachmentDirectory, changeRequest.MOC_Number, fileAttachment.FileName);
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
    }
}
