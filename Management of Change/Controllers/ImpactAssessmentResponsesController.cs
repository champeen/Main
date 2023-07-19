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
    public class ImpactAssessmentResponsesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ImpactAssessmentResponsesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ImpactAssessmentResponses
        public async Task<IActionResult> Index()
        {
              return _context.ImpactAssessmentResponse != null ? 
                          View(await _context.ImpactAssessmentResponse.OrderBy(m => m.ReviewType).ThenBy(m => m.ChangeType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponse'  is null.");
        }

        // GET: ImpactAssessmentResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponse == null)
                return NotFound();

            // Get all the ImpactAssessmentResponsesQuestions/Answers associated with this request...
            impactAssessmentResponse.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
                    .Where(m => m.ImpactAssessmentResponseId == impactAssessmentResponse.Id)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

            // Get all tasks associated with each ImpactAssessmentResponseAnswer
            foreach (var record in impactAssessmentResponse.ImpactAssessmentResponseAnswers)
            {
                Models.Task task = await _context.Task.FirstOrDefaultAsync(m => m.ImpactAssessmentResponseAnswerId == record.Id);
                record.Task = task;
            }

            return View(impactAssessmentResponse);
        }

        // GET: ImpactAssessmentResponses/Create
        public IActionResult Create()
        {
            ImpactAssessmentResponse impactAssessmentResponse = new ImpactAssessmentResponse
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            //ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            //ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,ChangeType,Reviewer,ReviewerEmail,Required,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponse impactAssessmentResponse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponse);
        }

        // GET: ImpactAssessmentResponses/Edit/5
        public async Task<IActionResult> Edit(int? id, string tab = "ImpactAssessments")
        {
            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse.FindAsync(id);

            if (impactAssessmentResponse == null)
                return NotFound();

            ViewBag.Tab = tab;

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,ChangeType,Reviewer,ReviewerEmail,Required,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponse impactAssessmentResponse, string tab = "ImpactAssessments")
        {
            if (id != impactAssessmentResponse.Id)
                return NotFound();

            impactAssessmentResponse.ModifiedUser = "Michael Wilson";
            impactAssessmentResponse.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(impactAssessmentResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentResponseExists(impactAssessmentResponse.Id))
                        return NotFound();
                    else
                        throw;
                }
                if (tab == "IARDetails")
                    return RedirectToAction("Details", "ImpactAssessmentResponses", new { Id = impactAssessmentResponse.Id, tab = tab });
                else
                    return RedirectToAction("Details", "ChangeRequests", new { Id = impactAssessmentResponse.ChangeRequestId, tab=tab });
            }
            return View(impactAssessmentResponse);
        }

        // GET: ImpactAssessmentResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponse == null)
                return NotFound();

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImpactAssessmentResponse == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponse'  is null.");

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse.FindAsync(id);

            if (impactAssessmentResponse != null)
                _context.ImpactAssessmentResponse.Remove(impactAssessmentResponse);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentResponseExists(int id)
        {
          return (_context.ImpactAssessmentResponse?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
