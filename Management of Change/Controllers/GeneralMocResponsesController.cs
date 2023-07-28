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
    public class GeneralMocResponsesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public GeneralMocResponsesController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: GeneralMocResponses
        public async Task<IActionResult> Index()
        {
              return _context.GeneralMocResponses != null ? 
                          View(await _context.GeneralMocResponses.OrderBy(m => m.Order).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.GeneralMocResponses'  is null.");
        }

        // GET: GeneralMocResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GeneralMocResponses == null)
                return NotFound();

            var generalMocResponses = await _context.GeneralMocResponses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (generalMocResponses == null)
                return NotFound();

            return View(generalMocResponses);
        }

        // GET: GeneralMocResponses/Create
        public async Task<IActionResult> Create()
        {
            GeneralMocResponses generalMocResponses = new GeneralMocResponses
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            return View(generalMocResponses);
        }

        // POST: GeneralMocResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,Question,Response,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] GeneralMocResponses generalMocResponses)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalMocResponses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(generalMocResponses);
        }

        // GET: GeneralMocResponses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GeneralMocResponses == null)
                return NotFound();

            var generalMocResponses = await _context.GeneralMocResponses.FindAsync(id);

            if (generalMocResponses == null)
                return NotFound();

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            return View(generalMocResponses);
        }

        // POST: GeneralMocResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeRequestId,Question,Response,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] GeneralMocResponses generalMocResponses)
        {
            if (id != generalMocResponses.Id)
                return NotFound();

            generalMocResponses.ModifiedUser = "Michael Wilson";
            generalMocResponses.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalMocResponses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralMocResponsesExists(generalMocResponses.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "ChangeRequests", new { Id = generalMocResponses.ChangeRequestId, tab = "GeneralMocQuestions" });
            }
            return View(generalMocResponses);
        }

        // GET: GeneralMocResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GeneralMocResponses == null)
                return NotFound();

            var generalMocResponses = await _context.GeneralMocResponses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (generalMocResponses == null)
                return NotFound();

            return View(generalMocResponses);
        }

        // POST: GeneralMocResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GeneralMocResponses == null)
                return Problem("Entity set 'Management_of_ChangeContext.GeneralMocResponses'  is null.");

            var generalMocResponses = await _context.GeneralMocResponses.FindAsync(id);

            if (generalMocResponses != null)
                _context.GeneralMocResponses.Remove(generalMocResponses);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralMocResponsesExists(int id)
        {
          return (_context.GeneralMocResponses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //// GET: GeneralMocResponses/Edit/5
        //public async Task<IActionResult> Edit(int? id, int changeRequestId)
        //{
        //    if (id == null || _context.GeneralMocResponses == null)
        //        return NotFound();

        //    var generalMocResponses = await _context.GeneralMocResponses.FindAsync(id);

        //    if (generalMocResponses == null)
        //        return NotFound();

        //    return View(generalMocResponses);
        //}
    }
}
