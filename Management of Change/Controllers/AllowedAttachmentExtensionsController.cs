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
    public class AllowedAttachmentExtensionsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public AllowedAttachmentExtensionsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: AllowedAttachmentExtensions
        public async Task<IActionResult> Index()
        {
              return _context.AllowedAttachmentExtensions != null ? 
                          View(await _context.AllowedAttachmentExtensions.OrderBy(m => m.ExtensionName).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.AllowedAttachmentExtensions'  is null.");
        }

        // GET: AllowedAttachmentExtensions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _context.AllowedAttachmentExtensions.FirstOrDefaultAsync(m => m.Id == id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllowedAttachmentExtensions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExtensionName,Description")] AllowedAttachmentExtensions allowedAttachmentExtensions)
        {
            // Make sure duplicates are not entered...
            List<AllowedAttachmentExtensions> checkDupes = await _context.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == allowedAttachmentExtensions.ExtensionName)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("ExtensionName", "Extension already exists.");
                return View(allowedAttachmentExtensions);
            }

            if (ModelState.IsValid)
            {
                _context.Add(allowedAttachmentExtensions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _context.AllowedAttachmentExtensions.FindAsync(id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // POST: AllowedAttachmentExtensions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExtensionName,Description")] AllowedAttachmentExtensions allowedAttachmentExtensions)
        {
            if (id != allowedAttachmentExtensions.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allowedAttachmentExtensions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllowedAttachmentExtensionsExists(allowedAttachmentExtensions.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _context.AllowedAttachmentExtensions.FirstOrDefaultAsync(m => m.Id == id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // POST: AllowedAttachmentExtensions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AllowedAttachmentExtensions == null)
                return Problem("Entity set 'Management_of_ChangeContext.AllowedAttachmentExtensions'  is null.");

            var allowedAttachmentExtensions = await _context.AllowedAttachmentExtensions.FindAsync(id);

            if (allowedAttachmentExtensions != null)
                _context.AllowedAttachmentExtensions.Remove(allowedAttachmentExtensions);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AllowedAttachmentExtensionsExists(int id)
        {
          return (_context.AllowedAttachmentExtensions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
