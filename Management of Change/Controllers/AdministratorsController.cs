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
    public class AdministratorsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        private string userName;

        public AdministratorsController(Management_of_ChangeContext context ) : base (context)
        {
            _context = context;
        }

        // GET: Administrators
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;


            //if (String.IsNullOrWhiteSpace(_username))
            //    return RedirectToAction("Error", "Home", new { errorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." });

            //if (!_isAuthorized)
            //    return RedirectToAction("Error", "Home", new {errorMessage = "User " + _username + " Unauthorized - Not Setup as Active Employee. Contact MoC Admin."});

            //if (!_isAdmin)
            //    return RedirectToAction("Error", "Home", new { errorMessage = "Must be setup as an Administrator to have access." });

              return _context.Administrators != null ? 
                          View(await _context.Administrators.OrderBy(m=>m.Username).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.Administrators'  is null.");
        }

        // GET: Administrators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.Administrators == null)
                return NotFound();

            var administrators = await _context.Administrators.FirstOrDefaultAsync(m => m.Id == id);
            if (administrators == null)
                return NotFound();

            return View(administrators);
        }

        // GET: Administrators/Create
        public async Task<IActionResult> Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            Administrators administrators = new Administrators
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            var employeeList = await _context.__mst_employee
                    .Where(m => m.accountenabled == true)
                    .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                    .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                    .OrderBy(m => m.displayname)
                    .ToListAsync();

            List<SelectListItem> employees = new List<SelectListItem>();
            foreach (var employee in employeeList)
            {
                SelectListItem item = new SelectListItem { Value = employee.onpremisessamaccountname, Text = employee.displayname + " : (" + employee.onpremisessamaccountname + ")" };
                employees.Add(item);
            }
            ViewBag.Employees = employees;

            return View(administrators);
        }

        // POST: Administrators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Approver,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Administrators administrators)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                _context.Add(administrators);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(administrators);
        }

        // GET: Administrators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.Administrators == null)
                return NotFound();

            var administrators = await _context.Administrators.FindAsync(id);
            if (administrators == null)
                return NotFound();

            return View(administrators);
        }

        // POST: Administrators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Approver,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Administrators administrators)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != administrators.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrators);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministratorsExists(administrators.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(administrators);
        }

        // GET: Administrators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.Administrators == null)
                return NotFound();

            var administrators = await _context.Administrators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrators == null)
                return NotFound();

            return View(administrators);
        }

        // POST: Administrators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.Administrators == null)
                return Problem("Entity set 'Management_of_ChangeContext.Administrators'  is null.");

            var administrators = await _context.Administrators.FindAsync(id);
            if (administrators != null)
                _context.Administrators.Remove(administrators);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorsExists(int id)
        {
          return (_context.Administrators?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
