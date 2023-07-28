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
    public class ImpactAssessmentMatricesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ImpactAssessmentMatricesController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ImpactAssessmentMatrices
        public async Task<IActionResult> Index()
        {
              return _context.ImpactAssessmentMatrix != null ? 
                          View(await _context.ImpactAssessmentMatrix.OrderBy(m => m.ReviewType).ThenBy(m => m.ChangeType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentMatrix'  is null.");
        }

        // GET: ImpactAssessmentMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImpactAssessmentMatrix == null)
                return NotFound();

            var impactAssessmentMatrix = await _context.ImpactAssessmentMatrix
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentMatrix == null)
                return NotFound();

            return View(impactAssessmentMatrix);
        }

        // GET: ImpactAssessmentMatrices/Create
        public async Task<IActionResult> Create()
        {
            ImpactAssessmentMatrix impactAssessmentMatrix = new ImpactAssessmentMatrix
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentMatrix);
        }

        // POST: ImpactAssessmentMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,ChangeType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentMatrix impactAssessmentMatrix)
        {
            // Make sure duplicates are not entered...
            List<ImpactAssessmentMatrix> checkDupes = await _context.ImpactAssessmentMatrix
                .Where(m => m.ChangeType == impactAssessmentMatrix.ChangeType)
                .Where(m => m.ReviewType == impactAssessmentMatrix.ReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ModelState.AddModelError("ChangeType", "Change Type and Review Type combination already exist.");
                ModelState.AddModelError("ReviewType", "Change Type and Review Type combination already exist.");
                return View(impactAssessmentMatrix);
            }

            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentMatrix);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentMatrix);
        }

        // GET: ImpactAssessmentMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImpactAssessmentMatrix == null)
                return NotFound();

            var impactAssessmentMatrix = await _context.ImpactAssessmentMatrix.FindAsync(id);

            if (impactAssessmentMatrix == null)
                return NotFound();

            ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentMatrix);
        }

        // POST: ImpactAssessmentMatrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,ChangeType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentMatrix impactAssessmentMatrix)
        {
            if (id != impactAssessmentMatrix.Id)
                return NotFound();

            impactAssessmentMatrix.ModifiedUser = "Michael Wilson";
            impactAssessmentMatrix.ModifiedDate = DateTime.Now;

            // Make sure duplicates are not entered...
            List<ImpactAssessmentMatrix> checkDupes = await _context.ImpactAssessmentMatrix
                .Where(m => m.ChangeType == impactAssessmentMatrix.ChangeType)
                .Where(m => m.ReviewType == impactAssessmentMatrix.ReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
                ModelState.AddModelError("ChangeType", "Change Type and Review Type combination already exist.");
                ModelState.AddModelError("ReviewType", "Change Type and Review Type combination already exist.");
                return View(impactAssessmentMatrix);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(impactAssessmentMatrix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentMatrixExists(impactAssessmentMatrix.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentMatrix);
        }

        // GET: ImpactAssessmentMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImpactAssessmentMatrix == null)
                return NotFound();

            var impactAssessmentMatrix = await _context.ImpactAssessmentMatrix
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentMatrix == null)
                return NotFound();

            return View(impactAssessmentMatrix);
        }

        // POST: ImpactAssessmentMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImpactAssessmentMatrix == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentMatrix'  is null.");

            var impactAssessmentMatrix = await _context.ImpactAssessmentMatrix.FindAsync(id);

            if (impactAssessmentMatrix != null)
                _context.ImpactAssessmentMatrix.Remove(impactAssessmentMatrix);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentMatrixExists(int id)
        {
          return (_context.ImpactAssessmentMatrix?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
