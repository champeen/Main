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
    public class ChangeLevelsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeLevelsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ChangeLevels
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ChangeLevel != null ? 
                          View(await _context.ChangeLevel.OrderBy(m => m.Order).ThenBy(m => m.Level).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeLevel'  is null.");
        }

        public async Task<IActionResult> IndexHelp()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ChangeLevel != null ?
                          View(await _context.ChangeLevel.OrderBy(m => m.Order).ThenBy(m => m.Level).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeLevel'  is null.");
        }

        // GET: ChangeLevels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeLevel == null)
                 return NotFound();

            var changeLevel = await _context.ChangeLevel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeLevel == null)
                return NotFound();

            return View(changeLevel);
        }

        // GET: ChangeLevels/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ChangeLevel changeLevel = new ChangeLevel
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            return View(changeLevel);
        }

        // POST: ChangeLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Level,Description,ReviewRequired,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeLevel changeLevel)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ChangeLevel> checkDupes = await _context.ChangeLevel
                .Where(m => m.Level == changeLevel.Level)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Level", "Change Grade already exists.");
                return View(changeLevel);
            }

            if (ModelState.IsValid)
            {
                _context.Add(changeLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeLevel);
        }

        // GET: ChangeLevels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeLevel == null)
                return NotFound();

            var changeLevel = await _context.ChangeLevel.FindAsync(id);

            if (changeLevel == null)
                return NotFound();

            return View(changeLevel);
        }

        // POST: ChangeLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Level,Description,ReviewRequired,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeLevel changeLevel)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != changeLevel.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ChangeLevel> checkDupes = await _context.ChangeLevel
                .Where(m => m.Level == changeLevel.Level && m.Id != changeLevel.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Level", "Change Grade already exists.");
                return View(changeLevel);
            }

            changeLevel.ModifiedUser = _username;
            changeLevel.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeLevelExists(changeLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeLevel);
        }

        // GET: ChangeLevels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeLevel == null)
                return NotFound();

            var changeLevel = await _context.ChangeLevel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeLevel == null)
                return NotFound();

            return View(changeLevel);
        }

        // POST: ChangeLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ChangeLevel == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeLevel'  is null.");

            var changeLevel = await _context.ChangeLevel.FindAsync(id);

            if (changeLevel != null)
                _context.ChangeLevel.Remove(changeLevel);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeLevelExists(int id)
        {
          return (_context.ChangeLevel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
