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
    public class ChangeTypesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeTypesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ChangeTypes
        public async Task<IActionResult> Index()
        {
              return _context.ChangeType != null ? 
                          View(await _context.ChangeType.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeType'  is null.");
        }

        // GET: ChangeTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeType == null)
            {
                return NotFound();
            }

            var changeType = await _context.ChangeType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (changeType == null)
            {
                return NotFound();
            }

            return View(changeType);
        }

        // GET: ChangeTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChangeTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Description,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeType changeType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(changeType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeType);
        }

        // GET: ChangeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeType == null)
            {
                return NotFound();
            }

            var changeType = await _context.ChangeType.FindAsync(id);
            if (changeType == null)
            {
                return NotFound();
            }
            return View(changeType);
        }

        // POST: ChangeTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Description,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeType changeType)
        {
            if (id != changeType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeTypeExists(changeType.Id))
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
            return View(changeType);
        }

        // GET: ChangeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeType == null)
            {
                return NotFound();
            }

            var changeType = await _context.ChangeType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (changeType == null)
            {
                return NotFound();
            }

            return View(changeType);
        }

        // POST: ChangeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeType == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.ChangeType'  is null.");
            }
            var changeType = await _context.ChangeType.FindAsync(id);
            if (changeType != null)
            {
                _context.ChangeType.Remove(changeType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeTypeExists(int id)
        {
          return (_context.ChangeType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
