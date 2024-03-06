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
        public async Task<IActionResult> Details(int? id, string tab="Waiver", string tabWaiver="Details", string fileAttachmentError = null)
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
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber)))
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
            waiverVM.Tab4Disabled = waiverVM.Waiver.Status == "Pending Approval" || waiverVM.Waiver.Status == "Approved" || waiverVM.Waiver.Status == "Completed" || waiverVM.Waiver.Status == "Rejected" ? "" : "disabled";

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
        public async Task<IActionResult> Create([Bind("Id,PorProject,Description,ProductProcess,DateClosed,CorrectiveActionDueDate,PTNId,PtnDocId,Status,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] Waiver waiver)
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

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber)))
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
            return RedirectToAction(nameof(Index));
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

            string filePath = Path.Combine(Initialization.AttachmentDirectoryWaiver, waiver.WaiverNumber, fileAttachment.FileName);
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
                waiver.SubmittedForAdminApprovalUser = userInfo.onpremisessamaccountname;
                waiver.SubmittedForAdminApprovalUserFullName = userInfo.displayname;
                waiver.SubmittedForAdminApprovalDate = DateTime.Now;
            }
            waiver.Status = "Pending Approval";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }

        public async Task<IActionResult> RejectWaiver(int id)

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
                waiver.SubmittedForAdminApprovalUser = userInfo.onpremisessamaccountname;
                waiver.SubmittedForAdminApprovalUserFullName = userInfo.displayname;
                waiver.SubmittedForAdminApprovalDate = DateTime.Now;
            }
            waiver.Status = "Rejected";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverApproval" });
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
                waiver.ApprovedByAdminlUser = userInfo.onpremisessamaccountname;
                waiver.ApprovedByAdminlUserFullName = userInfo.displayname;
                waiver.ApprovedByAdminDate = DateTime.Now;
            }
            waiver.Status = "Approved";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }

        public async Task<IActionResult> RejectWaiverAdmin(int id)
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
                waiver.ApprovedByAdminlUser = userInfo.onpremisessamaccountname;
                waiver.ApprovedByAdminlUserFullName = userInfo.displayname;
                waiver.ApprovedByAdminDate = DateTime.Now;
            }
            waiver.Status = "Rejected";

            _contextPtnWaiver.Waiver.Update(waiver);
            await _contextPtnWaiver.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tabWaiver = "WaiverAdminApproval" });
        }
    }
}
