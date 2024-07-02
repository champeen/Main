using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;

namespace Management_of_Change.Controllers
{
    public class PccbStepsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public PccbStepsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: PccbSteps
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.PccbStep != null ? 
                          View(await _context.PccbStep.OrderBy(m=>m.Order).ThenBy(m=>m.Description).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.PccbStep'  is null.");
        }

        // GET: PccbSteps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbStep == null)
                return NotFound();

            var pccbStep = await _context.PccbStep.FirstOrDefaultAsync(m => m.Id == id);
            if (pccbStep == null)
                return NotFound();

            return View(pccbStep);
        }

        // GET: PccbSteps/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            PccbStep pccbSTep = new PccbStep
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            return View(pccbSTep);
        }

        // POST: PccbSteps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PccbStep pccbStep)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<PccbStep> checkDupes = await _context.PccbStep
                .Where(m => m.Description == pccbStep.Description)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Description", "PCCB Meeting Step already exists.");

            if (ModelState.IsValid)
            {
                _context.Add(pccbStep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pccbStep);
        }

        // GET: PccbSteps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbStep == null)
                return NotFound();

            var pccbStep = await _context.PccbStep.FindAsync(id);
            if (pccbStep == null)
                return NotFound();

            return View(pccbStep);
        }

        // POST: PccbSteps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PccbStep pccbStep)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pccbStep.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<PccbStep> checkDupes = await _context.PccbStep
                .Where(m => m.Description == pccbStep.Description && m.Id != pccbStep.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Description", "PCCB Meeting Step already exists.");

            if (ModelState.IsValid)
            {
                try
                {
                    pccbStep.ModifiedDate = DateTime.Now;
                    pccbStep.ModifiedUser = _username;
                    _context.Update(pccbStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PccbStepExists(pccbStep.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pccbStep);
        }

        // GET: PccbSteps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.PccbStep == null)
                return NotFound();

            var pccbStep = await _context.PccbStep.FirstOrDefaultAsync(m => m.Id == id);
            if (pccbStep == null)
                return NotFound();

            return View(pccbStep);
        }

        // POST: PccbSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.PccbStep == null)
                return Problem("Entity set 'Management_of_ChangeContext.PccbStep'  is null.");

            var pccbStep = await _context.PccbStep.FindAsync(id);
            if (pccbStep != null)
                _context.PccbStep.Remove(pccbStep);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PccbStepExists(int id)
        {
          return (_context.PccbStep?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
