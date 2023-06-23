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
    public class ImplementationFinalApprovalResponsesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ImplementationFinalApprovalResponsesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ImplementationFinalApprovalResponses
        public async Task<IActionResult> Index()
        {
              return _context.ImplementationFinalApprovalResponse != null ? 
                          View(await _context.ImplementationFinalApprovalResponse.OrderBy(m => m.FinalReviewType).ThenBy(m => m.ChangeType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalResponse'  is null.");
        }

        // GET: ImplementationFinalApprovalResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImplementationFinalApprovalResponse == null)
            {
                return NotFound();
            }

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (implementationFinalApprovalResponse == null)
                return NotFound();

            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Create
        public IActionResult Create()
        {
            ImplementationFinalApprovalResponse implementationFinalApprovalResponse = new ImplementationFinalApprovalResponse
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeType,FinalReviewType,Reviewer,ReviewerEmail,ReviewResult,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalResponse implementationFinalApprovalResponse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(implementationFinalApprovalResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImplementationFinalApprovalResponse == null)
                return NotFound();

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FindAsync(id);

            if (implementationFinalApprovalResponse == null)
                return NotFound();

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeType,FinalReviewType,Reviewer,ReviewerEmail,ReviewResult,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalResponse implementationFinalApprovalResponse)
        {
            if (id != implementationFinalApprovalResponse.Id)
                return NotFound();

            implementationFinalApprovalResponse.ModifiedUser = "Michael Wilson";
            implementationFinalApprovalResponse.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(implementationFinalApprovalResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImplementationFinalApprovalResponseExists(implementationFinalApprovalResponse.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "ChangeRequests", new { Id = implementationFinalApprovalResponse.ChangeRequestId });
            }
            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImplementationFinalApprovalResponse == null)
                return NotFound();

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse
                .FirstOrDefaultAsync(m => m.Id == id);

            if (implementationFinalApprovalResponse == null)
                return NotFound();

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImplementationFinalApprovalResponse == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalResponse'  is null.");

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FindAsync(id);

            if (implementationFinalApprovalResponse != null)
                _context.ImplementationFinalApprovalResponse.Remove(implementationFinalApprovalResponse);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImplementationFinalApprovalResponseExists(int id)
        {
          return (_context.ImplementationFinalApprovalResponse?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
