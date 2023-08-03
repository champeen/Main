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

namespace Management_of_Change.Controllers
{
    public class ImplementationFinalApprovalMatricesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ImplementationFinalApprovalMatricesController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ImplementationFinalApprovalMatrices
        public async Task<IActionResult> Index()
        {
              return _context.ImplementationFinalApprovalMatrix != null ? 
                          View(await _context.ImplementationFinalApprovalMatrix.OrderBy(m => m.ChangeType).ThenBy(m => m.FinalReviewType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalMatrix'  is null.");
        }

        // GET: ImplementationFinalApprovalMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            ImplementationFinalApprovalMatrix implementationFinalApprovalMatrix = new ImplementationFinalApprovalMatrix
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
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
            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
            implementationFinalApprovalMatrix.ModifiedDate = DateTime.UtcNow;

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
            return View(implementationFinalApprovalMatrix);
        }

        // GET: ImplementationFinalApprovalMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
