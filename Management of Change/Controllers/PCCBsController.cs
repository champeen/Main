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
        public async Task<IActionResult> Details(int? id)
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

        // GET: PCCBs/Create
        public IActionResult Create(int id, string tab = null)
        {
            PCCB pccb = new PCCB();
            pccb.ChangeRequestId = id;
            pccb.CreatedUser = _username;
            pccb.CreatedDate = DateTime.Now;
            pccb.Status = "Open";

            return View(pccb);
        }

        // POST: PCCBs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,MeetingDate,MeetingTime,MeetingDateTime,Agenda,Decisions,ActionItems,Status,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PCCB pCCB)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (pCCB.MeetingDate == null)
                ModelState.AddModelError("MeetingDate", "Must Include a Valid Meeting Date");

            if (pCCB.MeetingDate < DateTime.Today)
                ModelState.AddModelError("MeetingDate", "Date Cannot Be In The Past");

            if (pCCB.MeetingTime == null)
                ModelState.AddModelError("MeetingTime", "Must Include a Valid Meeting Time");

            if (pCCB.MeetingDateTime == null)
                ModelState.AddModelError("MeetingDateTime", "Must Include a Valid Meeting Date/Time");

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                _context.Add(pCCB);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pCCB);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,MeetingDate,MeetingTime,MeetingDateTime,Agenda,Decisions,ActionItems,Status,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PCCB pCCB)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pCCB.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pCCB);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PCCBExists(pCCB.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pCCB);
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
    }
}
