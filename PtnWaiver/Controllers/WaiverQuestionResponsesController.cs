using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;

namespace PtnWaiver.Controllers
{
    public class WaiverQuestionResponsesController : Controller
    {
        private readonly PtnWaiverContext _context;

        public WaiverQuestionResponsesController(PtnWaiverContext context)
        {
            _context = context;
        }

        // GET: WaiverQuestionResponses
        public async Task<IActionResult> Index()
        {
              return _context.WaiverQuestionResponse != null ? 
                          View(await _context.WaiverQuestionResponse.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.WaiverQuestionResponse'  is null.");
        }

        // GET: WaiverQuestionResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WaiverQuestionResponse == null)
            {
                return NotFound();
            }

            var waiverQuestionResponse = await _context.WaiverQuestionResponse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waiverQuestionResponse == null)
            {
                return NotFound();
            }

            return View(waiverQuestionResponse);
        }

        // GET: WaiverQuestionResponses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaiverQuestionResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WaiverId,Question,GroupApprover,Order,Response,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] WaiverQuestionResponse waiverQuestionResponse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waiverQuestionResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waiverQuestionResponse);
        }

        // GET: WaiverQuestionResponses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WaiverQuestionResponse == null)
            {
                return NotFound();
            }

            var waiverQuestionResponse = await _context.WaiverQuestionResponse.FindAsync(id);
            if (waiverQuestionResponse == null)
            {
                return NotFound();
            }
            return View(waiverQuestionResponse);
        }

        // POST: WaiverQuestionResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WaiverId,Question,GroupApprover,Order,Response,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] WaiverQuestionResponse waiverQuestionResponse)
        {
            if (id != waiverQuestionResponse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waiverQuestionResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaiverQuestionResponseExists(waiverQuestionResponse.Id))
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
            return View(waiverQuestionResponse);
        }

        // GET: WaiverQuestionResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WaiverQuestionResponse == null)
            {
                return NotFound();
            }

            var waiverQuestionResponse = await _context.WaiverQuestionResponse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waiverQuestionResponse == null)
            {
                return NotFound();
            }

            return View(waiverQuestionResponse);
        }

        // POST: WaiverQuestionResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WaiverQuestionResponse == null)
            {
                return Problem("Entity set 'PtnWaiverContext.WaiverQuestionResponse'  is null.");
            }
            var waiverQuestionResponse = await _context.WaiverQuestionResponse.FindAsync(id);
            if (waiverQuestionResponse != null)
            {
                _context.WaiverQuestionResponse.Remove(waiverQuestionResponse);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaiverQuestionResponseExists(int id)
        {
          return (_context.WaiverQuestionResponse?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
