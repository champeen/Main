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
    public class PorProjectsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
    private readonly MocContext _contextMoc;

    public PorProjectsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
    {
        _contextPtnWaiver = contextPtnWaiver;
        _contextMoc = contextMoc;
    }

        // GET: PorProjects
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.PorProject != null ? 
                          View(await _contextPtnWaiver.PorProject.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.PorProject'  is null.");
        }

        // GET: PorProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PorProject == null)
                return NotFound();

            var porProject = await _contextPtnWaiver.PorProject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (porProject == null)
                return NotFound();
 
            return View(porProject);
        }

        // GET: PorProjects/Create
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

            PorProject porProject = new PorProject()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(porProject);
        }

        // POST: PorProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] PorProject porProject)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<PorProject> checkDupes = await _contextPtnWaiver.PorProject
                .Where(m => m.Code == porProject.Code)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "PorProject Code already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(porProject);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(porProject);
        }

        // GET: PorProjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PorProject == null)
                return NotFound();

            var porProject = await _contextPtnWaiver.PorProject.FindAsync(id);
            if (porProject == null)
                return NotFound();

            return View(porProject);
        }

        // POST: PorProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] PorProject porProject)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != porProject.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<PorProject> checkDupes = await _contextPtnWaiver.PorProject
                .Where(m => m.Code == porProject.Code && m.Id != porProject.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "PorProject Code already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    porProject.ModifiedDate = DateTime.Now;
                    porProject.ModifiedUser = userInfo.onpremisessamaccountname;
                    porProject.ModifiedUserFullName = userInfo.displayname;
                    porProject.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(porProject);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PorProjectExists(porProject.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(porProject);
        }

        // GET: PorProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PorProject == null)
                return NotFound();

            var porProject = await _contextPtnWaiver.PorProject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (porProject == null)
                return NotFound();

            return View(porProject);
        }

        // POST: PorProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.PorProject == null)
                return Problem("Entity set 'PtnWaiverContext.PorProject'  is null.");
            var porProject = await _contextPtnWaiver.PorProject.FindAsync(id);
            if (porProject != null)
                _contextPtnWaiver.PorProject.Remove(porProject);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PorProjectExists(int id)
        {
          return (_contextPtnWaiver.PorProject?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
