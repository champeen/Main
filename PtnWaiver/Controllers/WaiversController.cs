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
        public async Task<IActionResult> Details(int? id)
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

            return RedirectToAction("Details", "Waivers", new { id = waiver.PTNId, tab = "Waivers" });
            //return View(waiver);
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

                DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory, waiver.WaiverNumber));
                if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory, waiver.WaiverNumber)))
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
                return RedirectToAction(nameof(Index));
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
    }
}
