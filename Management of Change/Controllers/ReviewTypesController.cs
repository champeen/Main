using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;

namespace Management_of_Change.Controllers
{
    public class ReviewTypesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ReviewTypesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ReviewTypes
        public async Task<IActionResult> Index()
        {
              return _context.ReviewType != null ? 
                          View(await _context.ReviewType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ReviewType'  is null.");
        }

        // GET: ReviewTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reviewType == null)
                return NotFound();

            return View(reviewType);
        }

        // GET: ReviewTypes/Create
        public IActionResult Create()
        {
            ReviewType reviewType = new ReviewType
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            return View(reviewType);
        }

        // POST: ReviewTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Order,Reviewer,Email,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ReviewType reviewType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviewType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reviewType);
        }

        // GET: ReviewTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType.FindAsync(id);

            if (reviewType == null)
                return NotFound();

            return View(reviewType);
        }

        // POST: ReviewTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Order,Reviewer,Email,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ReviewType reviewType)
        {
            if (id != reviewType.Id)
                return NotFound();

            reviewType.ModifiedUser = "Michael Wilson";
            reviewType.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reviewType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewTypeExists(reviewType.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reviewType);
        }

        // GET: ReviewTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reviewType == null)
                return NotFound();

            return View(reviewType);
        }

        // POST: ReviewTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReviewType == null)
                return Problem("Entity set 'Management_of_ChangeContext.ReviewType'  is null.");

            var reviewType = await _context.ReviewType.FindAsync(id);

            if (reviewType != null)
                _context.ReviewType.Remove(reviewType);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewTypeExists(int id)
        {
          return (_context.ReviewType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
