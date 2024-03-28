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
    public class OriginatingGroupsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
    private readonly MocContext _contextMoc;

    public OriginatingGroupsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
    {
        _contextPtnWaiver = contextPtnWaiver;
        _contextMoc = contextMoc;
    }

        // GET: OriginatingGroups
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.OriginatingGroup != null ? 
                          View(await _contextPtnWaiver.OriginatingGroup.OrderBy(m=>m.Order).ThenBy(m=>m.Description).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.OriginatingGroup'  is null.");
        }

        // GET: OriginatingGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.OriginatingGroup == null)
                return NotFound();

            var originatingGroup = await _contextPtnWaiver.OriginatingGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (originatingGroup == null)
                return NotFound();
 
            return View(originatingGroup);
        }

        // GET: OriginatingGroups/Create
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

            OriginatingGroup originatingGroup = new OriginatingGroup()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(originatingGroup);
        }

        // POST: OriginatingGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description,BouleSizeRequired,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] OriginatingGroup originatingGroup)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<OriginatingGroup> checkDupes = await _contextPtnWaiver.OriginatingGroup
                .Where(m => m.Code == originatingGroup.Code)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "OriginatingGroup Code already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(originatingGroup);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(originatingGroup);
        }

        // GET: OriginatingGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.OriginatingGroup == null)
                return NotFound();

            var originatingGroup = await _contextPtnWaiver.OriginatingGroup.FindAsync(id);
            if (originatingGroup == null)
                return NotFound();

            return View(originatingGroup);
        }

        // POST: OriginatingGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Description,BouleSizeRequired,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] OriginatingGroup originatingGroup)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != originatingGroup.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<OriginatingGroup> checkDupes = await _contextPtnWaiver.OriginatingGroup
                .Where(m => m.Code == originatingGroup.Code && m.Id != originatingGroup.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "OriginatingGroup Code already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    originatingGroup.ModifiedDate = DateTime.Now;
                    originatingGroup.ModifiedUser = userInfo.onpremisessamaccountname;
                    originatingGroup.ModifiedUserFullName = userInfo.displayname;
                    originatingGroup.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(originatingGroup);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OriginatingGroupExists(originatingGroup.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(originatingGroup);
        }

        // GET: OriginatingGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.OriginatingGroup == null)
                return NotFound();

            var originatingGroup = await _contextPtnWaiver.OriginatingGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (originatingGroup == null)
                return NotFound();

            return View(originatingGroup);
        }

        // POST: OriginatingGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.OriginatingGroup == null)
                return Problem("Entity set 'PtnWaiverContext.OriginatingGroup'  is null.");
            var originatingGroup = await _contextPtnWaiver.OriginatingGroup.FindAsync(id);
            if (originatingGroup != null)
                _contextPtnWaiver.OriginatingGroup.Remove(originatingGroup);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OriginatingGroupExists(int id)
        {
          return (_contextPtnWaiver.OriginatingGroup?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
