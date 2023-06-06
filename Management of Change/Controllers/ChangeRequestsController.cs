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
    public class ChangeRequestsController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeRequestsController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ChangeRequests
        public async Task<IActionResult> Index()
        {
              return _context.ChangeRequest != null ? 
                          View(await _context.ChangeRequest.ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeRequest'  is null.");
        }

        // GET: ChangeRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeRequest == null)
                return NotFound();

            return View(changeRequest);
        }

        // GET: ChangeRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChangeRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary")] ChangeRequest changeRequest)
        {
            if (ModelState.IsValid)
            {
                changeRequest.MOC_Number = "MOC-";
                changeRequest.Request_Date = DateTime.Now.Date;
                _context.Add(changeRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeRequest);
        }

        // GET: ChangeRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest.FindAsync(id);

            if (changeRequest == null)
                return NotFound();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MOC_Number,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Request_Date,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary")] ChangeRequest changeRequest)
        {
            if (id != changeRequest.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeRequestExists(changeRequest.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeRequest);
        }

        // GET: ChangeRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest == null)
                return NotFound();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeRequest == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.ChangeRequest'  is null.");
            }
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest != null)
            {
                _context.ChangeRequest.Remove(changeRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeRequestExists(int id)
        {
          return (_context.ChangeRequest?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
