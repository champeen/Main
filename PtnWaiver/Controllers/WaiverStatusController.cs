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
    public class WaiverStatusController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public WaiverStatusController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: WaiverStatus
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.WaiverStatus != null ? 
                          View(await _contextPtnWaiver.WaiverStatus.OrderBy(m=>m.Order).ThenBy(m=>m.Status).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.WaiverStatus'  is null.");
        }

        // GET: WaiverStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.WaiverStatus == null)
                return NotFound();

            var waiverStatus = await _contextPtnWaiver.WaiverStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiverStatus == null)
                return NotFound();

            return View(waiverStatus);
        }

        // GET: WaiverStatus/Create
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

            WaiverStatus waiverStatus = new WaiverStatus
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(waiverStatus);
        }

        // POST: WaiverStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Description,Default,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] WaiverStatus waiverStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<WaiverStatus> checkDupes = await _contextPtnWaiver.WaiverStatus
                .Where(m => m.Status == waiverStatus.Status)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Status", "Waiver Status already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(waiverStatus);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waiverStatus);
        }

        // GET: WaiverStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.WaiverStatus == null)
                return NotFound();

            var waiverStatus = await _contextPtnWaiver.WaiverStatus.FindAsync(id);

            if (waiverStatus == null)
                return NotFound();

            return View(waiverStatus);
        }

        // POST: WaiverStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Description,Default,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] WaiverStatus waiverStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != waiverStatus.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<WaiverStatus> checkDupes = await _contextPtnWaiver.WaiverStatus
                .Where(m => m.Status == waiverStatus.Status && m.Id != waiverStatus.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Status", "Waiver Status already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo == null)
                {
                    waiverStatus.ModifiedDate = DateTime.Now;
                    waiverStatus.ModifiedUser = userInfo.onpremisessamaccountname;
                    waiverStatus.ModifiedUserFullName = userInfo.displayname;
                    waiverStatus.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(waiverStatus);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaiverStatusExists(waiverStatus.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(waiverStatus);
        }

        // GET: WaiverStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.WaiverStatus == null)
                return NotFound();

            var waiverStatus = await _contextPtnWaiver.WaiverStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (waiverStatus == null)
                return NotFound();

            return View(waiverStatus);
        }

        // POST: WaiverStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.WaiverStatus == null)
                return Problem("Entity set 'PtnWaiverContext.WaiverStatus'  is null.");

            var waiverStatus = await _contextPtnWaiver.WaiverStatus.FindAsync(id);

            if (waiverStatus != null)
                _contextPtnWaiver.WaiverStatus.Remove(waiverStatus);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaiverStatusExists(int id)
        {
          return (_contextPtnWaiver.WaiverStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
