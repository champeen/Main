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
    public class AdditionalImpactAssessmentReviewersController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public AdditionalImpactAssessmentReviewersController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: AdditionalImpactAssessmentReviewers
        public async Task<IActionResult> Index(int changeRequestId, string tab)
        {
            AdditionalImpactAssessmentReviewersVM vm = new AdditionalImpactAssessmentReviewersVM();
            vm.Tab = tab;
            vm.ChangeRequestId = changeRequestId;
            vm.AdditionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .Where(m => m.ChangeRequestId == changeRequestId)
                .ToListAsync();

              return View(vm);
        }

        // GET: AdditionalImpactAssessmentReviewers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (additionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }

            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdditionalImpactAssessmentReviewers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,Reviewer,ReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] AdditionalImpactAssessmentReviewers additionalImpactAssessmentReviewers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(additionalImpactAssessmentReviewers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers.FindAsync(id);
            if (additionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }
            return View(additionalImpactAssessmentReviewers);
        }

        // POST: AdditionalImpactAssessmentReviewers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeRequestId,Reviewer,ReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] AdditionalImpactAssessmentReviewers additionalImpactAssessmentReviewers)
        {
            if (id != additionalImpactAssessmentReviewers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(additionalImpactAssessmentReviewers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdditionalImpactAssessmentReviewersExists(additionalImpactAssessmentReviewers.Id))
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
            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (additionalImpactAssessmentReviewers == null)
            {
                return NotFound();
            }

            return View(additionalImpactAssessmentReviewers);
        }

        // POST: AdditionalImpactAssessmentReviewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdditionalImpactAssessmentReviewers == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.AdditionalImpactAssessmentReviewers'  is null.");
            }
            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers.FindAsync(id);
            if (additionalImpactAssessmentReviewers != null)
            {
                _context.AdditionalImpactAssessmentReviewers.Remove(additionalImpactAssessmentReviewers);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdditionalImpactAssessmentReviewersExists(int id)
        {
          return (_context.AdditionalImpactAssessmentReviewers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
