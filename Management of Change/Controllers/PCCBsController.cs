using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.ViewModels;
using Management_of_Change.Utilities;

namespace Management_of_Change.Controllers
{
    public class PCCBsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public PCCBsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: PCCBs
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.PCCB != null ?
                          View(await _context.PCCB.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.PCCB'  is null.");
        }

        // GET: PCCBs/Details/5
        public async Task<IActionResult> Details(int? id, string fileAttachmentError = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PCCB == null)
                return NotFound();

            var pCCB = await _context.PCCB.FirstOrDefaultAsync(m => m.Id == id);
            if (pCCB == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == pCCB.ChangeRequestId);
            if (changeRequest == null)
                return NotFound();

            PccbVM pccbVM = new PccbVM();
            pccbVM.FileAttachmentError = fileAttachmentError;
            pccbVM.PCCB = pCCB;
            pccbVM.PCCB.Invitees = await _context.PccbInvitees.Where(m => m.PccbId == id).ToListAsync();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // MEETING ATTACHMENTS                                                                                   \\BAY1VPRD-MOC01\Management of Change\MOC-230707-1
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number, pCCB.Id.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number, pCCB.Id.ToString())))
                path.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<Attachment> attachments = new List<ViewModels.Attachment>();
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
            pccbVM.Attachments = attachments.OrderBy(m => m.Name).ToList();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            return View(pccbVM);
        }

        // GET: PCCBs/Create
        public IActionResult Create(int id, string tab = null)
        {
            PCCB pccb = new PCCB();
            pccb.ChangeRequestId = id;
            pccb.CreatedUser = _username;
            pccb.CreatedDate = DateTime.Now;
            pccb.Status = "Open";

            PccbVM pccbVM = new PccbVM();
            pccbVM.PCCB = pccb;

            ViewBag.Employees = getUserList();

            return View(pccbVM);
        }

        // POST: PCCBs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,Title,MeetingDateTime, Invitees,Agenda,Decisions,ActionItems,Status,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")]*/ PccbVM pccbVM)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            //if (pCCB.MeetingDate == null)
            //    ModelState.AddModelError("MeetingDate", "Must Include a Valid Meeting Date");

            //if (pCCB.MeetingDate < DateTime.Today)
            //    ModelState.AddModelError("MeetingDate", "Date Cannot Be In The Past");

            //if (pCCB.MeetingTime == null)
            //    ModelState.AddModelError("MeetingTime", "Must Include a Valid Meeting Time");

            if (pccbVM.PCCB.MeetingDateTime == null)
                ModelState.AddModelError("PCCB.MeetingDateTime", "Must Include a Valid Meeting Date/Time");

            if (pccbVM.Invitees == null || pccbVM.Invitees.Count == 0)
                ModelState.AddModelError("Invitees", "Must Invite at least 1 Person");

            if (ModelState.IsValid)
            {
                List<PccbInvitees> pccbInvitees = new List<PccbInvitees>();

                foreach (var invite in pccbVM.Invitees)
                {
                    var employee = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == invite).FirstOrDefaultAsync();
                    if (employee != null)
                    {
                        PccbInvitees invitee = new PccbInvitees();
                        invitee.Username = employee.onpremisessamaccountname;
                        invitee.FullName = employee.displayname;
                        invitee.Title = employee.jobtitle;
                        invitee.Email = employee.mail;
                        invitee.Status = "Invited"; // "Attended" "No Show"
                        invitee.Attended = false;
                        invitee.MocId = pccbVM.PCCB.ChangeRequestId;
                        invitee.PccbId = pccbVM.PCCB.Id; // CHECK THIS MAKE SURE IT IS FILLED
                        invitee.CreatedDate = DateTime.Now;
                        invitee.CreatedUser = _username;

                        pccbInvitees.Add(invitee);
                    }
                }
                pccbVM.PCCB.Invitees = pccbInvitees;

                _context.Add(pccbVM.PCCB);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "ChangeRequests", new { id = pccbVM.PCCB.ChangeRequestId, tab = "PccbReview" });
            }
            ViewBag.Employees = getUserList();
            return View(pccbVM);
        }

        // GET: PCCBs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PCCB == null)
                return NotFound();

            var pCCB = await _context.PCCB.FindAsync(id);
            if (pCCB == null)
                return NotFound();

            return View(pCCB);
        }

        // POST: PCCBs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,MeetingDate,MeetingTime,MeetingDateTime,Agenda,Decisions,ActionItems,Status,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PccbVM pccbVM)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pccbVM.PCCB.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pccbVM.PCCB);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PCCBExists(pccbVM.PCCB.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "ChangeRequests", new { id = pccbVM.PCCB.ChangeRequestId, tab = "PccbReview" });
            }
            return View(pccbVM);
        }

        // GET: PCCBs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PCCB == null)
                return NotFound();

            var pCCB = await _context.PCCB
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pCCB == null)
                return NotFound();

            return View(pCCB);
        }

        // POST: PCCBs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.PCCB == null)
                return Problem("Entity set 'Management_of_ChangeContext.PCCB'  is null.");

            var pCCB = await _context.PCCB.FindAsync(id);
            if (pCCB != null)
                _context.PCCB.Remove(pCCB);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PCCBExists(int id)
        {
            return (_context.PCCB?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Attended(int? inviteeId)
        {
            PccbInvitees pccbInvitee = await _context.PccbInvitees.FindAsync(inviteeId);
            if (pccbInvitee == null)
                return RedirectToAction("Index", "Home");

            pccbInvitee.Status = "Attended";
            pccbInvitee.ModifiedDate = DateTime.Now;
            pccbInvitee.ModifiedUser = _username;
            _context.Update(pccbInvitee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "PCCBs", new { id = pccbInvitee.PccbId, tab = "PccbReview" });
        }

        public async Task<IActionResult> Invited(int? inviteeId)
        {
            PccbInvitees pccbInvitee = await _context.PccbInvitees.FindAsync(inviteeId);
            if (pccbInvitee == null)
                return RedirectToAction("Index", "Home");

            pccbInvitee.Status = "Invited";
            pccbInvitee.ModifiedDate = DateTime.Now;
            pccbInvitee.ModifiedUser = _username;
            _context.Update(pccbInvitee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "PCCBs", new { id = pccbInvitee.PccbId, tab = "PccbReview" });
        }

        public async Task<IActionResult> Declined(int? inviteeId)
        {
            PccbInvitees pccbInvitee = await _context.PccbInvitees.FindAsync(inviteeId);
            if (pccbInvitee == null)
                return RedirectToAction("Index", "Home");

            pccbInvitee.Status = "Declined";
            pccbInvitee.ModifiedDate = DateTime.Now;
            pccbInvitee.ModifiedUser = _username;
            _context.Update(pccbInvitee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "PCCBs", new { id = pccbInvitee.PccbId, tab = "PccbReview" });
        }

        public async Task<IActionResult> NoShow(int? inviteeId)
        {
            PccbInvitees pccbInvitee = await _context.PccbInvitees.FindAsync(inviteeId);
            if (pccbInvitee == null)
                return RedirectToAction("Index", "Home");

            pccbInvitee.Status = "No Show";
            pccbInvitee.ModifiedDate = DateTime.Now;
            pccbInvitee.ModifiedUser = _username;
            _context.Update(pccbInvitee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "PCCBs", new { id = pccbInvitee.PccbId, tab = "PccbReview" });
        }

        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _context.PCCB == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", "PCCBs", new { id = id, tab = "PccbReview", fileAttachmentError = "No File Has Been Selected For Upload" });

            // get PCCB (meeting) record...
            var pccbRec = await _context.PCCB.FindAsync(id);
            if (pccbRec == null)
                return RedirectToAction("Index", "Home");

            // get ChangeRequest
            var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == pccbRec.ChangeRequestId);
            if (changeRequest == null)
                return RedirectToAction("Index","Home");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _context.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new { id = id, tab = "PccbReview", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact MoC Admin to add, or change document to allowable type." });

            string filePath = Path.Combine(Initialization.AttachmentDirectory, changeRequest.MOC_Number, pccbRec.Id.ToString(), fileAttachment.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }

            return RedirectToAction("Details", new { id = id, tab = "PccbReview" });
        }

    }
}
