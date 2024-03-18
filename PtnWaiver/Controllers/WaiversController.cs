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
                .Where(m=>m.DeletedDate == null)
                .OrderBy(m=>m.CreatedDate)
                .ThenBy(m=>m.WaiverNumber)
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
        public async Task<IActionResult> Details(int? id, string tab="Waiver", string tabWaiver="Details", string fileAttachmentError = null, string rejectedReason = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver
                .FirstOrDefaultAsync(m => m.Id == id);

            PTN ptn = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == waiver.PTNId);

            if (waiver == null || ptn == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.RejectedReason = rejectedReason;

            WaiverViewModel waiverVM = new WaiverViewModel();
            waiverVM.FileAttachmentError = fileAttachmentError;
            waiverVM.Waiver = waiver;
            waiverVM.Ptn = ptn;

            waiverVM.TabActiveDetail = "";
            waiverVM.TabActiveAttachmentsWaiver = "";
            waiverVM.TabActiveWaiverApproval = "";
            waiverVM.TabActiveWaiverAdminApproval = "";
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
            }

            // GET ALL ATTACHMENTS FOR Waiver ///////////////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString())))
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
            waiverVM.AttachmentsWaiver = attachments.OrderBy(m => m.Name).ToList();

            // Render Tabs Disabled/Enabled
            // Submit for Admin Approval Tab...
            waiverVM.Tab3Disabled = waiverVM.AttachmentsWaiver.Count == 0 ? "disabled" : "";
            // Admin Approve Waiver Tab...
            waiverVM.Tab4Disabled = waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Approved" || waiverVM.Waiver.Status == "Closed" || waiverVM.Waiver.Status == "Rejected" ? "" : "disabled";

            if (waiverVM.Waiver.Status != "Draft")
                ViewBag.Disable = "disabled";

            //return RedirectToAction("Details", "Waivers", new { id = waiver.PTNId, tab = "Waivers" });
            return View(waiverVM);
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

            //ViewBag.Ptns = getPtns();
            ViewBag.Status = getWaiverStatus();
            ViewBag.PorProjects = getPorProjects();
            ViewBag.ProductProcess = getProductProcess();

            PTN ptn = await _contextPtnWaiver.PTN.FirstOrDefaultAsync(m => m.Id == ptnId);
            if (ptn == null)
                return NotFound();

            Waiver waiver = new Waiver()
            {
                PTNId = ptnId,
                PtnDocId = ptn.DocId,
                Status = "Draft",
                PrimaryApproverUsername = ptn.PrimaryApproverUsername,
                PrimaryApproverEmail = ptn.PrimaryApproverEmail,
                PrimaryApproverFullName = ptn.PrimaryApproverFullName,
                PrimaryApproverTitle = ptn.PrimaryApproverTitle,
                SecondaryApproverUsername = ptn.SecondaryApproverUsername,
                SecondaryApproverEmail = ptn.SecondaryApproverEmail,
                SecondaryApproverFullName = ptn.SecondaryApproverFullName,
                SecondaryApproverTitle = ptn.SecondaryApproverTitle,
                CreatedDate = DateTime.Now,
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail
            };

            return View(waiver);
        }

        // POST: Waivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PorProject,Description,ProductProcess,DateClosed,CorrectiveActionDueDate,PTNId,PtnDocId,Status,PrimaryApproverUsername,PrimaryApproverFullName,PrimaryApproverEmail,PrimaryApproverTitle,SecondaryApproverUsername,SecondaryApproverFullName,SecondaryApproverEmail,SecondaryApproverTitle,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] Waiver waiver)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // This weird naming convention is striaght from how they are doing it in the spreadsheet.....
                string year = DateTime.Now.Year.ToString();
                string waiverNumber = "";
                for (int i = 1; i < 10000; i++)
                {
                    waiverNumber = "INS" + DateTime.Now.Year.ToString() + "-" + i.ToString();
                    Waiver record = await _contextPtnWaiver.Waiver
                        .FirstOrDefaultAsync(m => m.WaiverNumber == waiverNumber);
                    if (record == null)
                        break;
                }
                waiver.WaiverNumber = waiverNumber;

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString()));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString())))
                    path.Create();

                //waiver.PtnDocId = await _contextPtnWaiver.PTN.Where(m=>m.Id == waiver.PTNId).Select(m=>m.DocId).FirstOrDefaultAsync();
                _contextPtnWaiver.Add(waiver);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction("Details", "Waivers", new { id = waiver.Id, tab="Waivers" });
                //return RedirectToAction(nameof(Index));
            }
            ViewBag.Ptns = getPtns();
            ViewBag.Status = getWaiverStatus();
            ViewBag.PorProjects = getPorProjects();
            ViewBag.ProductProcess = getProductProcess();            
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
            ViewBag.PorProjects = getPorProjects();
            ViewBag.ProductProcess = getProductProcess();

            if (id == null || _contextPtnWaiver.Waiver == null)
                return NotFound();

            var waiver = await _contextPtnWaiver.Waiver.FindAsync(id);

            if (waiver == null)
                return NotFound();

            return View(waiver);
        }

        // POST: Waivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RevisionNumber,WaiverNumber,PorProject,Description,ProductProcess,Status,DateClosed,CorrectiveActionDueDate,PTNId,PtnDocId,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Waiver waiver)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != waiver.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var userInfo = getUserInfo(_username);
                    if (userInfo != null)
                    {
                        waiver.ModifiedUser = userInfo.onpremisessamaccountname;
                        waiver.ModifiedUserFullName = userInfo.displayname;
                        waiver.ModifiedUserEmail = userInfo.mail;
                        waiver.ModifiedDate = DateTime.Now;
                    }

                    waiver.PtnDocId = await _contextPtnWaiver.PTN.Where(m => m.Id == waiver.PTNId).Select(m => m.DocId).FirstOrDefaultAsync();
                    _contextPtnWaiver.Update(waiver);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaiverExists(waiver.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Waivers", new { id = waiver.Id, tab = "Details" });
                //return RedirectToAction(nameof(Index));
            }
            ViewBag.Ptns = getPtns();
            ViewBag.Status = getWaiverStatus();
            ViewBag.PorProjects = getPorProjects();
            ViewBag.ProductProcess = getProductProcess();
            
            return View(waiver);
        }

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

            if (waiver != null)
                _contextPtnWaiver.Waiver.Remove(waiver);
            
            await _contextPtnWaiver.SaveChangesAsync();
           
            return RedirectToAction("Details", "PTNs", new { id = waiver.PTNId, tab = "Waivers" });
        }

        private bool WaiverExists(int id)
        {
          return (_contextPtnWaiver.Waiver?.Any(e => e.Id == id)).GetValueOrDefault();
        }

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

            string filePath = Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber + '-' + waiver.RevisionNumber.ToString(), fileAttachment.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }

            return RedirectToAction("Details", new { id = id, tabWaiver = "AttachmentsWaiver" });
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

        public async Task<IActionResult> SubmitWaiverForAdminApproval(int id)
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
                waiver.SubmittedForApprovalUser = userInfo.onpremisessamaccountname;
                waiver.SubmittedForApprovalUserFullName = userInfo.displayname;
                waiver.SubmittedForApprovalDate = DateTime.Now;
            }
            waiver.Status = "Pending Approval";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email administrators to review the Waiver and either Approve or Reject....
            var admins = await _contextPtnWaiver.Administrators.Where(m => m.Approver == true).ToListAsync();
            foreach (var admin in admins)
            {
                string subject = @"Process Test Notification (PTN) - Waiver Review Needed";
                string body = @"Your Review is needed. Please follow link below and review/respond to the following Waiver request. <br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
                __mst_employee person = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == admin.Username).FirstOrDefaultAsync();

                Initialization.EmailProviderSmtp.SendMessage(subject, body, person.mail, null, null, null);
                AddEmailHistory(null, subject, body, person.displayname, person.onpremisessamaccountname, person.mail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);
            }
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
            var personRejecting = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Rejected";
            string body = @"Your Waiver has been <span style=""color:red"">rejected</span> by " + personRejecting.displayname + "." +
                "<br/><br/><strong>Reason Rejected: </strong>" + waiver.RejectedReason + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personRejecting.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = waiverIn.Id, tabWaiver = "WaiverApproval" });
        }

        public async Task<IActionResult> ApproveWaiverAdmin(int id)
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
            var personApproving = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Approved by Admin";
            string body = @"Your Waiver has been <span style=""color:green"">approved</span> by " + personApproving.displayname + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Description: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personApproving.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWaiverAdmin([Bind("Id,RejectedReason", Prefix = "Ptn")] PTN ptnIn, [Bind("Id,RejectedReason", Prefix = "Waiver")] Waiver waiverIn)
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
            var personRejecting = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
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
            waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            waiver.Id = 0;
            waiver.DateClosed = null;
            waiver.RevisionNumber += 1;
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
                return RedirectToAction("Index","Home");

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
            }
            waiver.Status = "Closed";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            // email Waiver creator to notify of PTN Rejection....
            var personClosing = await _contextMoc.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            string subject = @"Process Test Notification (PTN) - Waiver Closed";
            string body = @"Your Waiver has been <span style=""color:green"">closed</span> by " + personClosing.displayname + "." +
                "<br/><br/><strong>Waiver Number: </strong>" + waiver.WaiverNumber + "-" + waiver.RevisionNumber.ToString() + @"<br/><strong>Waiver Title: </strong>" + waiver.Description + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >PTN System</a></strong><br/><br/>";
            Initialization.EmailProviderSmtp.SendMessage(subject, body, waiver.CreatedUserEmail, personClosing.mail, null, null);
            AddEmailHistory(null, subject, body, waiver.CreatedUserFullName, waiver.CreatedUser, waiver.CreatedUserEmail, waiver.PTNId, waiver.Id, null, "Waiver", waiver.Status, DateTime.Now, _username);

            return RedirectToAction("Details", "PTNs", new { id = waiver.PTNId, tab = "Waivers" });
        }
    }
}
