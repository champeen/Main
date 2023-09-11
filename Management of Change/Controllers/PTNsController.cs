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
    public class PTNsController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public PTNsController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: PTNs
        public async Task<IActionResult> Index()
        {
              return _context.PTN != null ? 
                          View(await _context.PTN.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.PTN'  is null.");
        }

        // GET: PTNs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PTN == null)
            {
                return NotFound();
            }

            var pTN = await _context.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
            {
                return NotFound();
            }

            return View(pTN);
        }

        // GET: PTNs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PTNs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Enabled,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PTN pTN)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pTN);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pTN);
        }

        // GET: PTNs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PTN == null)
            {
                return NotFound();
            }

            var pTN = await _context.PTN.FindAsync(id);
            if (pTN == null)
            {
                return NotFound();
            }
            return View(pTN);
        }

        // POST: PTNs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Enabled,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] PTN pTN)
        {
            if (id != pTN.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pTN);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PTNExists(pTN.Id))
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
            return View(pTN);
        }

        // GET: PTNs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PTN == null)
            {
                return NotFound();
            }

            var pTN = await _context.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
            {
                return NotFound();
            }

            return View(pTN);
        }

        // POST: PTNs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PTN == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.PTN'  is null.");
            }
            var pTN = await _context.PTN.FindAsync(id);
            if (pTN != null)
            {
                _context.PTN.Remove(pTN);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PTNExists(int id)
        {
          return (_context.PTN?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
