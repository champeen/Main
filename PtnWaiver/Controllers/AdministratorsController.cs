using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class AdministratorsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public AdministratorsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: Administrators
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.Administrators != null ? 
                          View(await _contextPtnWaiver.Administrators.OrderBy(m=>m.Username).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.Administrators'  is null.");
        }

        // GET: Administrators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.Administrators == null)
                return NotFound();

            var administrators = await _contextPtnWaiver.Administrators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrators == null)
                return NotFound();

            return View(administrators);
        }

        // GET: Administrators/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            var userInfo = getUserInfo(_username);
            if (userInfo == null)
            {
                errorViewModel = new ErrorViewModel() { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = "Invalid Username: " + _username });
            }

            Administrators administrators = new Administrators()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            ViewBag.Users = getUserList();

            return View(administrators);
        }

        // POST: Administrators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Approver,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] Administrators administrators)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<Administrators> checkDupes = await _contextPtnWaiver.Administrators
                .Where(m => m.Username == administrators.Username)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Username", "Username already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(administrators);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = getUserList(administrators.Username);
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
            
            if (id == null || _contextPtnWaiver.Administrators == null)
                return NotFound();

            var administrators = await _contextPtnWaiver.Administrators.FindAsync(id);

            if (administrators == null)
                return NotFound();

            ViewBag.Users = getUserList(administrators.Username);

            return View(administrators);
        }

        // POST: Administrators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Approver,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] Administrators administrators)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != administrators.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<Administrators> checkDupes = await _contextPtnWaiver.Administrators
                .Where(m => m.Username == administrators.Username && m.Id != administrators.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Username", "Username already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    administrators.ModifiedDate = DateTime.Now;
                    administrators.ModifiedUser = userInfo.onpremisessamaccountname;
                    administrators.ModifiedUserFullName = userInfo.displayname;
                    administrators.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(administrators);
                    await _contextPtnWaiver.SaveChangesAsync();
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

            ViewBag.Users = getUserList(administrators.Username);
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

            if (id == null || _contextPtnWaiver.Administrators == null)
                return NotFound();

            var administrators = await _contextPtnWaiver.Administrators
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

            if (_contextPtnWaiver.Administrators == null)
                return Problem("Entity set 'PtnWaiverContext.Administrators'  is null.");
 
            var administrators = await _contextPtnWaiver.Administrators.FindAsync(id);

            if (administrators != null)
                _contextPtnWaiver.Administrators.Remove(administrators);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorsExists(int id)
        {
          return (_contextPtnWaiver.Administrators?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
