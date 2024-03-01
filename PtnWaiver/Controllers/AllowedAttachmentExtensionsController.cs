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
    public class AllowedAttachmentExtensionsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;
        private readonly ILogger<AdminController> _logger;

        public AllowedAttachmentExtensionsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc, ILogger<AdminController> logger) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
            _logger = logger;
        }

            // GET: AllowedAttachmentExtensions
            public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.AllowedAttachmentExtensions != null ?
                          View(await _contextPtnWaiver.AllowedAttachmentExtensions.OrderBy(m => m.ExtensionName).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.AllowedAttachmentExtensions'  is null.");
        }

        public async Task<IActionResult> IndexHelp()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.AllowedAttachmentExtensions != null ?
                          View(await _contextPtnWaiver.AllowedAttachmentExtensions.OrderBy(m => m.ExtensionName).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.AllowedAttachmentExtensions'  is null.");
        }

        // GET: AllowedAttachmentExtensions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _contextPtnWaiver.AllowedAttachmentExtensions.FirstOrDefaultAsync(m => m.Id == id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View();
        }

        // POST: AllowedAttachmentExtensions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExtensionName,Description")] AllowedAttachmentExtensions allowedAttachmentExtensions)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<AllowedAttachmentExtensions> checkDupes = await _contextPtnWaiver.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == allowedAttachmentExtensions.ExtensionName)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("ExtensionName", "Extension already exists.");
                return View(allowedAttachmentExtensions);
            }

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(allowedAttachmentExtensions);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _contextPtnWaiver.AllowedAttachmentExtensions.FindAsync(id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // POST: AllowedAttachmentExtensions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExtensionName,Description")] AllowedAttachmentExtensions allowedAttachmentExtensions)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != allowedAttachmentExtensions.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contextPtnWaiver.Update(allowedAttachmentExtensions);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllowedAttachmentExtensionsExists(allowedAttachmentExtensions.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(allowedAttachmentExtensions);
        }

        // GET: AllowedAttachmentExtensions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.AllowedAttachmentExtensions == null)
                return NotFound();

            var allowedAttachmentExtensions = await _contextPtnWaiver.AllowedAttachmentExtensions.FirstOrDefaultAsync(m => m.Id == id);

            if (allowedAttachmentExtensions == null)
                return NotFound();

            return View(allowedAttachmentExtensions);
        }

        // POST: AllowedAttachmentExtensions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.AllowedAttachmentExtensions == null)
                return Problem("Entity set 'Management_of_ChangeContext.AllowedAttachmentExtensions'  is null.");

            var allowedAttachmentExtensions = await _contextPtnWaiver.AllowedAttachmentExtensions.FindAsync(id);

            if (allowedAttachmentExtensions != null)
                _contextPtnWaiver.AllowedAttachmentExtensions.Remove(allowedAttachmentExtensions);

            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AllowedAttachmentExtensionsExists(int id)
        {
            return (_contextPtnWaiver.AllowedAttachmentExtensions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
