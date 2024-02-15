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
    public class PtnStatusController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public PtnStatusController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: PtnStatus
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.PtnStatus != null ? 
                          View(await _contextPtnWaiver.PtnStatus.OrderBy(m=>m.Order).ThenBy(m=>m.Status).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.PtnStatus'  is null.");
        }

        // GET: PtnStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PtnStatus == null)
                return NotFound();

            var ptnStatus = await _contextPtnWaiver.PtnStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ptnStatus == null)
                return NotFound();

            return View(ptnStatus);
        }

        // GET: PtnStatus/Create
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

            PtnStatus ptnStatus = new PtnStatus
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(ptnStatus);
        }

        // POST: PtnStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Description,Default,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] PtnStatus ptnStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<PtnStatus> checkDupes = await _contextPtnWaiver.PtnStatus
                .Where(m => m.Status == ptnStatus.Status)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Status", "PTN Status already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(ptnStatus);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ptnStatus);
        }

        // GET: PtnStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PtnStatus == null)
                return NotFound();

            var ptnStatus = await _contextPtnWaiver.PtnStatus.FindAsync(id);

            if (ptnStatus == null)
                return NotFound();

            return View(ptnStatus);
        }

        // POST: PtnStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Description,Default,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] PtnStatus ptnStatus)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != ptnStatus.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<PtnStatus> checkDupes = await _contextPtnWaiver.PtnStatus
                .Where(m => m.Status == ptnStatus.Status && m.Id != ptnStatus.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Status", "PTN Status already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo == null)
                {
                    ptnStatus.ModifiedDate = DateTime.Now;
                    ptnStatus.ModifiedUser = userInfo.onpremisessamaccountname;
                    ptnStatus.ModifiedUserFullName = userInfo.displayname;
                    ptnStatus.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(ptnStatus);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PtnStatusExists(ptnStatus.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ptnStatus);
        }

        // GET: PtnStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PtnStatus == null)
                return NotFound();

            var ptnStatus = await _contextPtnWaiver.PtnStatus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ptnStatus == null)
                return NotFound();

            return View(ptnStatus);
        }

        // POST: PtnStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.PtnStatus == null)
                return Problem("Entity set 'PtnWaiverContext.PtnStatus'  is null.");

            var ptnStatus = await _contextPtnWaiver.PtnStatus.FindAsync(id);

            if (ptnStatus != null)
                _contextPtnWaiver.PtnStatus.Remove(ptnStatus);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PtnStatusExists(int id)
        {
          return (_contextPtnWaiver.PtnStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
