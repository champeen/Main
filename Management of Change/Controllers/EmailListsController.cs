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
    public class EmailListsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public EmailListsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: EmailLists
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.EmailLists != null ? 
                          View(await _context.EmailLists.OrderBy(m=>m.Order).ThenBy(m=>m.ListName).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.EmailLists'  is null.");
        }

        // GET: EmailLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.EmailLists == null)
                return NotFound();

            var emailLists = await _context.EmailLists.FirstOrDefaultAsync(m => m.Id == id);
            if (emailLists == null)
                return NotFound();

            return View(emailLists);
        }

        // GET: EmailLists/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Employees = getUserEmailList();

            EmailLists emailList = new EmailLists
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            return View(emailList);
        }

        // POST: EmailLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ListName,Emails,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] EmailLists emailLists)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<EmailLists> checkDupes = await _context.EmailLists
                .Where(m => m.ListName == emailLists.ListName)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("ListName", "Email List Name already exists.");

            if (ModelState.IsValid)
            {
                _context.Add(emailLists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = getUserEmailList();
            return View(emailLists);
        }

        // GET: EmailLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;            

            if (id == null || _context.EmailLists == null)
                return NotFound();

            var emailLists = await _context.EmailLists.FindAsync(id);
            if (emailLists == null)
                return NotFound();

            ViewBag.Employees = getUserEmailList(emailLists.Emails);

            return View(emailLists);
        }

        // POST: EmailLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ListName,Emails,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] EmailLists emailLists)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != emailLists.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<EmailLists> checkDupes = await _context.EmailLists
                .Where(m => m.ListName == emailLists.ListName && m.Id != emailLists.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("ListName", "Change Area already exists.");

            if (ModelState.IsValid)
            {
                try
                {
                    emailLists.ModifiedDate = DateTime.Now;
                    emailLists.ModifiedUser = _username;
                    _context.Update(emailLists);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailListsExists(emailLists.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = getUserEmailList(emailLists.Emails);
            return View(emailLists);
        }

        // GET: EmailLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.EmailLists == null)
                return NotFound();

            var emailLists = await _context.EmailLists.FirstOrDefaultAsync(m => m.Id == id);
            if (emailLists == null)
                return NotFound();

            return View(emailLists);
        }

        // POST: EmailLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.EmailLists == null)
                return Problem("Entity set 'Management_of_ChangeContext.EmailLists'  is null.");

            var emailLists = await _context.EmailLists.FindAsync(id);
            if (emailLists != null)
                _context.EmailLists.Remove(emailLists);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailListsExists(int id)
        {
          return (_context.EmailLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
