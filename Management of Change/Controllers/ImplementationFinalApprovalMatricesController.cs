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
    public class ImplementationFinalApprovalMatricesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public ImplementationFinalApprovalMatricesController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: ImplementationFinalApprovalMatrices
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            List<ImplementationFinalApprovalMatrix> implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix.OrderBy(m => m.ChangeType).ThenBy(m => m.FinalReviewType).ToListAsync();

            foreach (var record in implementationFinalApprovalMatrix)
            {
                FinalReviewType finalReviewType = await _context.FinalReviewType.Where(m => m.Type == record.FinalReviewType).FirstOrDefaultAsync();
                if (finalReviewType != null)
                {
                    record.ReviewerEmail = finalReviewType.Email;
                    record.ReviewerName = finalReviewType.Reviewer;
                    record.ReviewerUsername = finalReviewType.Username;
                }
            }
            return View(implementationFinalApprovalMatrix);
        }

        public async Task<IActionResult> IndexHelp(string changeTypeFilter = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.ChangeTypes = await _context.ChangeType.Select(m => m.Type).OrderBy(m => m).ToListAsync();

            List<ImplementationFinalApprovalMatrix> implementationFinalApprovalMatrix = new List<ImplementationFinalApprovalMatrix>();
            if (changeTypeFilter == null)
                implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix.OrderBy(m => m.ChangeType).ThenBy(m => m.FinalReviewType).ToListAsync();
            else
                implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix.Where(m => m.ChangeType == changeTypeFilter).OrderBy(m => m.ChangeType).ThenBy(m => m.FinalReviewType).ToListAsync();

            foreach (var record in implementationFinalApprovalMatrix)
            {
                FinalReviewType finalReviewType = await _context.FinalReviewType.Where(m => m.Type == record.FinalReviewType).FirstOrDefaultAsync();
                if (finalReviewType != null)
                {
                    record.ReviewerEmail = finalReviewType.Email;
                    record.ReviewerName = finalReviewType.Reviewer;
                    record.ReviewerUsername = finalReviewType.Username;
                }
            }
            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalMatrix == null)
                return NotFound();

            var implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix
                .FirstOrDefaultAsync(m => m.Id == id);

            if (implementationFinalApprovalMatrix == null)
                return NotFound();

            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Create
        public async Task<IActionResult> Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ImplementationFinalApprovalMatrix implementationFinalApprovalMatrix = new ImplementationFinalApprovalMatrix
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(implementationFinalApprovalMatrix);
        }

        // POST: ImplementationFinalApprovalMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeType,FinalReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalMatrix implementationFinalApprovalMatrix)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ImplementationFinalApprovalMatrix> checkDupes = await _context.ImplementationFinalApprovalMatrix
                .Where(m => m.ChangeType == implementationFinalApprovalMatrix.ChangeType)
                .Where(m => m.FinalReviewType == implementationFinalApprovalMatrix.FinalReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ModelState.AddModelError("ChangeType", "Change Type and Final Review Type combination already exist.");
                ModelState.AddModelError("FinalReviewType", "Change Type and Final Review Type combination already exist.");
                return View(implementationFinalApprovalMatrix);
            }

            if (ModelState.IsValid)
            {
                _context.Add(implementationFinalApprovalMatrix);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalMatrix == null)
                return NotFound();

            var implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix.FindAsync(id);

            if (implementationFinalApprovalMatrix == null)
                return NotFound();

            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(implementationFinalApprovalMatrix);
        }

        // POST: ImplementationFinalApprovalMatrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeType,FinalReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalMatrix implementationFinalApprovalMatrix)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != implementationFinalApprovalMatrix.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ImplementationFinalApprovalMatrix> checkDupes = await _context.ImplementationFinalApprovalMatrix
                .Where(m => m.ChangeType == implementationFinalApprovalMatrix.ChangeType)
                .Where(m => m.FinalReviewType == implementationFinalApprovalMatrix.FinalReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ModelState.AddModelError("ChangeType", "Change Type and Final Review Type combination already exist.");
                ModelState.AddModelError("FinalReviewType", "Change Type and Final Review Type combination already exist.");
                return View(implementationFinalApprovalMatrix);
            }

            implementationFinalApprovalMatrix.ModifiedUser = _username;
            implementationFinalApprovalMatrix.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(implementationFinalApprovalMatrix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImplementationFinalApprovalMatrixExists(implementationFinalApprovalMatrix.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.FinalReviewTypes = await _context.FinalReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalMatrix == null)
                return NotFound();

            var implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix
                .FirstOrDefaultAsync(m => m.Id == id);

            if (implementationFinalApprovalMatrix == null)
                return NotFound();

            return View(implementationFinalApprovalMatrix);
        }

        // POST: ImplementationFinalApprovalMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ImplementationFinalApprovalMatrix == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalMatrix'  is null.");

            var implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix.FindAsync(id);

            if (implementationFinalApprovalMatrix != null)
                _context.ImplementationFinalApprovalMatrix.Remove(implementationFinalApprovalMatrix);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImplementationFinalApprovalMatrixExists(int id)
        {
          return (_context.ImplementationFinalApprovalMatrix?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
