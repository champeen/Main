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
    public class FinalReviewTypesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public FinalReviewTypesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: FinalReviewTypes
        public async Task<IActionResult> Index()
        {
              return _context.FinalReviewType != null ? 
                          View(await _context.FinalReviewType.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.FinalReviewType'  is null.");
        }

        // GET: FinalReviewTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FinalReviewType == null)
            {
                return NotFound();
            }

            var finalReviewType = await _context.FinalReviewType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finalReviewType == null)
            {
                return NotFound();
            }

            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FinalReviewTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Reviewer,Email,Order")] FinalReviewType finalReviewType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(finalReviewType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FinalReviewType == null)
            {
                return NotFound();
            }

            var finalReviewType = await _context.FinalReviewType.FindAsync(id);
            if (finalReviewType == null)
            {
                return NotFound();
            }
            return View(finalReviewType);
        }

        // POST: FinalReviewTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Reviewer,Email,Order")] FinalReviewType finalReviewType)
        {
            if (id != finalReviewType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(finalReviewType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinalReviewTypeExists(finalReviewType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FinalReviewType == null)
            {
                return NotFound();
            }

            var finalReviewType = await _context.FinalReviewType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finalReviewType == null)
            {
                return NotFound();
            }

            return View(finalReviewType);
        }

        // POST: FinalReviewTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FinalReviewType == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.FinalReviewType'  is null.");
            }
            var finalReviewType = await _context.FinalReviewType.FindAsync(id);
            if (finalReviewType != null)
            {
                _context.FinalReviewType.Remove(finalReviewType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinalReviewTypeExists(int id)
        {
          return (_context.FinalReviewType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
