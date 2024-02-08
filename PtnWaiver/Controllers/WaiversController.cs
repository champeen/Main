using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;

namespace PtnWaiver.Controllers
{
    public class WaiversController : Controller
    {
        private readonly PtnWaiverContext _context;

        public WaiversController(PtnWaiverContext context)
        {
            _context = context;
        }

        // GET: Waivers
        public async Task<IActionResult> Index()
        {
              return _context.Waiver != null ? 
                          View(await _context.Waiver.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.Waiver'  is null.");
        }

        // GET: Waivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Waiver == null)
                return NotFound();

            var waiver = await _context.Waiver
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiver == null)
                return NotFound();

            return View(waiver);
        }

        // GET: Waivers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Waivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PTNId,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] Waiver waiver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waiver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waiver);
        }

        // GET: Waivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Waiver == null)
                return NotFound();

            var waiver = await _context.Waiver.FindAsync(id);

            if (waiver == null)
                return NotFound();

            return View(waiver);
        }

        // POST: Waivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PTNId,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Waiver waiver)
        {
            if (id != waiver.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waiver);
                    await _context.SaveChangesAsync();
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
            return View(waiver);
        }

        // GET: Waivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Waiver == null)
                return NotFound();

            var waiver = await _context.Waiver
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
            if (_context.Waiver == null)
                return Problem("Entity set 'PtnWaiverContext.Waiver'  is null.");

            var waiver = await _context.Waiver.FindAsync(id);

            if (waiver != null)
                _context.Waiver.Remove(waiver);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaiverExists(int id)
        {
          return (_context.Waiver?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
