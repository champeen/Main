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
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeAreasController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeAreasController(Management_of_ChangeContext context) : base (context)
        {
            _context = context;
        }

        // GET: ChangeAreas
        public async Task<IActionResult> Index()
        {
              return _context.ChangeArea != null ? 
                          View(await _context.ChangeArea.OrderBy(m => m.Order).ThenBy(m => m.Description).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeArea'  is null.");
        }

        // GET: ChangeAreas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeArea == null)
                return NotFound();

            var changeArea = await _context.ChangeArea.FirstOrDefaultAsync(m => m.Id == id);

            if (changeArea == null)
                return NotFound();

            return View(changeArea);
        }

        // GET: ChangeAreas/Create
        public IActionResult Create()
        {
            ChangeArea changeArea = new ChangeArea
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            return View(changeArea);
        }

        // POST: ChangeAreas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeArea changeArea)
        {
            // Make sure duplicates are not entered...
            List<ChangeArea> checkDupes = await _context.ChangeArea
                .Where(m => m.Description == changeArea.Description)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Description", "Change Area already exists.");
                return View(changeArea);
            }

            if (ModelState.IsValid)
            {
                _context.Add(changeArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeArea);
        }

        // GET: ChangeAreas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeArea == null)
                return NotFound();

            var changeArea = await _context.ChangeArea.FindAsync(id);

            if (changeArea == null)
                return NotFound();

             return View(changeArea);
        }

        // POST: ChangeAreas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeArea changeArea)
        {
            if (id != changeArea.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            //List<ChangeArea> checkDupes = await _context.ChangeArea
            //    .Where(m => m.Description == changeArea.Description)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Description", "Change Area already exists.");
            //    return View(changeArea);
            //}

            changeArea.ModifiedUser = "Michael Wilson";
            changeArea.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeAreaExists(changeArea.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeArea);
        }

        // GET: ChangeAreas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeArea == null)
                 return NotFound();

            var changeArea = await _context.ChangeArea.FirstOrDefaultAsync(m => m.Id == id);

            if (changeArea == null)
                return NotFound();

            return View(changeArea);
        }

        // POST: ChangeAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeArea == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeArea'  is null.");

            var changeArea = await _context.ChangeArea.FindAsync(id);

            if (changeArea != null)
                _context.ChangeArea.Remove(changeArea);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeAreaExists(int id)
        {
          return (_context.ChangeArea?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
