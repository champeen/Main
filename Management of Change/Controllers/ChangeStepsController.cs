using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeStepsController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeStepsController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ChangeSteps
        public async Task<IActionResult> Index()
        {
              return _context.ChangeStep != null ? 
                          View(await _context.ChangeStep.OrderBy(m => m.Order).ThenBy(m => m.Step).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeStep'  is null.");
        }

        // GET: ChangeSteps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeStep == null)
                return NotFound();

            var changeStep = await _context.ChangeStep
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStep == null)
                return NotFound();

            return View(changeStep);
        }

        // GET: ChangeSteps/Create
        public IActionResult Create()
        {
            Models.ChangeStep changeStep = new Models.ChangeStep
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };
            return View(changeStep);
        }

        // POST: ChangeSteps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Step,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStep changeStep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(changeStep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeStep);
        }

        // GET: ChangeSteps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeStep == null)
                 return NotFound();

            var changeStep = await _context.ChangeStep.FindAsync(id);

            if (changeStep == null)
                return NotFound();

            return View(changeStep);
        }

        // POST: ChangeSteps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Step,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStep changeStep)
        {
            if (id != changeStep.Id)
                return NotFound();

            changeStep.ModifiedUser = "Michael Wilson";
            changeStep.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeStepExists(changeStep.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeStep);
        }

        // GET: ChangeSteps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeStep == null)
                return NotFound();

            var changeStep = await _context.ChangeStep
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStep == null)
                return NotFound();

            return View(changeStep);
        }

        // POST: ChangeSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeStep == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeStep'  is null.");

            var changeStep = await _context.ChangeStep.FindAsync(id);

            if (changeStep != null)
                _context.ChangeStep.Remove(changeStep);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeStepExists(int id)
        {
          return (_context.ChangeStep?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
