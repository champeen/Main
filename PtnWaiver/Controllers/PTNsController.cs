using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.Utilities;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class PTNsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public PTNsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: PTNs
        public async Task<IActionResult> Index(string statusFilter, string prevStatusFilter = null, string sort = null, string prevSort = null)
        {
            // make sure valid Username
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
            var ptns = await _contextPtnWaiver.PTN
                .Where(m => m.DeletedDate == null)
                .OrderBy(m => m.CreatedDate)
                .ThenBy(m => m.DocId)
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
                    ptns = ptns.Where(m => m.Status == statusFilter).ToList();
                    ViewBag.PrevStatusFilter = statusFilter;
                    break;
            }
            return View(ptns);
        }

        // GET: PTNs/Details/5
        public async Task<IActionResult> Details(int? id, string? tab = "Details", string fileAttachmentError = null, string rejectedReason = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.RejectedReason = rejectedReason;

            PtnViewModel ptnVM = new PtnViewModel();
            ptnVM.FileAttachmentError = fileAttachmentError;
            ptnVM.PTN = pTN;

            ptnVM.TabActiveDetail = "";
            ptnVM.TabActiveAttachmentsPtn = "";
            ptnVM.TabActivePtnApproval = "";
            ptnVM.TabActivePtnAdminApproval = "";
            ptnVM.TabActiveWaivers = "";
            ptnVM.TabActiveAttachmentsWaiver = "";
            switch (tab)
            {
                case null:
                    ptnVM.TabActiveDetail = "active";
                    break;
                case "":
                    ptnVM.TabActiveDetail = "active";
                    break;
                case "Details":
                    ptnVM.TabActiveDetail = "active";
                    break;
                case "AttachmentsPtn":
                    ptnVM.TabActiveAttachmentsPtn = "active";
                    break;
                case "PtnApproval":
                    ptnVM.TabActivePtnApproval = "active";
                    break;
                case "PtnAdminApproval":
                    ptnVM.TabActivePtnAdminApproval = "active";
                    break;
                case "Waivers":
                    ptnVM.TabActiveWaivers = "active";
                    break;
            }

            // GET ALL ATTACHMENTS FOR PTN ///////////////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryPTN, pTN.DocId));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryPTN, pTN.DocId)))
                path.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

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

                //var blah = i.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            ptnVM.AttachmentsPtn = attachments.OrderBy(m => m.Name).ToList();

            // Get all Waivers under this PTN....
            ptnVM.PTN.Waivers = await _contextPtnWaiver.Waiver.Where(m => m.PTNId == id && m.DeletedDate == null).ToListAsync();

            // RENDER TABS DISABLED/ENABLED....
            // Submit for Admin Approval Tab...
            ptnVM.Tab3Disabled = ptnVM.AttachmentsPtn.Count == 0 ? "disabled" : "";
            // Admin Approve Ptn Tab...
            ptnVM.Tab4Disabled = ptnVM.PTN.Status == "Pending Approval" || ptnVM.PTN.Status == "Approved" || ptnVM.PTN.Status == "Closed" || ptnVM.PTN.Status == "Rejected" ? "" : "disabled";
            // Waivers Tab...
            ptnVM.Tab5Disabled = ptnVM.PTN.Status == "Approved" || ptnVM.PTN.Status == "Closed" || ptnVM.PTN.Status == "Rejected" ? "" : "disabled";

            if (ptnVM.PTN.Status != "Draft")
                ViewBag.Disable = "disabled";

            return View(ptnVM);
        }

        // GET: PTNs/Create
        public IActionResult Create()
        {
            // make sure valid Username
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

            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();

            PTN ptn = new PTN()
            {
                Status = "Draft",
                CreatedDate = DateTime.Now,
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail
            };

            return View(ptn);
        }

        // POST: PTNs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DocId,PtnPin,Area,SubjectType,Title,Group,TisNumber,PdfLocation,Status,Roadblocks,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] PTN ptn)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // This weird naming convention is striaght from how they are doing it in the spreadsheet.....
                int days = _getDaysSince1900;
                string docId = "";
                for (int i = 1; i < 10000; i++)
                {
                    docId = ptn.PtnPin + "-" + days.ToString() + "-" + i.ToString();
                    PTN record = await _contextPtnWaiver.PTN
                        .FirstOrDefaultAsync(m => m.DocId == docId);
                    if (record == null)
                        break;
                }
                ptn.DocId = docId;

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryPTN, ptn.DocId));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryPTN, ptn.DocId)))
                    path.Create();

                _contextPtnWaiver.Add(ptn);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            return View(ptn);
        }

        public async Task<IActionResult> Clone(int id, string? tab = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || id == 0)
                return View("Index");

            PTN ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == id);
            if (ptn == null)
                return View("Index");

            var ptnOwner = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();

            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            ViewBag.ClonedId = id;

            ptn.Status = "Draft";
            ptn.CreatedUser = _username;
            ptn.CreatedUserFullName = ptnOwner.displayname;
            ptn.CreatedUserEmail = ptnOwner.mail;
            ptn.CreatedDate = DateTime.Now;
            ptn.ModifiedDate = null;
            ptn.ModifiedUser = null;
            ptn.DeletedDate = null;
            ptn.DeletedUser = null;

            return View(ptn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloneCreate([Bind("Id,DocId,PtnPin,Area,SubjectType,Title,Group,TisNumber,PdfLocation,Status,Roadblocks,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] PTN ptn, int clonedId, string? source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            //if (changeRequest.Estimated_Completion_Date == null)
            //    ModelState.AddModelError("Estimated_Completion_Date", "Must Include a Valid Completion Date");

            //if (changeRequest.Estimated_Completion_Date < DateTime.Today)
            //    ModelState.AddModelError("Estimated_Completion_Date", "Date Cannot Be In The Past");

            //if ((changeRequest.Change_Level == "Level 1 - Major" || changeRequest.Change_Level == "Level 2 - Major" || changeRequest.Change_Level == "Level 3 - Minor") && (String.IsNullOrWhiteSpace(changeRequest.CMT_Number)))
            //    ModelState.AddModelError("CMT_Number", "All Level 1-3 Changes Require a CMT");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // This weird naming convention is striaght from how they are doing it in the spreadsheet.....
                int days = _getDaysSince1900;
                string docId = "";
                for (int i = 1; i < 10000; i++)
                {
                    docId = ptn.PtnPin + "-" + days.ToString() + "-" + i.ToString();
                    PTN record = await _contextPtnWaiver.PTN
                        .FirstOrDefaultAsync(m => m.DocId == docId);
                    if (record == null)
                        break;
                }
                ptn.DocId = docId;

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryPTN, ptn.DocId));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryPTN, ptn.DocId)))
                    path.Create();

                _contextPtnWaiver.Add(ptn);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            ViewBag.ClonedId = clonedId;

            return View("Clone", ptn);
        }

        // GET: PTNs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN.FindAsync(id);
            if (pTN == null)
                return NotFound();

            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();

            return View(pTN);
        }

        // POST: PTNs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocId,PtnPin,Area,SubjectType,Title,Group,TisNumber,PdfLocation,Status,Roadblocks,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] PTN pTN)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pTN.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    pTN.ModifiedUser = userInfo.onpremisessamaccountname;
                    pTN.ModifiedUserFullName = userInfo.displayname;
                    pTN.ModifiedUserEmail = userInfo.mail;
                    pTN.ModifiedDate = DateTime.Now;
                }
                try
                {
                    _contextPtnWaiver.Update(pTN);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PTNExists(pTN.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", new { id = pTN.Id });
            }
            ViewBag.Status = getPtnStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            return View(pTN);
        }

        // GET: PTNs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
                return NotFound();

            return View(pTN);
        }

        // POST: PTNs/Delete/5
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

            if (_contextPtnWaiver.PTN == null)
                return Problem("Entity set 'PtnWaiverContext.PTN'  is null.");

            var pTN = await _contextPtnWaiver.PTN.FindAsync(id);
            if (pTN != null)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    pTN.DeletedUser = userInfo.onpremisessamaccountname;
                    pTN.DeletedUserFullName = userInfo.displayname;
                    pTN.DeletedUserEmail = userInfo.mail;
                    pTN.DeletedDate = DateTime.Now;
                }
                _contextPtnWaiver.PTN.Update(pTN);
                await _contextPtnWaiver.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PTNExists(int id)
        {
            return (_contextPtnWaiver.PTN?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "No File Has Been Selected For Upload" });

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == id);
            if (ptn == null)
                return RedirectToAction("Index");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _contextPtnWaiver.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            string filePath = Path.Combine(Initialization.AttachmentDirectoryPTN, ptn.DocId, fileAttachment.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }

            return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn" });
        }

        public async Task<IActionResult> DownloadFile(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }
        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn" });
        }

        public async Task<IActionResult> SubmitPtnForAdminApproval(int id)
        {
            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == id);
            if (ptn == null)
                return RedirectToAction("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                ptn.ModifiedUser = userInfo.onpremisessamaccountname;
                ptn.ModifiedUserFullName = userInfo.displayname;
                ptn.ModifiedUserEmail = userInfo.mail;
                ptn.ModifiedDate = DateTime.Now;
                ptn.SubmittedForAdminApprovalUser = userInfo.onpremisessamaccountname;
                ptn.SubmittedForAdminApprovalUserFullName = userInfo.displayname;
                ptn.SubmittedForAdminApprovalDate = DateTime.Now;
            }
            ptn.Status = "Pending Approval";

            _contextPtnWaiver.PTN.Update(ptn);
            await _contextPtnWaiver.SaveChangesAsync();

            // email administrators to review the PTN and either Approve or Reject....
            var admins = await _contextPtnWaiver.Administrators.Where(m => m.Approver == true).ToListAsync();
            foreach (var admin in admins)
            {
                string subject = @"Process Test Notification (PTN) - PTN Review Needed";
                string body = @"Your Review is needed. Please follow link below and review/respond to the following PTN request. <br/><br/><strong>DocId: </strong>" + ptn.DocId + @"<br/><strong>PTN Title: </strong>" + ptn.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
                __mst_employee person = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == admin.Username).FirstOrDefaultAsync();

                Initialization.EmailProviderSmtp.SendMessage(subject, body, person.mail, null, null, null);
                AddEmailHistory(null, subject, body, person.displayname, person.onpremisessamaccountname, person.mail, ptn.Id, null, null, "PTN", ptn.Status, DateTime.Now, _username);
            }

            return RedirectToAction("Details", new { id = id, tab = "PtnAdminApproval" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPtn(int id, [Bind("Id,RejectedReason")] PTN pTN)
        {
            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == pTN.Id);
            if (ptn == null)
                return RedirectToAction("Index");

            if (pTN.RejectedReason == null)                
                return RedirectToAction("Details", new { id = pTN.Id, tab = "PtnApproval", rejectedReason = "If PTN is Rejected, Rejected Reason is Required" });

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                ptn.ModifiedUser = userInfo.onpremisessamaccountname;
                ptn.ModifiedUserFullName = userInfo.displayname;
                ptn.ModifiedUserEmail = userInfo.mail;
                ptn.ModifiedDate = DateTime.Now;
                ptn.SubmittedForAdminApprovalUser = userInfo.onpremisessamaccountname;
                ptn.SubmittedForAdminApprovalUserFullName = userInfo.displayname;
                ptn.SubmittedForAdminApprovalDate = DateTime.Now;
            }
            ptn.RejectedReason = pTN.RejectedReason;
            ptn.RejectedBeforeSubmission = true;
            ptn.Status = "Rejected";

            _contextPtnWaiver.PTN.Update(ptn);
            await _contextPtnWaiver.SaveChangesAsync();

            // email PTN creator to notify of PTN Rejection....
            var personRejecting = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - PTN Rejected";
            string body = @"Your PTN has been <span style=""color:red"">rejected</span> by " + personRejecting.displayname + "." + 
                "<br/><br/><strong>Reason Rejected: </strong>" + ptn.RejectedReason + "." +
                "<br/><br/><strong>DocId: </strong>" + ptn.DocId + @"<br/><strong>PTN Title: </strong>" + ptn.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, ptn.CreatedUserEmail, personRejecting.mail, null, null);
            AddEmailHistory(null, subject, body, ptn.CreatedUserFullName, ptn.CreatedUser, ptn.CreatedUserEmail, ptn.Id, null, null, "PTN", ptn.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = pTN.Id, tab = "PtnApproval" });
        }

        public async Task<IActionResult> ApprovePtnAdmin(int id)
        {
            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == id);
            if (ptn == null)
                return RedirectToAction("Index");

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                ptn.ModifiedUser = userInfo.onpremisessamaccountname;
                ptn.ModifiedUserFullName = userInfo.displayname;
                ptn.ModifiedUserEmail = userInfo.mail;
                ptn.ModifiedDate = DateTime.Now;
                ptn.ApprovedByAdminlUser = userInfo.onpremisessamaccountname;
                ptn.ApprovedByAdminlUserFullName = userInfo.displayname;
                ptn.ApprovedByAdminDate = DateTime.Now;
            }
            ptn.Status = "Approved";

            _contextPtnWaiver.PTN.Update(ptn);
            await _contextPtnWaiver.SaveChangesAsync();

            // email PTN creator that the PTN was Approved....
            var personApproving = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - PTN Approved by Admin";
            string body = @"Your PTN has been <span style=""color:green"">approved</span> by " + personApproving.displayname + ".<br/><br/><strong>DocId: </strong>" + ptn.DocId + @"<br/><strong>PTN Title: </strong>" + ptn.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, ptn.CreatedUserEmail, personApproving.mail, null, null);
            AddEmailHistory(null, subject, body, ptn.CreatedUserFullName, ptn.CreatedUser, ptn.CreatedUserEmail, ptn.Id, null, null, "PTN", ptn.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = id, tab = "Waivers" });
        }

        //public async Task<IActionResult> RejectPtnAdmin(int id)
        //public async Task<IActionResult> RejectPtn(int id)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPtnAdmin(int id, [Bind("Id,RejectedReason")] PTN pTN)
        {
            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == id);
            if (ptn == null)
                return RedirectToAction("Index");

            if (pTN.RejectedReason == null)
                return RedirectToAction("Details", new { id = id, tab = "PtnAdminApproval", rejectedReason = "If PTN is Rejected, Rejected Reason is Required" });

            var userInfo = getUserInfo(_username);
            if (userInfo != null)
            {
                ptn.ModifiedUser = userInfo.onpremisessamaccountname;
                ptn.ModifiedUserFullName = userInfo.displayname;
                ptn.ModifiedUserEmail = userInfo.mail;
                ptn.ModifiedDate = DateTime.Now;
                ptn.ApprovedByAdminlUser = userInfo.onpremisessamaccountname;
                ptn.ApprovedByAdminlUserFullName = userInfo.displayname;
                ptn.ApprovedByAdminDate = DateTime.Now;
            }
            ptn.RejectedReason = pTN.RejectedReason;
            ptn.RejectedByAdmin = true;
            ptn.Status = "Rejected";

            _contextPtnWaiver.PTN.Update(ptn);
            await _contextPtnWaiver.SaveChangesAsync();

            // email PTN creator to notify of PTN Rejection....
            var personRejecting = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - PTN Rejected";
            string body = @"Your PTN has been <span style=""color:red"">rejected</span> by " + personRejecting.displayname + "." +
                "<br/><br/><strong>Reason Rejected: </strong>" + ptn.RejectedReason + "." +
                "<br/><br/><strong>DocId: </strong>" + ptn.DocId + @"<br/><strong>PTN Title: </strong>" + ptn.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, ptn.CreatedUserEmail, personRejecting.mail, null, null);
            AddEmailHistory(null, subject, body, ptn.CreatedUserFullName, ptn.CreatedUser, ptn.CreatedUserEmail, ptn.Id, null, null, "PTN", ptn.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = id, tab = "PtnAdminApproval" });
        }

    }
}
