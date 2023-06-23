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
    public class ImpactAssessmentResponseAnswersController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ImpactAssessmentResponseAnswersController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ImpactAssessmentResponseAnswers
        public async Task<IActionResult> Index()
        {
              return _context.ImpactAssessmentResponseAnswer != null ? 
                          View(await _context.ImpactAssessmentResponseAnswer.OrderBy(m => m.ImpactAssessmentResponseId).ThenBy(m => m.Order).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseAnswer'  is null.");
        }

        // GET: ImpactAssessmentResponseAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Create
        public async Task<IActionResult> Create()
        {
            ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer = new ImpactAssessmentResponseAnswer
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,Question,Order,Action,DetailsOfActionNeeded,PreOrPostImplementation,ActionOwner,DateDue,ImpactAssessmentResponseId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentResponseAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer.FindAsync(id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,Question,Order,Action,DetailsOfActionNeeded,PreOrPostImplementation,ActionOwner,DateDue,ImpactAssessmentResponseId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer)
        {
            if (id != impactAssessmentResponseAnswer.Id)
                return NotFound();

            impactAssessmentResponseAnswer.ModifiedUser = "Michael Wilson";
            impactAssessmentResponseAnswer.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(impactAssessmentResponseAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentResponseAnswerExists(impactAssessmentResponseAnswer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "ImpactAssessmentResponses", new { Id = impactAssessmentResponseAnswer.ImpactAssessmentResponseId });
            }
            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImpactAssessmentResponseAnswer == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseAnswer'  is null.");

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer.FindAsync(id);

            if (impactAssessmentResponseAnswer != null)
                _context.ImpactAssessmentResponseAnswer.Remove(impactAssessmentResponseAnswer);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentResponseAnswerExists(int id)
        {
          return (_context.ImpactAssessmentResponseAnswer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
