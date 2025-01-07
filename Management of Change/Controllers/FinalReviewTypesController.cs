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

namespace Management_of_Change.Controllers
{
    public class FinalReviewTypesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public FinalReviewTypesController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: FinalReviewTypes
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.FinalReviewType != null ? 
                          View(await _context.FinalReviewType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.FinalReviewType'  is null.");
        }

        public async Task<IActionResult> IndexHelp()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.FinalReviewType != null ?
                          View(await _context.FinalReviewType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.FinalReviewType'  is null.");
        }

        // GET: FinalReviewTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.FinalReviewType == null)
                return NotFound();

            var finalReviewType = await _context.FinalReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (finalReviewType == null) 
                return NotFound();

            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            FinalReviewType finalReviewType = new FinalReviewType
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            ViewBag.Users = getUserList();

            return View(finalReviewType);
        }

        // POST: FinalReviewTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Username,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] FinalReviewType finalReviewType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<FinalReviewType> checkDupes = await _context.FinalReviewType
                .Where(m => m.Type == finalReviewType.Type)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Type", "Final Review Type already exists.");

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname.ToLower() == finalReviewType.Username.ToLower());
            if (employee != null)
            {
                finalReviewType.Reviewer = employee?.displayname;
                finalReviewType.Email = employee?.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + finalReviewType.Username);

            if (String.IsNullOrWhiteSpace(finalReviewType.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(finalReviewType.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(finalReviewType.Email))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (ModelState.IsValid)
            {
                _context.Add(finalReviewType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = getUserList();

            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.FinalReviewType == null)
                return NotFound();

            var finalReviewType = await _context.FinalReviewType.FindAsync(id);

            if (finalReviewType == null)
                return NotFound();

            ViewBag.Users = getUserList(finalReviewType.Username);

            return View(finalReviewType);
        }

        // POST: FinalReviewTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Username,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] FinalReviewType finalReviewType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != finalReviewType.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<FinalReviewType> checkDupes = await _context.FinalReviewType
                .Where(m => m.Type == finalReviewType.Type && m.Id != finalReviewType.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Type", "Final Review Type already exists.");

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname.ToLower() == finalReviewType.Username.ToLower());
            if (employee != null)
            {
                finalReviewType.Reviewer = employee?.displayname;
                finalReviewType.Email = employee?.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + finalReviewType.Username);

            if (String.IsNullOrWhiteSpace(finalReviewType.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(finalReviewType.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(finalReviewType.Email))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (ModelState.IsValid)
            {
                finalReviewType.ModifiedUser = _username;
                finalReviewType.ModifiedDate = DateTime.Now;
                try
                {
                    _context.Update(finalReviewType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinalReviewTypeExists(finalReviewType.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = getUserList(finalReviewType.Username);

            return View(finalReviewType);
        }

        // GET: FinalReviewTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.FinalReviewType == null)
                return NotFound();

            var finalReviewType = await _context.FinalReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (finalReviewType == null)
                return NotFound();

            return View(finalReviewType);
        }

        // POST: FinalReviewTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.FinalReviewType == null)
                return Problem("Entity set 'Management_of_ChangeContext.FinalReviewType'  is null.");

            var finalReviewType = await _context.FinalReviewType.FindAsync(id);

            if (finalReviewType != null)
                _context.FinalReviewType.Remove(finalReviewType);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinalReviewTypeExists(int id)
        {
          return (_context.FinalReviewType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
