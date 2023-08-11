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
    public class ChangeStatusController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeStatusController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ChangeSteps
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ChangeStatus != null ? 
                          View(await _context.ChangeStatus.OrderBy(m => m.Order).ThenBy(m => m.Status).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeStatus'  is null.");
        }

        // GET: ChangeStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeStatus == null)
                return NotFound();

            var changeStatus = await _context.ChangeStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // GET: ChangeStatus/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Models.ChangeStatus changeStatus = new Models.ChangeStatus
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };
            return View(changeStatus);
        }

        // POST: ChangeStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Default,Status,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStatus changeStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ChangeStatus> checkDupes = await _context.ChangeStatus
                .Where(m => m.Status == changeStatus.Status)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Status", "Change Status already exists.");
                return View(changeStatus);
            }

            if (ModelState.IsValid)
            {
                _context.Add(changeStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeStatus);
        }

        // GET: ChangeStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeStatus == null)
                 return NotFound();

            var changeStatus = await _context.ChangeStatus.FindAsync(id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // POST: ChangeStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Default,Status,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.ChangeStatus changeStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != changeStatus.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            //List<ChangeStatus> checkDupes = await _context.ChangeStatus
            //    .Where(m => m.Status == changeStatus.Status)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Status", "Change Status already exists.");
            //    return View(changeStatus);
            //}

            changeStatus.ModifiedUser = _username;
            changeStatus.ModifiedDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeStatusExists(changeStatus.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeStatus);
        }

        // GET: ChangeStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeStatus == null)
                return NotFound();

            var changeStatus = await _context.ChangeStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeStatus == null)
                return NotFound();

            return View(changeStatus);
        }

        // POST: ChangeStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ChangeStatus == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeStatus'  is null.");

            var changeStatus = await _context.ChangeStatus.FindAsync(id);

            if (changeStatus != null)
                _context.ChangeStatus.Remove(changeStatus);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeStatusExists(int id)
        {
          return (_context.ChangeStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
