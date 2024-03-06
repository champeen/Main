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
    public class EmailHistoriesController : Controller
    {
        private readonly PtnWaiverContext _context;

        public EmailHistoriesController(PtnWaiverContext context)
        {
            _context = context;
        }

        // GET: EmailHistories
        public async Task<IActionResult> Index()
        {
              return _context.EmailHistory != null ? 
                          View(await _context.EmailHistory.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.EmailHistory'  is null.");
        }

        // GET: EmailHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmailHistory == null)
            {
                return NotFound();
            }

            var emailHistory = await _context.EmailHistory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailHistory == null)
            {
                return NotFound();
            }

            return View(emailHistory);
        }

        // GET: EmailHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Priority,SentToDisplayName,SentToUsername,SentToEmail,Subject,Body,PtnId,WaiverId,TaskId,Type,Status,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] EmailHistory emailHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailHistory);
        }

        // GET: EmailHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmailHistory == null)
            {
                return NotFound();
            }

            var emailHistory = await _context.EmailHistory.FindAsync(id);
            if (emailHistory == null)
            {
                return NotFound();
            }
            return View(emailHistory);
        }

        // POST: EmailHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Priority,SentToDisplayName,SentToUsername,SentToEmail,Subject,Body,PtnId,WaiverId,TaskId,Type,Status,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] EmailHistory emailHistory)
        {
            if (id != emailHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailHistoryExists(emailHistory.Id))
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
            return View(emailHistory);
        }

        // GET: EmailHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EmailHistory == null)
            {
                return NotFound();
            }

            var emailHistory = await _context.EmailHistory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailHistory == null)
            {
                return NotFound();
            }

            return View(emailHistory);
        }

        // POST: EmailHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmailHistory == null)
            {
                return Problem("Entity set 'PtnWaiverContext.EmailHistory'  is null.");
            }
            var emailHistory = await _context.EmailHistory.FindAsync(id);
            if (emailHistory != null)
            {
                _context.EmailHistory.Remove(emailHistory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailHistoryExists(int id)
        {
          return (_context.EmailHistory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
