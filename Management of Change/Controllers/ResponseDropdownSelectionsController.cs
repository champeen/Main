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
using Management_of_Change.ViewModels;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ResponseDropdownSelectionsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public ResponseDropdownSelectionsController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: ResponseDropdownSelections
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ResponseDropdownSelections != null ? 
                          View(await _context.ResponseDropdownSelections.OrderBy(m => m.Order).ThenBy(m => m.Response).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ResponseDropdownSelections'  is null.");
        }

        // GET: ResponseDropdownSelections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ResponseDropdownSelections == null)
                return NotFound();

            var responseDropdownSelections = await _context.ResponseDropdownSelections
                .FirstOrDefaultAsync(m => m.Id == id);

            if (responseDropdownSelections == null)
                return NotFound();

            return View(responseDropdownSelections);
        }

        // GET: ResponseDropdownSelections/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Models.ResponseDropdownSelections model = new Models.ResponseDropdownSelections
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            return View(model);
        }

        // POST: ResponseDropdownSelections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Response,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ResponseDropdownSelections responseDropdownSelections)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ResponseDropdownSelections> checkDupes = await _context.ResponseDropdownSelections
                .Where(m => m.Response == responseDropdownSelections.Response)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Response", "Response already exists.");
                return View(responseDropdownSelections);
            }

            if (ModelState.IsValid)
            {
                _context.Add(responseDropdownSelections);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(responseDropdownSelections);
        }

        // GET: ResponseDropdownSelections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ResponseDropdownSelections == null)
                 return NotFound();
 
            var responseDropdownSelections = await _context.ResponseDropdownSelections.FindAsync(id);

            if (responseDropdownSelections == null)
                return NotFound();
            return View(responseDropdownSelections);
        }

        // POST: ResponseDropdownSelections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Response,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ResponseDropdownSelections responseDropdownSelections)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != responseDropdownSelections.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            //List<ResponseDropdownSelections> checkDupes = await _context.ResponseDropdownSelections
            //    .Where(m => m.Response == responseDropdownSelections.Response)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Response", "Response already exists.");
            //    return View(responseDropdownSelections);
            //}

            responseDropdownSelections.ModifiedUser = _username;
            responseDropdownSelections.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(responseDropdownSelections);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResponseDropdownSelectionsExists(responseDropdownSelections.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(responseDropdownSelections);
        }

        // GET: ResponseDropdownSelections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ResponseDropdownSelections == null)
                return NotFound();
 
            var responseDropdownSelections = await _context.ResponseDropdownSelections
                .FirstOrDefaultAsync(m => m.Id == id);

            if (responseDropdownSelections == null)
                return NotFound();
 
            return View(responseDropdownSelections);
        }

        // POST: ResponseDropdownSelections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ResponseDropdownSelections == null)
                return Problem("Entity set 'Management_of_ChangeContext.ResponseDropdownSelections'  is null.");

            var responseDropdownSelections = await _context.ResponseDropdownSelections.FindAsync(id);

            if (responseDropdownSelections != null)
                _context.ResponseDropdownSelections.Remove(responseDropdownSelections);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResponseDropdownSelectionsExists(int id)
        {
          return (_context.ResponseDropdownSelections?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
