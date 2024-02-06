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
    public class AdministratorsController : Controller
    {
        private readonly PtnWaiverContext _context;

        public AdministratorsController(PtnWaiverContext context)
        {
            _context = context;
        }

        // GET: Administrators
        public async Task<IActionResult> Index()
        {
              return _context.Administrators != null ? 
                          View(await _context.Administrators.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.Administrators'  is null.");
        }

        // GET: Administrators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrators = await _context.Administrators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrators == null)
            {
                return NotFound();
            }

            return View(administrators);
        }

        // GET: Administrators/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Approver,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Administrators administrators)
        {
            if (ModelState.IsValid)
            {
                _context.Add(administrators);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(administrators);
        }

        // GET: Administrators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrators = await _context.Administrators.FindAsync(id);
            if (administrators == null)
            {
                return NotFound();
            }
            return View(administrators);
        }

        // POST: Administrators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Approver,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Administrators administrators)
        {
            if (id != administrators.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrators);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministratorsExists(administrators.Id))
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
            return View(administrators);
        }

        // GET: Administrators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Administrators == null)
            {
                return NotFound();
            }

            var administrators = await _context.Administrators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrators == null)
            {
                return NotFound();
            }

            return View(administrators);
        }

        // POST: Administrators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Administrators == null)
            {
                return Problem("Entity set 'PtnWaiverContext.Administrators'  is null.");
            }
            var administrators = await _context.Administrators.FindAsync(id);
            if (administrators != null)
            {
                _context.Administrators.Remove(administrators);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorsExists(int id)
        {
          return (_context.Administrators?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
