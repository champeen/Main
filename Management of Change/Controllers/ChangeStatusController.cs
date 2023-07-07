using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeStatusController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeStatusController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ChangeSteps
        public async Task<IActionResult> Index()
        {
              return _context.ChangeStatus != null ? 
                          View(await _context.ChangeStatus.OrderBy(m => m.Order).ThenBy(m => m.Status).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeStatus'  is null.");
        }

        // GET: ChangeStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeStatus == null)
                return NotFound();

            var changeStatus = await _context.ChangeStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // GET: ChangeStatus/Create
        public IActionResult Create()
        {
            Models.ChangeStatus changeStatus = new Models.ChangeStatus
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };
            return View(changeStatus);
        }

        // POST: ChangeStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStatus changeStatus)
        {
            // Make sure duplicates are not entered...
            List<ChangeStatus> checkDupes = await _context.ChangeStatus
                .Where(m => m.Status == changeStatus.Status)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Status", "Change Status already exists.");
                return View(changeStatus);
            }

            if (ModelState.IsValid)
            {
                _context.Add(changeStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeStatus);
        }

        // GET: ChangeStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeStatus == null)
                 return NotFound();

            var changeStatus = await _context.ChangeStatus.FindAsync(id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // POST: ChangeStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStatus changeStatus)
        {
            if (id != changeStatus.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            //List<ChangeStatus> checkDupes = await _context.ChangeStatus
            //    .Where(m => m.Status == changeStatus.Status)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Status", "Change Status already exists.");
            //    return View(changeStatus);
            //}

            changeStatus.ModifiedUser = "Michael Wilson";
            changeStatus.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeStatusExists(changeStatus.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeStatus);
        }

        // GET: ChangeStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeStatus == null)
                return NotFound();

            var changeStatus = await _context.ChangeStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // POST: ChangeStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeStatus == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeStatus'  is null.");

            var changeStatus = await _context.ChangeStatus.FindAsync(id);

            if (changeStatus != null)
                _context.ChangeStatus.Remove(changeStatus);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeStatusExists(int id)
        {
          return (_context.ChangeStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
