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
    public class ImpactAssessmentResponseQuestionsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ImpactAssessmentResponseQuestionsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ImpactAssessmentResponseQuestions
        public async Task<IActionResult> Index()
        {
              return _context.ImpactAssessmentResponseQuestions != null ? 
                          View(await _context.ImpactAssessmentResponseQuestions.OrderBy(m => m.ReviewType).ThenBy(m => m.Order).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseQuestions'  is null.");
        }

        // GET: ImpactAssessmentResponseQuestions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.ImpactAssessmentResponseQuestions == null)
                return NotFound();

            var impactAssessmentResponseQuestions = await _context.ImpactAssessmentResponseQuestions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseQuestions == null)
                return NotFound();

            return View(impactAssessmentResponseQuestions);
        }

        // GET: ImpactAssessmentResponseQuestions/Create
        public async Task<IActionResult> Create()
        {
            ImpactAssessmentResponseQuestions impactAssessmentResponseQuestions = new ImpactAssessmentResponseQuestions
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentResponseQuestions);
        }

        // POST: ImpactAssessmentResponseQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,Question,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseQuestions impactAssessmentResponseQuestions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentResponseQuestions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponseQuestions);
        }

        // GET: ImpactAssessmentResponseQuestions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.ImpactAssessmentResponseQuestions == null)
                return NotFound();

            var impactAssessmentResponseQuestions = await _context.ImpactAssessmentResponseQuestions.FindAsync(id);

            if (impactAssessmentResponseQuestions == null)
                return NotFound();

            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentResponseQuestions);
        }

        // POST: ImpactAssessmentResponseQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,Question,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseQuestions impactAssessmentResponseQuestions)
        {
            if (id != impactAssessmentResponseQuestions.Id)
                return NotFound();

            impactAssessmentResponseQuestions.ModifiedUser = "Michael Wilson";
            impactAssessmentResponseQuestions.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(impactAssessmentResponseQuestions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentResponseQuestionsExists(impactAssessmentResponseQuestions.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponseQuestions);
        }

        // GET: ImpactAssessmentResponseQuestions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.ImpactAssessmentResponseQuestions == null)
                return NotFound();

            var impactAssessmentResponseQuestions = await _context.ImpactAssessmentResponseQuestions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseQuestions == null)
                return NotFound();

            return View(impactAssessmentResponseQuestions);
        }

        // POST: ImpactAssessmentResponseQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImpactAssessmentResponseQuestions == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseQuestions'  is null.");

            var impactAssessmentResponseQuestions = await _context.ImpactAssessmentResponseQuestions.FindAsync(id);

            if (impactAssessmentResponseQuestions != null)
                _context.ImpactAssessmentResponseQuestions.Remove(impactAssessmentResponseQuestions);
             
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentResponseQuestionsExists(int id)
        {
          return (_context.ImpactAssessmentResponseQuestions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
