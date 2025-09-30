using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.Utilities;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class WaiversController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public WaiversController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: Waivers
        public async Task<IActionResult> Index(string statusFilter, string prevStatusFilter = null, string sort = null, string prevSort = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // if no filter selected, keep previous
            if (statusFilter == null)
                statusFilter = prevStatusFilter;
            ViewBag.StatusList = getStatusFilter(statusFilter);

            // Get
            var waivers = await _contextPtnWaiver.Waiver
                .Where(m => m.DeletedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .ThenBy(m => m.WaiverNumber)
                .ToListAsync();

            switch (statusFilter)
            {
                case null:
                    ViewBag.PrevStatusFilter = "All";
                    break;
                case "All":
                    ViewBag.PrevStatusFilter = "All";
                    break;
                default:
                    waivers = waivers.Where(m => m.Status == statusFilter).ToList();
                    ViewBag.PrevStatusFilter = statusFilter;
                    break;
            }

            return View(waivers);
        }

        // GET: Waivers/Details/5
        public async Task<IActionResult> Details(int? id, string tab = "Waiver", string tabWaiver = "Details", string fileAttachmentError = null, string rejectedReason = null, string saveMessageMaterialDetail = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            Waiver waiver = await _contextPtnWaiver.Waiver
                .Include(w => w.WaiverQuestionResponse)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiver?.WaiverQuestionResponse != null)
            {
                waiver.WaiverQuestionResponse = waiver.WaiverQuestionResponse
                    .OrderBy(r => int.TryParse(r.Order, out var n) ? n : int.MaxValue)
                    .ThenBy(r => r.Question)
                    .ToList();
            }

            PTN ptn = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == waiver.PTNId);

            if (waiver == null || ptn == null)
                return NotFound();

            var groupApproversReviews = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == waiver.Id && m.SourceTable == "Waiver").OrderBy(m => m.Group).ToListAsync();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.RejectedReason = rejectedReason;
            ViewBag.Employees = getUserList();
            ViewBag.SaveMessageMaterialDetail = saveMessageMaterialDetail;

            WaiverViewModel waiverVM = new WaiverViewModel();
            waiverVM.FileAttachmentError = fileAttachmentError;
            waiverVM.Waiver = waiver;
            waiverVM.Ptn = ptn;
            waiverVM.GroupApproversReview = groupApproversReviews;

            waiverVM.TabActiveDetail = "";
            waiverVM.TabActiveAttachmentsWaiver = "";
            waiverVM.TabActiveWaiverApproval = "";
            waiverVM.TabActiveWaiverAdminApproval = "";
            waiverVM.TabActiveWaiverMaterialDetails = "";

            switch (tabWaiver)
            {
                case null:
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "":
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "Details":
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "AttachmentsWaiver":
                    waiverVM.TabActiveAttachmentsWaiver = "active";
                    break;
                case "WaiverApproval":
                    waiverVM.TabActiveWaiverApproval = "active";
                    break;
                case "WaiverAdminApproval":
                    waiverVM.TabActiveWaiverAdminApproval = "active";
                    break;
                case "WaiverMaterialDetails":
                    waiverVM.TabActiveWaiverMaterialDetails = "active";
                    break;
            }

            // GET ALL ATTACHMENTS FOR WAIVER /////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            string waiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            DirectoryInfo pathWaiver = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber)))
                pathWaiver.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] FilesWaiver = pathWaiver.GetFiles();

            // Display the file names
            List<Attachment> attachments = new List<Attachment>();
            foreach (FileInfo i in FilesWaiver)
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
            waiverVM.AttachmentsWaiver = attachments.OrderBy(m => m.Name).ToList();

            // GET ALL ATTACHMENTS FOR Waiver Material Details /////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo pathWaiverMaterialDetails = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber)))
                pathWaiverMaterialDetails.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] FilesWaiverMaterialDetails = pathWaiverMaterialDetails.GetFiles();

            // Display the file names
            List<Attachment> attachmentsWaiverMaterialDetails = new List<Attachment>();
            foreach (FileInfo i in FilesWaiverMaterialDetails)
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
                attachmentsWaiverMaterialDetails.Add(attachment);
            }
            waiverVM.AttachmentsWaiverMaterialDetail = attachmentsWaiverMaterialDetails.OrderBy(m => m.Name).ToList();

            // Render Tabs Disabled/Enabled
            // Submit for Admin Approval Tab...
            int incompleteWaiverQuestionCount = _contextPtnWaiver.WaiverQuestionResponse.Where(m => m.WaiverId == waiver.Id && m.Response == null).Count();
            waiverVM.TabSubmitWaiverForApprovalDisabled = waiverVM.AttachmentsWaiver.Count == 0 || incompleteWaiverQuestionCount > 0 ? "disabled" : "";
            // Admin Approve Waiver Tab...
            waiverVM.TabApproveWaiverDisabled = waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Approved" || waiverVM.Waiver.Status == "Closed" || waiverVM.Waiver.Status == "Rejected" ? "" : "disabled";
            waiverVM.TabWaiverMaterialDetailsDisabled = waiverVM.Ptn.isWaferingDepartment == true ? "" : "disabled";
            if (waiverVM.Waiver.Status == "Draft" || waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Rejected")
                waiverVM.TabWaiverMaterialDetailsDisabled = "disabled";

            // Per Ian Manning, do not allow anyone to change an attachment after Draft (because the PTN has been approved in the documents state it is in) but allow to upload new documents.
            //if (waiverVM.Waiver.Status != "Draft")
            //    ViewBag.DisableAttachmentUpload = "disabled";

            //return RedirectToAction("Details", "Waivers", new { id = waiver.PTNId, tab = "Waivers" });
            return View(waiverVM);
        }

        public async Task<IActionResult> MesLookup(string id, string tab = "Waiver", string tabWaiver = "Details", string fileAttachmentError = null, string rejectedReason = null, string saveMessageMaterialDetail = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            // Format: https://localhost:7214/Waivers/MesLookup?id=2025-1000-W01-R02
            Waiver waiver = await _contextPtnWaiver.Waiver
                .Where(m => m.ExternalIdMes == id)  //.Where(m => m.WaiverNumber == docId)
                .Include(w => w.WaiverQuestionResponse)
                .AsNoTracking()
                .OrderByDescending(m => m.RevisionNumber)
                .FirstOrDefaultAsync();

            if (waiver?.WaiverQuestionResponse != null)
            {
                waiver.WaiverQuestionResponse = waiver.WaiverQuestionResponse
                    .OrderBy(r => int.TryParse(r.Order, out var n) ? n : int.MaxValue)
                    .ThenBy(r => r.Question)
                    .ToList();
            }

            PTN ptn = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == waiver.PTNId);

            if (waiver == null || ptn == null)
                return NotFound();

            var groupApproversReviews = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == waiver.Id && m.SourceTable == "Waiver").OrderBy(m => m.Group).ToListAsync();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.RejectedReason = rejectedReason;
            ViewBag.Employees = getUserList();
            ViewBag.SaveMessageMaterialDetail = saveMessageMaterialDetail;

            WaiverViewModel waiverVM = new WaiverViewModel();
            waiverVM.FileAttachmentError = fileAttachmentError;
            waiverVM.Waiver = waiver;
            waiverVM.Ptn = ptn;
            waiverVM.GroupApproversReview = groupApproversReviews;

            waiverVM.TabActiveDetail = "";
            waiverVM.TabActiveAttachmentsWaiver = "";
            waiverVM.TabActiveWaiverApproval = "";
            waiverVM.TabActiveWaiverAdminApproval = "";
            waiverVM.TabActiveWaiverMaterialDetails = "";
            switch (tabWaiver)
            {
                case null:
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "":
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "Details":
                    waiverVM.TabActiveDetail = "active";
                    break;
                case "AttachmentsWaiver":
                    waiverVM.TabActiveAttachmentsWaiver = "active";
                    break;
                case "WaiverApproval":
                    waiverVM.TabActiveWaiverApproval = "active";
                    break;
                case "WaiverAdminApproval":
                    waiverVM.TabActiveWaiverAdminApproval = "active";
                    break;
                case "WaiverMaterialDetails":
                    waiverVM.TabActiveWaiverMaterialDetails = "active";
                    break;
            }

            // GET ALL ATTACHMENTS FOR Waiver ///////////////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            string waiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            DirectoryInfo pathWaiver = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber)))
                pathWaiver.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = pathWaiver.GetFiles();

            // Display the file names
            List<Attachment> attachments = new List<Attachment>();
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
            }
            waiverVM.AttachmentsWaiver = attachments.OrderBy(m => m.Name).ToList();

            // GET ALL ATTACHMENTS FOR Waiver Material Details /////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo pathWaiverMaterialDetails = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber)))
                pathWaiverMaterialDetails.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] FilesWaiverMaterialDetails = pathWaiverMaterialDetails.GetFiles();

            // Display the file names
            List<Attachment> attachmentsWaiverMaterialDetails = new List<Attachment>();
            foreach (FileInfo i in FilesWaiverMaterialDetails)
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
                attachmentsWaiverMaterialDetails.Add(attachment);
            }
            waiverVM.AttachmentsWaiverMaterialDetail = attachmentsWaiverMaterialDetails.OrderBy(m => m.Name).ToList();

            // Render Tabs Disabled/Enabled
            // Submit for Admin Approval Tab...
            waiverVM.TabSubmitWaiverForApprovalDisabled = waiverVM.AttachmentsWaiver.Count == 0 ? "disabled" : "";
            // Admin Approve Waiver Tab...
            waiverVM.TabApproveWaiverDisabled = waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Approved" || waiverVM.Waiver.Status == "Closed" || waiverVM.Waiver.Status == "Rejected" ? "" : "disabled";
            waiverVM.TabWaiverMaterialDetailsDisabled = waiverVM.Ptn.isWaferingDepartment == true ? "" : "disabled";
            if (waiverVM.Waiver.Status == "Draft" || waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Rejected")
                waiverVM.TabWaiverMaterialDetailsDisabled = "disabled";

            // Per Ian Manning, do not allow anyone to change an attachment after Draft (because the PTN has been approved in the documents state it is in) but allow to upload new documents.
            //if (waiverVM.Waiver.Status != "Draft")
            //    ViewBag.Disable = "disabled";

            //return RedirectToAction("Details", "Waivers", new { id = waiver.PTNId, tab = "Waivers" });
            return View("Details", waiverVM);
        }

        // GET: Waivers/Create
        public async Task<IActionResult> Create(int ptnId, string tab = "Details")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            var userInfo = getUserInfo(_username);
            if (userInfo == null)
            {
                errorViewModel = new ErrorViewModel() { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = "Invalid Username: " + _username });
            }

            PTN ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == ptnId);
            if (ptn == null)
                return NotFound();

            ViewBag.Status = getWaiverStatus();
            ViewBag.ProductProcess = getProductProcess();
            ViewBag.Areas = getAreas();
            ViewBag.PtnGroupApprovers = ptn.GroupApprover;

            var groups = getGroupApprovers();
            foreach (SelectListItem group in groups)
            {
                bool found = ptn.GroupApprover.Contains(group.Value);  // Prefill Waivers groupApprovers with whatever PTN's groupApprovers are
                if (found == true)
                {
                    group.Selected = true;
                    group.Disabled = true;
                }
            }
            ViewBag.Groups = groups;

            Waiver waiver = new Waiver()
            {
                PTNId = ptnId,
                PtnDocId = ptn.DocId,
                DateSequence = ptn.OriginatorYear + "-" + ptn.SerialNumber,
                Status = "Draft",
                CreatedDate = DateTime.Now,
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail
            };

            // Get Waiver Questions that writer needs to answer...
            var waiverQuestions = await _contextPtnWaiver.WaiverQuestion.Where(m => m.DeletedDate == null).OrderBy(m => m.Order).ToListAsync();

            waiver.WaiverQuestionResponse = new List<WaiverQuestionResponse>();

            foreach (var question in waiverQuestions)
            {
                var WaiverQuestionResponse = new WaiverQuestionResponse
                {
                    GroupApprover = question.GroupApprover,
                    Question = question.Question,
                    Response = null,
                    Order = question.Order,
                    CreatedUser = waiver.CreatedUser,
                    CreatedUserFullName = waiver.CreatedUserFullName,
                    CreatedUserEmail = waiver.CreatedUserEmail,
                    CreatedDate = DateTime.Now
                };
                waiver.WaiverQuestionResponse.Add(WaiverQuestionResponse);
            }

            return View(waiver);
        }

        // POST: Waivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Waiver waiver)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !string.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            waiver.IsMostCurrentWaiver = true;

            if (ModelState.IsValid)
            {
                waiver.WaiverSequence = getWaiverSerialNumber(waiver.PtnDocId);
                waiver.RevisionNumber = 0;
                waiver.ExternalIdMes = $"{waiver.DateSequence}-{waiver.WaiverSequence}-R{waiver.RevisionNumber:00}";
                waiver.WaiverNumber = $"{waiver.PtnDocId}-{waiver.WaiverSequence}-R{waiver.RevisionNumber:00}";

                // Create attachments directory
                var waiverNumber = waiver.WaiverNumber;
                var waiverDir = Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber);
                if (!Directory.Exists(waiverDir))
                    Directory.CreateDirectory(waiverDir);

                // Optional: ensure child rows have audit fields, etc.
                if (waiver.WaiverQuestionResponse != null)
                {
                    foreach (var q in waiver.WaiverQuestionResponse)
                    {
                        // EF will set WaiverId automatically because the child is in the parent's collection.
                        // Set any defaults if needed:
                        q.CreatedUser ??= waiver.CreatedUser;
                        q.CreatedUserFullName ??= waiver.CreatedUserFullName;
                        q.CreatedUserEmail ??= waiver.CreatedUserEmail;
                        q.CreatedDate = waiver.CreatedDate == null ? DateTime.Now : q.CreatedDate;
                    }
                }

                _contextPtnWaiver.Add(waiver);
                await _contextPtnWaiver.SaveChangesAsync();

                return RedirectToAction("Details", "Waivers", new { id = waiver.Id, tab = "Waivers" });
            }

            // Rebuild ViewBags on validation failure (unchanged from yours) ...
            PTN ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == waiver.PTNId);
            if (ptn == null) return NotFound();

            ViewBag.Status = getWaiverStatus();
            ViewBag.ProductProcess = getProductProcess();
            ViewBag.Areas = getAreas();
            ViewBag.PtnGroupApprovers = ptn.GroupApprover;

            var groups = getGroupApprovers();
            foreach (SelectListItem group in groups)
            {
                bool found = waiver.GroupApprover?.Contains(group.Value) == true;
                if (found)
                {
                    group.Selected = true;
                    group.Disabled = true;
                }
            }
            ViewBag.Groups = groups;

            return View(waiver);
        }

        // GET: Waivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Ptns = getPtns();
            ViewBag.Status = getWaiverStatus();
            //ViewBag.PorProjects = getPorProjects();
            ViewBag.ProductProcess = getProductProcess();
            ViewBag.Areas = getAreas();
            //ViewBag.Groups = getGroupApprovers();

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            Waiver waiver = await _contextPtnWaiver.Waiver
                .Include(w => w.WaiverQuestionResponse)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiver?.WaiverQuestionResponse != null)
            {
                waiver.WaiverQuestionResponse = waiver.WaiverQuestionResponse
                    .OrderBy(r => int.TryParse(r.Order, out var n) ? n : int.MaxValue)
                    .ThenBy(r => r.Question)
                    .ToList();
            }

            if (waiver == null)
                return NotFound();

            var groups = getGroupApprovers();
            foreach (SelectListItem group in groups)
            {
                bool found = waiver.GroupApprover.Contains(group.Value);
                if (found == true)
                {
                    group.Selected = true;
                    group.Disabled = true;
                }
            }
            ViewBag.Groups = groups;

            return View(waiver);
        }

        // POST: Waivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ptnId, Waiver model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Ptns = getPtns();
                ViewBag.Status = getWaiverStatus();
                ViewBag.ProductProcess = getProductProcess();
                ViewBag.Areas = getAreas();

                var groups = getGroupApprovers();
                foreach (SelectListItem group in groups)
                {
                    bool found = model.GroupApprover.Contains(group.Value);
                    if (found == true)
                    {
                        group.Selected = true;
                        group.Disabled = true;
                    }
                }
                ViewBag.Groups = groups;
                return View(model);
            }

            // Load the current waiver + children from DB
            var waiver = await _contextPtnWaiver.Waiver
                .Include(w => w.WaiverQuestionResponse)
                .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (waiver == null) return NotFound();

            // Update simple fields
            waiver.Description = model.Description;
            waiver.ProductProcess = model.ProductProcess;
            waiver.Areas = model.Areas;
            waiver.GroupApprover = model.GroupApprover;

            var userInfo = getUserInfo(_username);
            waiver.ModifiedUser = _username;
            if (userInfo != null)
            {
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
            }

            // Update question responses (match by Question + Order, or by index if you prefer)
            if (model.WaiverQuestionResponse != null && waiver.WaiverQuestionResponse != null)
            {
                foreach (var posted in model.WaiverQuestionResponse)
                {
                    var existing = waiver.WaiverQuestionResponse
                        .FirstOrDefault(x =>
                            string.Equals(x.Question, posted.Question, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(x.Order, posted.Order, StringComparison.OrdinalIgnoreCase));

                    if (existing != null)
                    {
                        // Only response is editable on Edit; GroupApprover/Question/Order are metadata
                        existing.Response = string.IsNullOrWhiteSpace(posted.Response) ? null : posted.Response;
                        existing.ModifiedUser = waiver.ModifiedUser;
                        existing.ModifiedUserFullName = waiver.ModifiedUserFullName;
                        existing.ModifiedUserEmail = waiver.ModifiedUserEmail;
                        existing.ModifiedDate = DateTime.UtcNow;
                    }
                }
            }

            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction("Details", "Waivers", new { id = waiver.Id, tab = "Details" });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,RevisionNumber,WaiverNumber,WaiverSequence,DateSequence,ExternalIdMes,PorProject,Areas,GroupApprover,Description,ProductProcess,Status,DateClosed,CorrectiveActionDueDate,PTNId,PtnDocId,IsMostCurrentWaiver,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Waiver waiver)
        //{
        //    ErrorViewModel errorViewModel = CheckAuthorization();
        //    if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
        //        return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

        //    ViewBag.IsAdmin = _isAdmin;
        //    ViewBag.Username = _username;

        //    if (id != waiver.Id)
        //        return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // attachment storage file path should already exist, but just make sure....
        //            string waiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
        //            DirectoryInfo pathWaiver = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber));
        //            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber)))
        //                pathWaiver.Create();

        //            var userInfo = getUserInfo(_username);
        //            if (userInfo != null)
        //            {
        //                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
        //                waiver.ModifiedUserFullName = userInfo.displayname;
        //                waiver.ModifiedUserEmail = userInfo.mail;
        //                waiver.ModifiedDate = DateTime.Now;
        //            }

        //            waiver.PtnDocId = await _contextPtnWaiver.PTN.Where(m => m.Id == waiver.PTNId).Select(m => m.DocId).FirstOrDefaultAsync();
        //            _contextPtnWaiver.Update(waiver);
        //            await _contextPtnWaiver.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!WaiverExists(waiver.Id))
        //                return NotFound();
        //            else
        //                throw;
        //        }
        //        return RedirectToAction("Details", "Waivers", new { id = waiver.Id, tab = "Details" });
        //        //return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.Ptns = getPtns();
        //    ViewBag.Status = getWaiverStatus();
        //    //ViewBag.PorProjects = getPorProjects();
        //    ViewBag.ProductProcess = getProductProcess();
        //    ViewBag.Areas = getAreas();
        //    //ViewBag.Groups = getGroupApprovers();

        //    var groups = getGroupApprovers();
        //    foreach (SelectListItem group in groups)
        //    {
        //        bool found = waiver.GroupApprover.Contains(group.Value);
        //        if (found == true)
        //        {
        //            group.Selected = true;
        //            group.Disabled = true;
        //        }

        //    }
        //    ViewBag.Groups = groups;

        //    return View(waiver);
        //}

        // GET: Waivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiver == null)
                return NotFound();

            return View(waiver);
        }

        // POST: Waivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.Waiver == null)
                return Problem("Entity set 'PtnWaiverContext.Waiver'  is null.");

            var waiver = await _contextPtnWaiver.Waiver.FindAsync(id);
            Waiver deletedWaiver = waiver;

            if (waiver != null)
                _contextPtnWaiver.Waiver.Remove(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // try to find previous waiver and if found activate it...
            var previousWaiver = await _contextPtnWaiver.Waiver.Where(m => m.DateSequence == deletedWaiver.DateSequence && m.WaiverSequence == deletedWaiver.WaiverSequence).OrderByDescending(m => m.RevisionNumber).FirstOrDefaultAsync();

            if (previousWaiver != null)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    previousWaiver.ModifiedUser = userInfo.onpremisessamaccountname;
                    previousWaiver.ModifiedUserFullName = userInfo.displayname;
                    previousWaiver.ModifiedUserEmail = userInfo.mail;
                    previousWaiver.ModifiedDate = DateTime.Now;
                }
                previousWaiver.IsMostCurrentWaiver = true;
                _contextPtnWaiver.Update(previousWaiver);
                await _contextPtnWaiver.SaveChangesAsync();
            }
            return RedirectToAction("Details", "PTNs", new { id = deletedWaiver.PTNId, tab = "Waivers" });
        }

        private bool WaiverExists(int id)
        {
            return (_contextPtnWaiver.Waiver?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver", fileAttachmentError = "No File Has Been Selected For Upload" });

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return RedirectToAction("Index");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _contextPtnWaiver.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            // attachment storage file path should already exist, but just make sure....
            string waiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber)))
                path.Create();

            string filePath = Path.Combine(Initialization.AttachmentDirectoryWaiver, waiverNumber, fileAttachment.FileName);

            if (waiver.Status != "Draft" && System.IO.File.Exists(filePath))
            {
                // return message to user saying they cannot overwrite a document after Waiver has been approved.  You can only upload new documents.
                return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver", fileAttachmentError = "Cannot Overwrite Existing Attachment. Past Draft Mode. Must Create New Attachment." });
            }
            else
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileAttachment.CopyToAsync(fileStream);
                }
            }
            return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver" });
        }

        [HttpPost]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SaveFileMaterialDetails(int id, IFormFile? fileAttachment)
        {
            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails", fileAttachmentError = "No File Has Been Selected For Upload" });

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return RedirectToAction("Index");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _contextPtnWaiver.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            // attachment storage file path should already exist, but just make sure....
            string waiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber)))
                path.Create();

            string filePath = Path.Combine(Initialization.AttachmentDirectoryWaiverMaterialDetail, waiverNumber, fileAttachment.FileName);

            //if (waiver.Status != "Draft" && System.IO.File.Exists(filePath))
            //{
            //    // return message to user saying they cannot overwrite a document after Waiver has been approved.  You can only upload new documents.
            //    return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails", fileAttachmentError = "Cannot Overwrite Existing Attachment. Past Draft Mode. Must Create New Attachment." });
            //}
            //else
            //{
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }
            //}
            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails", saveMessageMaterialDetail = "File has been uploaded" });
        }

        public async Task<IActionResult> DownloadFile(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }
        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver" });
        }
        public async Task<IActionResult> DeleteFileWaiverMaterialDetails(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails" });
        }

        public async Task<IActionResult> SubmitWaiverForApproval(int id)
        {
            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver
                .Include(w => w.WaiverQuestionResponse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return RedirectToAction("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
                waiver.SubmittedForApprovalUser = userInfo.onpremisessamaccountname;
                waiver.SubmittedForApprovalUserFullName = userInfo.displayname;
                waiver.SubmittedForApprovalDate = DateTime.Now;
            }
            waiver.Status = "Pending Approval";

            // Setup email Primary and Secondary Approvers to review and Approve/Reject (for below)...
            string subject = @"Process Test Notification (PTN) - Waiver Review Needed";
            string body = @"Your Review is needed. Please follow link below and review/respond to the following Waiver request. <br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";

            // Create the GroupApproverReviews based on user's selected GroupApprovers for this Waiver...
            foreach (var approver in waiver.GroupApprover)
            {
                GroupApproversReview groupApproversReview = new GroupApproversReview();
                var record = await _contextPtnWaiver.GroupApprovers.Where(m => m.Group == approver).FirstOrDefaultAsync();
                if (record != null)
                {
                    groupApproversReview.SourceId = waiver.Id;
                    groupApproversReview.SourceTable = "Waiver";
                    groupApproversReview.Group = record.Group;
                    groupApproversReview.PrimaryApproverUsername = record.PrimaryApproverUsername;
                    groupApproversReview.PrimaryApproverFullName = record.PrimaryApproverFullName;
                    groupApproversReview.PrimaryApproverEmail = record.PrimaryApproverEmail;
                    groupApproversReview.PrimaryApproverTitle = record.PrimaryApproverTitle;
                    groupApproversReview.SecondaryApproverUsername = record.SecondaryApproverUsername;
                    groupApproversReview.SecondaryApproverFullName = record.SecondaryApproverFullName;
                    groupApproversReview.SecondaryApproverEmail = record.SecondaryApproverEmail;
                    groupApproversReview.SecondaryApproverTitle = record.SecondaryApproverTitle;
                    groupApproversReview.CreatedDate = DateTime.Now;
                    groupApproversReview.CreatedUser = userInfo.onpremisessamaccountname != null ? userInfo.onpremisessamaccountname : "";
                    groupApproversReview.CreatedUserFullName = userInfo.displayname != null ? userInfo.displayname : "";
                    groupApproversReview.CreatedUserEmail = userInfo.mail != null ? userInfo.mail : "";

                    _contextPtnWaiver.GroupApproversReview.Add(groupApproversReview);
                    await _contextPtnWaiver.SaveChangesAsync();

                    Initialization.EmailProviderSmtp.SendMessage(subject, body, groupApproversReview.PrimaryApproverEmail, groupApproversReview.SecondaryApproverEmail, null, null);
                    AddEmailHistory(null, subject, body, groupApproversReview.PrimaryApproverFullName, groupApproversReview.PrimaryApproverUsername, groupApproversReview.PrimaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                    if (groupApproversReview.PrimaryApproverEmail != null)
                        AddEmailHistory(null, subject, body, groupApproversReview.SecondaryApproverFullName, groupApproversReview.SecondaryApproverUsername, groupApproversReview.SecondaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                }
            }

            // Create the GroupApproverReviews based on user's responses to the Waiver Questions that have GroupApprovers associated with them...
            var waiverQuestionsWithGroupApprovers = waiver.WaiverQuestionResponse.Where(m => m.WaiverId == waiver.Id && m.Response == "Yes").ToList();
            foreach (var record in waiverQuestionsWithGroupApprovers)
            {
                foreach (var group in record.GroupApprover)
                {
                    // make sure the group does not already exist in the GroupApproverReviews for this Waiver. If it doesnt, add it...
                    var exists = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == waiver.Id && m.SourceTable == "Waiver" && m.Group == group).AnyAsync();

                    // add the group approver review because it does not already exist for this Waiver...
                    if (!exists)
                    {
                        GroupApproversReview groupApproversReview = new GroupApproversReview();
                        var groupToAdd = await _contextPtnWaiver.GroupApprovers.Where(m => m.Group == group).FirstOrDefaultAsync();
                        if (record != null)
                        {
                            groupApproversReview.SourceId = waiver.Id;
                            groupApproversReview.SourceTable = "Waiver";
                            groupApproversReview.Group = groupToAdd.Group;
                            groupApproversReview.PrimaryApproverUsername = groupToAdd.PrimaryApproverUsername;
                            groupApproversReview.PrimaryApproverFullName = groupToAdd.PrimaryApproverFullName;
                            groupApproversReview.PrimaryApproverEmail = groupToAdd.PrimaryApproverEmail;
                            groupApproversReview.PrimaryApproverTitle = groupToAdd.PrimaryApproverTitle;
                            groupApproversReview.SecondaryApproverUsername = groupToAdd.SecondaryApproverUsername;
                            groupApproversReview.SecondaryApproverFullName = groupToAdd.SecondaryApproverFullName;
                            groupApproversReview.SecondaryApproverEmail = groupToAdd.SecondaryApproverEmail;
                            groupApproversReview.SecondaryApproverTitle = groupToAdd.SecondaryApproverTitle;
                            groupApproversReview.CreatedDate = DateTime.Now;
                            groupApproversReview.CreatedUser = userInfo.onpremisessamaccountname != null ? userInfo.onpremisessamaccountname : "";
                            groupApproversReview.CreatedUserFullName = userInfo.displayname != null ? userInfo.displayname : "";
                            groupApproversReview.CreatedUserEmail = userInfo.mail != null ? userInfo.mail : "";

                            _contextPtnWaiver.GroupApproversReview.Add(groupApproversReview);
                            await _contextPtnWaiver.SaveChangesAsync();

                            Initialization.EmailProviderSmtp.SendMessage(subject, body, groupApproversReview.PrimaryApproverEmail, groupApproversReview.SecondaryApproverEmail, null, null);
                            AddEmailHistory(null, subject, body, groupApproversReview.PrimaryApproverFullName, groupApproversReview.PrimaryApproverUsername, groupApproversReview.PrimaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                            if (groupApproversReview.PrimaryApproverEmail != null)
                                AddEmailHistory(null, subject, body, groupApproversReview.SecondaryApproverFullName, groupApproversReview.SecondaryApproverUsername, groupApproversReview.SecondaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                        }
                    }
                }
            }

            // add group/approver mandatory waiver approvers.....
            var mandatoryWaiverApproverGroups = _contextPtnWaiver.GroupApprovers.Where(m => m.WaiverMandatoryApproval == true).ToList();
            foreach (var record in mandatoryWaiverApproverGroups)
            {
                // make sure the group does not already exist in the GroupApproverReviews for this Waiver. If it doesnt, add it...
                var exists = await _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == waiver.Id && m.SourceTable == "Waiver" && m.Group == record.Group).AnyAsync();
                // add the group approver review because it does not already exist for this Waiver...
                if (!exists)
                {
                    GroupApproversReview groupApproversReview = new GroupApproversReview();
                    groupApproversReview.SourceId = waiver.Id;
                    groupApproversReview.SourceTable = "Waiver";
                    groupApproversReview.Group = record.Group;
                    groupApproversReview.PrimaryApproverUsername = record.PrimaryApproverUsername;
                    groupApproversReview.PrimaryApproverFullName = record.PrimaryApproverFullName;
                    groupApproversReview.PrimaryApproverEmail = record.PrimaryApproverEmail;
                    groupApproversReview.PrimaryApproverTitle = record.PrimaryApproverTitle;
                    groupApproversReview.SecondaryApproverUsername = record.SecondaryApproverUsername;
                    groupApproversReview.SecondaryApproverFullName = record.SecondaryApproverFullName;
                    groupApproversReview.SecondaryApproverEmail = record.SecondaryApproverEmail;
                    groupApproversReview.SecondaryApproverTitle = record.SecondaryApproverTitle;
                    groupApproversReview.CreatedDate = DateTime.Now;
                    groupApproversReview.CreatedUser = userInfo.onpremisessamaccountname != null ? userInfo.onpremisessamaccountname : "";
                    groupApproversReview.CreatedUserFullName = userInfo.displayname != null ? userInfo.displayname : "";
                    groupApproversReview.CreatedUserEmail = userInfo.mail != null ? userInfo.mail : "";

                    _contextPtnWaiver.GroupApproversReview.Add(groupApproversReview);
                    await _contextPtnWaiver.SaveChangesAsync();

                    Initialization.EmailProviderSmtp.SendMessage(subject, body, groupApproversReview.PrimaryApproverEmail, groupApproversReview.SecondaryApproverEmail, null, null);
                    AddEmailHistory(null, subject, body, groupApproversReview.PrimaryApproverFullName, groupApproversReview.PrimaryApproverUsername, groupApproversReview.PrimaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                    if (groupApproversReview.PrimaryApproverEmail != null)
                        AddEmailHistory(null, subject, body, groupApproversReview.SecondaryApproverFullName, groupApproversReview.SecondaryApproverUsername, groupApproversReview.SecondaryApproverEmail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                }
            }

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }

        [HttpPost, ActionName("RejectWaiver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWaiver([Bind("Id,RejectedReason", Prefix = "Ptn")] PTN ptnIn, [Bind("Id,RejectedReason", Prefix = "Waiver")] Waiver waiverIn)
        {
            if (waiverIn.Id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == waiverIn.Id);
            if (waiver == null)
                return RedirectToAction("Index");

            if (waiverIn.RejectedReason == null)
                return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverApproval", rejectedReason = "If Waiver is Rejected, Rejected Reason is Required" });

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
                waiver.SubmittedForApprovalUser = userInfo.onpremisessamaccountname;
                waiver.SubmittedForApprovalUserFullName = userInfo.displayname;
                waiver.SubmittedForApprovalDate = DateTime.Now;
            }
            waiver.RejectedReason = waiverIn.RejectedReason;
            waiver.RejectedBeforeSubmission = true;
            waiver.Status = "Rejected";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email Waiver creator to notify of Waiver Rejection....
            var personRejecting = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (_username ?? string.Empty).ToLower()).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Rejected";
            string body = @"Your Waiver has been <span style=""color:red"">rejected</span> by " + personRejecting.displayname + "." +
                "<br/><br/><strong>Reason Rejected: </strong>" + waiver.RejectedReason + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personRejecting.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverApproval" });
        }

        public async Task<IActionResult> ApproveWaiver(int id)
        {
            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return RedirectToAction("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
                waiver.ApprovedByUser = userInfo.onpremisessamaccountname;
                waiver.ApprovedByUserFullName = userInfo.displayname;
                waiver.ApprovedByDate = DateTime.Now;
            }
            waiver.Status = "Approved";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email Waiver creator that the Waiver was Approved....
            var personApproving = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (_username ?? string.Empty).ToLower()).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Approved by Admin";
            string body = @"Your Waiver has been <span style=""color:green"">approved</span> by " + personApproving.displayname + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personApproving.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWaiverApprover([Bind("Id,RejectedReason", Prefix = "Ptn")] PTN ptnIn, [Bind("Id,RejectedReason", Prefix = "Waiver")] Waiver waiverIn)
        {
            if (waiverIn.Id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == waiverIn.Id);
            if (waiver == null)
                return RedirectToAction("Index");

            if (waiverIn.RejectedReason == null)
                return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverAdminApproval", rejectedReason = "If Waiver is Rejected, Rejected Reason is Required" });

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
                waiver.ApprovedByUser = userInfo.onpremisessamaccountname;
                waiver.ApprovedByUserFullName = userInfo.displayname;
                waiver.ApprovedByDate = DateTime.Now;
            }
            waiver.RejectedReason = waiverIn.RejectedReason;
            waiver.RejectedByApprover = true;
            waiver.Status = "Rejected";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email Waiver creator to notify of Waiver Rejection....
            var personRejecting = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (_username ?? string.Empty).ToLower()).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Rejected";
            string body = @"Your Waiver has been <span style=""color:red"">rejected</span> by " + personRejecting.displayname + "." +
                "<br/><br/><strong>Reason Rejected: </strong>" + waiver.RejectedReason + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personRejecting.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverAdminApproval" });
        }

        public async Task<IActionResult> Revise(int id, string? tab = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return View("Index");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Waiver waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return View("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
            }
            waiver.RejectedReason = "Waiver Revised";
            waiver.IsMostCurrentWaiver = false;
            //waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            waiver.Id = 0;
            waiver.DateClosed = null;
            waiver.RevisionNumber += 1;
            waiver.ExternalIdMes = waiver.DateSequence + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            waiver.WaiverNumber = waiver.PtnDocId + "-" + waiver.WaiverSequence + "-R" + waiver.RevisionNumber.ToString("###00");
            waiver.IsMostCurrentWaiver = true;
            waiver.RejectedBeforeSubmission = false;
            waiver.RejectedByApprover = false;
            waiver.SubmittedForApprovalDate = null;
            waiver.SubmittedForApprovalUser = null;
            waiver.SubmittedForApprovalUserFullName = null;
            waiver.ApprovedByDate = null;
            waiver.ApprovedByUser = null;
            waiver.ApprovedByUserFullName = null;
            waiver.CompletedByDate = null;
            waiver.CompletedBylUser = null;
            waiver.CompletedBylUserFullName = null;
            waiver.CreatedDate = DateTime.Now;
            waiver.CreatedUser = userInfo.onpremisessamaccountname;
            waiver.CreatedUserFullName = userInfo.displayname;
            waiver.CreatedUserEmail = userInfo.mail;
            waiver.ModifiedUser = null;
            waiver.ModifiedUserFullName = null;
            waiver.ModifiedUserEmail = null;
            waiver.ModifiedDate = null;
            waiver.RejectedReason = null;
            waiver.Status = "Draft";

            _contextPtnWaiver.Add(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", "PTNs", new { id = waiver.PTNId, tab = "Waivers" });
        }

        public async Task<IActionResult> CloseWaiver(int id)
        {
            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return RedirectToAction("Index", "Home");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
                waiver.CompletedBylUser = userInfo.onpremisessamaccountname;
                waiver.CompletedBylUserFullName = userInfo.displayname;
                waiver.CompletedByDate = DateTime.Now;
                waiver.DateClosed = DateTime.Now;
            }
            waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email Waiver creator to notify of PTN Rejection....
            var personClosing = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (_username ?? string.Empty).ToLower()).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Closed";
            string body = @"Your Waiver has been <span style=""color:green"">closed</span> by " + personClosing.displayname + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Title: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personClosing.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", "PTNs", new { id = waiver.PTNId, tab = "Waivers" });
        }

        public async Task<IActionResult> GroupApprove(int waiverId, int groupApproverId, string status /*, [Bind("Id,RejectedReason")] PTN pTN*/)
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (groupApproverId == null)
                return NotFound();

            var groupApprove = await _contextPtnWaiver.GroupApproversReview.FirstOrDefaultAsync(m => m.Id == groupApproverId);
            if (groupApprove == null)
                return NotFound();

            groupApprove.Status = status;

            var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == groupApprove.SourceId);
            if (waiver == null)
                return NotFound();

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == waiver.PTNId);
            if (ptn == null)
                return NotFound();

            //if (pTN.RejectedReason == null)
            //    return RedirectToAction("Details", new { id = pTN.Id, tab = "PtnApproval", rejectedReason = "If PTN is Rejected, Rejected Reason is Required" });

            var groupApproverReviewVM = new GroupApproverReviewVM();
            groupApproverReviewVM.Waiver = waiver;
            groupApproverReviewVM.PTN = ptn;
            groupApproverReviewVM.GroupApproversReview = groupApprove;

            return View("GroupApproveComment", groupApproverReviewVM);

            //return RedirectToAction("Details", new { id = ptnId, tab = "GroupApprove" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupApprove([Bind("Id,SourceId,SourceTable,Group,Status,PrimaryApproverUsername,PrimaryApproverFullName,PrimaryApproverEmail,PrimaryApproverTitle,SecondaryApproverUsername,SecondaryApproverFullName,SecondaryApproverEmail,SecondaryApproverTitle,ReviewedBy,ReviewDate,Comment,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate", Prefix = "GroupApproversReview")] GroupApproversReview groupApproversReview)
        {
            {
                ViewBag.IsAdmin = _isAdmin;
                ViewBag.Username = _username;

                if (groupApproversReview.Id == null)
                    return NotFound();

                if (groupApproversReview.Comment == null && groupApproversReview.Status == "Rejected")
                    ModelState.AddModelError("groupApproversReview.Comment", "ERROR: If Rejecting, Comment is Required");

                if (ModelState.IsValid)
                {
                    var userInfo = getUserInfo(_username);
                    if (userInfo != null)
                    {
                        groupApproversReview.ReviewedBy = userInfo.displayname;
                        groupApproversReview.ReviewDate = DateTime.Now;
                        groupApproversReview.Comment = groupApproversReview.Comment;
                        groupApproversReview.ModifiedUser = userInfo.onpremisessamaccountname;
                        groupApproversReview.ModifiedUserFullName = userInfo.displayname;
                        groupApproversReview.ModifiedUserEmail = userInfo.mail;
                        groupApproversReview.ModifiedDate = DateTime.Now;
                    }
                    _contextPtnWaiver.Update(groupApproversReview);
                    await _contextPtnWaiver.SaveChangesAsync();

                    // if rejected, we need to reject the Waiver and send email to Waiver owner that it has been rejected and why....
                    if (groupApproversReview.Status == "Rejected")
                    {
                        var waiverRec = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == groupApproversReview.SourceId);
                        if (waiverRec == null)
                            return RedirectToAction("Index");

                        waiverRec.Status = "Rejected";
                        waiverRec.RejectedReason = groupApproversReview.Comment;
                        waiverRec.RejectedByApprover = true;
                        waiverRec.ModifiedDate = DateTime.Now;
                        waiverRec.ModifiedUser = _username;
                        waiverRec.ModifiedUserFullName = userInfo.displayname;
                        waiverRec.ModifiedUserEmail = userInfo.mail;

                        _contextPtnWaiver.Update(waiverRec);
                        await _contextPtnWaiver.SaveChangesAsync();

                        // email Waiver creator that the Waiver was Rejected....
                        string subject = @"Process Test Notification (PTN) - Waiver Rejected";
                        string body = @"Your Waiver has been <span style=""color:red"">Rejected</span> by " + userInfo.displayname + ". <br/><br/><strong>Waiver#: </strong>" + waiverRec.WaiverNumber + @"<br/><strong>PTN Title: </strong>" + waiverRec.Description + "<br/><strong>Rejected Reason: " + groupApproversReview.Comment + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, waiverRec.CreatedUserEmail, null, null, null);
                        AddEmailHistory(null, subject, body, waiverRec.CreatedUserFullName, waiverRec.CreatedUser, waiverRec.CreatedUserEmail, null, waiverRec.Id, null, "Waiver", waiverRec.Status, DateTime.Now, _username);
                    }

                    // See if all Reviews have been approved. If they have been, automatically Approve Waiver ....
                    int count = _contextPtnWaiver.GroupApproversReview.Where(m => m.SourceId == groupApproversReview.SourceId && m.SourceTable == "Waiver" && m.Status != "Approved").Count();
                    if (count == 0)
                    {
                        var waiverRec = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == groupApproversReview.SourceId);
                        if (waiverRec == null)
                            return RedirectToAction("Index");

                        waiverRec.ModifiedUser = "System";
                        waiverRec.ModifiedUserFullName = "System";
                        waiverRec.ModifiedUserEmail = "System";
                        waiverRec.ModifiedDate = DateTime.Now;
                        waiverRec.ApprovedByUser = "System";
                        waiverRec.ApprovedByUserFullName = "System";
                        waiverRec.ApprovedByDate = DateTime.Now;
                        waiverRec.Status = "Approved";

                        _contextPtnWaiver.Update(waiverRec);
                        await _contextPtnWaiver.SaveChangesAsync();

                        // email Waiver creator that the Waiver was Approved....
                        //var personApproving = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                        string subject = @"Process Test Notification (PTN) - Waiver Approved";
                        string body = @"Your Waiver has been <span style=""color:green"">Approved</span>. <br/><br/><strong>Waiver#: </strong>" + waiverRec.WaiverNumber + @"<br/><strong>Waiver Title: </strong>" + waiverRec.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";

                        // Send Email...
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, waiverRec.CreatedUserEmail, null, null, null);

                        // Log that Email was Sent...
                        AddEmailHistory(null, subject, body, waiverRec.CreatedUserFullName, waiverRec.CreatedUser, waiverRec.CreatedUserEmail, null, waiverRec.Id, null, "Waiver", waiverRec.Status, DateTime.Now, _username);

                        // Email all admins with 'Approver' rights that this Waiver has been approved....
                        var adminApproverList = await _contextPtnWaiver.Administrators.Where(m => m.Approver == true).ToListAsync();
                        foreach (var record in adminApproverList)
                        {
                            var adminToNotify = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (record.Username ?? string.Empty).ToLower()).FirstOrDefaultAsync();
                            string subjectAdmin = @"Process Test Notification (PTN) - Waiver Approved";
                            string bodyAdmin = @"Waiver has been <span style=""color:green"">Approved</span>. <br/><br/><strong>Waiver#: </strong>" + waiverRec.WaiverNumber + @"<br/><strong>Waiver Title: </strong>" + waiverRec.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "/Waivers/Details?id=" + waiverRec.Id.ToString() + "&tab=Details&sourceScreen=Dashboard" + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";

                            // Send Email...
                            Initialization.EmailProviderSmtp.SendMessage(subjectAdmin, bodyAdmin, adminToNotify?.mail, null, null, null);

                            // Log that Email was Sent...
                            AddEmailHistory(null, subjectAdmin, bodyAdmin, waiverRec.CreatedUserFullName, waiverRec.CreatedUser, waiverRec.CreatedUserEmail, null, waiverRec.Id, null, "Waiver", waiverRec.Status, DateTime.Now, _username);
                        }

                    }
                    return RedirectToAction("Details", new { id = groupApproversReview.SourceId, tabWaiver = "WaiverAdminApproval" });
                }

                var groupApproverReviewVM = new GroupApproverReviewVM();

                var waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == groupApproversReview.SourceId);
                if (waiver == null)
                    return NotFound();

                var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == waiver.PTNId);
                if (ptn == null)
                    return NotFound();

                groupApproverReviewVM.PTN = ptn;
                groupApproverReviewVM.Waiver = waiver;
                groupApproverReviewVM.GroupApproversReview = groupApproversReview;

                return View("GroupApproveComment", groupApproverReviewVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWaiverMaterialNotes([Bind("Id", Prefix = "Ptn")] PTN ptnIn, [Bind("Id,MaterialDetailNotes", Prefix = "Waiver")] Waiver waiverIn)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (waiverIn.Id == null)
                return View("Index");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Waiver waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == waiverIn.Id);
            if (waiver == null)
                return View("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
            }
            waiver.MaterialDetailNotes = waiverIn.MaterialDetailNotes;
            //waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverMaterialDetails", saveMessageMaterialDetail = "Notes have been Saved" });
        }

        public async Task<IActionResult> SaveAdditionalEmailNotification([Bind("Id", Prefix = "Ptn")] PTN ptnIn, [Bind("Id,AdditionalEmailNotificationsOfMaterialDetails", Prefix = "Waiver")] Waiver waiverIn)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (waiverIn.Id == null)
                return View("Index");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Waiver waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == waiverIn.Id);
            if (waiver == null)
                return View("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                waiver.ModifiedUserFullName = userInfo.displayname;
                waiver.ModifiedUserEmail = userInfo.mail;
                waiver.ModifiedDate = DateTime.Now;
            }
            waiver.AdditionalEmailNotificationsOfMaterialDetails = waiverIn.AdditionalEmailNotificationsOfMaterialDetails;
            //waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverMaterialDetails", saveMessageMaterialDetail = "Email Notifications have been Saved" });
        }

        public async Task<IActionResult> MaterialDetailsNotification(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return View("Index"); ;

            if (id == null)
                return View("Index"); ;

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Waiver waiver = await _contextPtnWaiver.Waiver.FirstOrDefaultAsync(m => m.Id == id);
            if (waiver == null)
                return View("Index"); ;

            // Build Notification Email....
            string url = Initialization.WebsiteUrl + "/Waivers/Details?id=2&tabWaiver=WaiverMaterialDetails";
            string subject = @"PTN/Waiver System - Material Detail Notes notification.";
            string body = @"A waiver's Material Details have been added or changed.  Please follow link below and review the waivers Material Detail information. <br/><br/><strong>Waiver: </strong>" + waiver.WaiverNumber + @"<br/><strong>Revision: </strong>" + waiver.RevisionNumber.ToString() + @"<br/><strong>Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + url + "\" target=\"blank\" >PTN/Waiver System</a></strong><br/><br/>";

            // email the disposition team....
            Initialization.EmailProviderSmtp.SendMessage(subject, body, "DispoTeam@sksiltron.com", null, null, null);
            var emailHistory = AddEmailHistory(null, subject, body, "Disposition Team", null, "DispoTeam@sksiltron.com", null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            // send to each additional person to be notified...
            if (waiver.AdditionalEmailNotificationsOfMaterialDetails != null && waiver.AdditionalEmailNotificationsOfMaterialDetails.Any())
            {
                foreach (var person in waiver.AdditionalEmailNotificationsOfMaterialDetails)
                {
                    var sendToPerson = await _contextMoc.__mst_employee.Where(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (person ?? string.Empty).ToLower()).FirstOrDefaultAsync();
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, sendToPerson.mail, null, null, null);
                    AddEmailHistory(null, subject, body, sendToPerson.displayname, sendToPerson.onpremisessamaccountname, sendToPerson.mail, null, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
                }
            }
            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverMaterialDetails", saveMessageMaterialDetail = "Email Notifications have been sent out" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrimaryReviewer(int id, int waiverId, string newReviewer)
        {
            var record = await _contextPtnWaiver.GroupApproversReview.FindAsync(id);
            if (record == null)
                return NotFound();

            var originalPerson = record.PrimaryApproverFullName;

            // get assigned-to person info....
            var toPerson = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == newReviewer.ToLower()).FirstOrDefaultAsync();

            if (toPerson != null)
            {
                record.PrimaryApproverUsername = newReviewer;
                record.PrimaryApproverFullName = toPerson?.displayname;
                record.PrimaryApproverEmail = toPerson?.mail;
                record.PrimaryApproverTitle = toPerson?.jobtitle;

                record.ModifiedUser = _username;
                record.ModifiedDate = DateTime.Now;

                _contextPtnWaiver.Update(record);
                await _contextPtnWaiver.SaveChangesAsync();

                TempData["SuccessMessageImpactAssessment"] = "Reviewer updated successfully from '" + originalPerson + "" + "' to '" + toPerson?.displayname + "'.";
            }

            return RedirectToAction("Details", new { id = waiverId, tabWaiver = "WaiverAdminApproval" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSecondaryReviewer(int id, int waiverId, string newReviewer)
        {
            var record = await _contextPtnWaiver.GroupApproversReview.FindAsync(id);
            if (record == null)
                return NotFound();

            var originalPerson = record.SecondaryApproverFullName;

            // get assigned-to person info....
            var toPerson = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == (newReviewer ?? "").ToLower()).FirstOrDefaultAsync();

            //if (toPerson != null)
            //{
            record.SecondaryApproverUsername = newReviewer;
            record.SecondaryApproverFullName = toPerson?.displayname;
            record.SecondaryApproverEmail = toPerson?.mail;
            record.SecondaryApproverTitle = toPerson?.jobtitle;

            record.ModifiedUser = _username;
            record.ModifiedDate = DateTime.Now;

            _contextPtnWaiver.Update(record);
            await _contextPtnWaiver.SaveChangesAsync();

            TempData["SuccessMessageImpactAssessment"] = "Reviewer updated successfully from '" + originalPerson + "" + "' to '" + toPerson?.displayname + "'.";
            //}

            return RedirectToAction("Details", new { id = waiverId, tabWaiver = "WaiverAdminApproval" });
        }
    }
}