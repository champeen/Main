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
    public class PccbInviteesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public PccbInviteesController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: PccbInvitees
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.PccbInvitees != null ?
                          View(await _context.PccbInvitees.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.PccbInvitees'  is null.");
        }

        // GET: PccbInvitees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbInvitees == null)
                return NotFound();

            var pccbInvitees = await _context.PccbInvitees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pccbInvitees == null)
                return NotFound();

            return View(pccbInvitees);
        }

        // GET: PccbInvitees/Create
        public IActionResult Create(int pccbId)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            PCCB pccbRec = _context.PCCB.Find(pccbId);
            if (pccbRec == null)
                return View("Index", "Home");

            PccbInvitees pccbInvitees = new PccbInvitees
            {
                MocId = pccbRec.ChangeRequestId,
                PccbId = pccbId,
                CreatedDate = DateTime.Now,
                CreatedUser = _username
            };

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Employees = getUserList();

            return View(pccbInvitees);
        }

        // POST: PccbInvitees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,FullName,Title,Attended,Status,Comments,PccbId,MocId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PccbInvitees pccbInvitees)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            bool found = await _context.PccbInvitees.AnyAsync(x=>x.Username == pccbInvitees.Username);
            if (found)
                ModelState.AddModelError("Username", "Employee is already invited");

            var employee = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == pccbInvitees.Username).FirstOrDefaultAsync();
            if (employee != null)
            {
                if (ModelState.IsValid)
                {
                    pccbInvitees.FullName = employee.displayname;
                    pccbInvitees.Email = employee.mail;
                    pccbInvitees.Title = employee.jobtitle;

                    _context.Add(pccbInvitees);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "PCCBs", new { id = pccbInvitees.PccbId, tab="PccbReview" });
                    //return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Employees = getUserList();
            return View(pccbInvitees);
        }

        // GET: PccbInvitees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbInvitees == null)
                return NotFound();

            var pccbInvitees = await _context.PccbInvitees.FindAsync(id);
            if (pccbInvitees == null)
                return NotFound();

            return View(pccbInvitees);
        }

        // POST: PccbInvitees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,FullName,Title,Attended,Status,Comments,PccbId,MocId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PccbInvitees pccbInvitees)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pccbInvitees.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pccbInvitees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PccbInviteesExists(pccbInvitees.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pccbInvitees);
        }

        // GET: PccbInvitees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbInvitees == null)
                return NotFound();

            var pccbInvitees = await _context.PccbInvitees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pccbInvitees == null)
                return NotFound();

            //return RedirectToAction("Details","PCCBs", new {id = pccbInvitees.PccbId, tab="PccbReview"});
            return View(pccbInvitees);
        }

        // POST: PccbInvitees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.PccbInvitees == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.PccbInvitees'  is null.");
            }
            var pccbInvitees = await _context.PccbInvitees.FindAsync(id);
            if (pccbInvitees != null)
                _context.PccbInvitees.Remove(pccbInvitees);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "PCCBs", new { id = pccbInvitees.PccbId, tab = "PccbReview" });
            //return RedirectToAction(nameof(Index));
        }

        private bool PccbInviteesExists(int id)
        {
            return (_context.PccbInvitees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
