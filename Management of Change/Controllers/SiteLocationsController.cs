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
    public class SiteLocationsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public SiteLocationsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: SiteLocations
        public async Task<IActionResult> Index()
        {
              return _context.SiteLocation != null ? 
                          View(await _context.SiteLocation.OrderBy(m => m.Order).ThenBy(m => m.Description).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.SiteLocation'  is null.");
        }

        // GET: SiteLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SiteLocation == null)
            {
                return NotFound();
            }

            var siteLocation = await _context.SiteLocation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteLocation == null)
            {
                return NotFound();
            }

            return View(siteLocation);
        }

        // GET: SiteLocations/Create
        public IActionResult Create()
        {
            SiteLocation siteLocation = new SiteLocation
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            return View(siteLocation);
        }

        // POST: SiteLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] SiteLocation siteLocation)
        {
            // Make sure duplicates are not entered...
            List<SiteLocation> checkDupes = await _context.SiteLocation
                .Where(m => m.Description == siteLocation.Description)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Description", "Site/Location already exists.");
                return View(siteLocation);
            }

            if (ModelState.IsValid)
            {
                _context.Add(siteLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteLocation);
        }

        // GET: SiteLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SiteLocation == null)
            {
                return NotFound();
            }

            var siteLocation = await _context.SiteLocation.FindAsync(id);
            if (siteLocation == null)
            {
                return NotFound();
            }
            return View(siteLocation);
        }

        // POST: SiteLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] SiteLocation siteLocation)
        {
            if (id != siteLocation.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            //List<SiteLocation> checkDupes = await _context.SiteLocation
            //    .Where(m => m.Description == siteLocation.Description)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Description", "Site/Location already exists.");
            //    return View(siteLocation);
            //}

            siteLocation.ModifiedUser = "Michael Wilson";
            siteLocation.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteLocationExists(siteLocation.Id))
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
            return View(siteLocation);
        }

        // GET: SiteLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SiteLocation == null)
            {
                return NotFound();
            }

            var siteLocation = await _context.SiteLocation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteLocation == null)
            {
                return NotFound();
            }

            return View(siteLocation);
        }

        // POST: SiteLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SiteLocation == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.SiteLocation'  is null.");
            }
            var siteLocation = await _context.SiteLocation.FindAsync(id);
            if (siteLocation != null)
            {
                _context.SiteLocation.Remove(siteLocation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteLocationExists(int id)
        {
          return (_context.SiteLocation?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
