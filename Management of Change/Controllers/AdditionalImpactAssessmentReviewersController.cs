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
    public class AdditionalImpactAssessmentReviewersController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public AdditionalImpactAssessmentReviewersController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: AdditionalImpactAssessmentReviewers
        public async Task<IActionResult> Index(int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // get requested list of additional impact reviewers....
            AdditionalImpactAssessmentReviewersVM vm = new AdditionalImpactAssessmentReviewersVM();
            vm.Tab = tab;
            vm.ChangeRequestId = changeRequestId;
            vm.AdditionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .Where(m => m.ChangeRequestId == changeRequestId)
                .ToListAsync();

            // get list of available equipment reviewers...


            // get list of available maintenance reviewers...


            return View(vm);
        }

        //public async Task<IActionResult> Index(int changeRequestId, string tab)
        //{
        //    AdditionalImpactAssessmentReviewersVM vm = new AdditionalImpactAssessmentReviewersVM();
        //    vm.Tab = tab;
        //    vm.ChangeRequestId = changeRequestId;
        //    vm.AdditionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
        //        .Where(m => m.ChangeRequestId == changeRequestId)
        //        .ToListAsync();

        //    return View(vm);
        //}

        // GET: AdditionalImpactAssessmentReviewers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
                return NotFound();


            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (additionalImpactAssessmentReviewers == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Create
        public async Task<IActionResult> Create(int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Users = getUserList();
            ViewBag.ReviewTypes = (await _context.ReviewType.Select(m => m.Type).Distinct().ToListAsync()).OrderBy(x => x).ToList();

            AdditionalImpactAssessmentReviewers aiar = new AdditionalImpactAssessmentReviewers();
            aiar.ChangeRequestId = changeRequestId;
            ViewBag.Tab = tab;
            aiar.CreatedUser = _username;
            aiar.CreatedDate = DateTime.UtcNow;
            return View(aiar);
        }

        // POST: AdditionalImpactAssessmentReviewers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,Reviewer,ReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] AdditionalImpactAssessmentReviewers additionalImpactAssessmentReviewers, int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            // Make sure duplicates are not entered...
            List<AdditionalImpactAssessmentReviewers> checkDupes = await _context.AdditionalImpactAssessmentReviewers
                .Where(m => m.Reviewer == additionalImpactAssessmentReviewers.Reviewer)
                .Where(m => m.ReviewType == additionalImpactAssessmentReviewers.ReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Type).Select(m => m.Type).ToListAsync();
                ViewBag.Users = getUserList();
                ViewBag.IsAdmin = _isAdmin;
                ViewBag.Username = _username;
                ViewBag.Tab = tab;
                additionalImpactAssessmentReviewers.ChangeRequestId = changeRequestId;
                ModelState.AddModelError("Reviewer", "Reviewer and Review Type combination already exist.");
                ModelState.AddModelError("ReviewType", "Reviewer and Review Type combination already exist.");
                return View(additionalImpactAssessmentReviewers);
            }

            if (ModelState.IsValid)
            {
                _context.Add(additionalImpactAssessmentReviewers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { changeRequestId = changeRequestId, tab = tab });
            }
            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Type).Select(m => m.Type).ToListAsync();
            ViewBag.Users = getUserList();
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Tab = tab;
            additionalImpactAssessmentReviewers.ChangeRequestId = changeRequestId;
            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Edit/5
        public async Task<IActionResult> Edit(int? id, int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
                return NotFound();

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers.FindAsync(id);
            if (additionalImpactAssessmentReviewers == null)
                return NotFound();

            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Type).Select(m => m.Type).ToListAsync();
            ViewBag.Users = getUserList();
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Tab = tab;
            additionalImpactAssessmentReviewers.ChangeRequestId = changeRequestId;
            return View(additionalImpactAssessmentReviewers);
        }

        // POST: AdditionalImpactAssessmentReviewers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeRequestId,Reviewer,ReviewType,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] AdditionalImpactAssessmentReviewers additionalImpactAssessmentReviewers, int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id != additionalImpactAssessmentReviewers.Id)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<AdditionalImpactAssessmentReviewers> checkDupes = await _context.AdditionalImpactAssessmentReviewers
                .Where(m => m.Reviewer == additionalImpactAssessmentReviewers.Reviewer)
                .Where(m => m.ReviewType == additionalImpactAssessmentReviewers.ReviewType)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Type).Select(m => m.Type).ToListAsync();
                ViewBag.Users = getUserList();
                ViewBag.Tab = tab;
                additionalImpactAssessmentReviewers.ChangeRequestId = changeRequestId;
                ModelState.AddModelError("Reviewer", "Reviewer and Review Type combination already exist.");
                ModelState.AddModelError("ReviewType", "Reviewer and Review Type combination already exist.");
                return View(additionalImpactAssessmentReviewers);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    additionalImpactAssessmentReviewers.ModifiedDate = DateTime.UtcNow;
                    additionalImpactAssessmentReviewers.ModifiedUser = _username;
                    _context.Update(additionalImpactAssessmentReviewers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdditionalImpactAssessmentReviewersExists(additionalImpactAssessmentReviewers.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index), new { changeRequestId = changeRequestId, tab = tab });
            }
            ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Type).Select(m => m.Type).ToListAsync();
            ViewBag.Users = getUserList();
            ViewBag.Tab = tab;
            additionalImpactAssessmentReviewers.ChangeRequestId = changeRequestId;
            return View(additionalImpactAssessmentReviewers);
        }

        // GET: AdditionalImpactAssessmentReviewers/Delete/5
        public async Task<IActionResult> Delete(int? id, int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.ChangeRequestId = changeRequestId;
            ViewBag.Tab = tab;

            if (id == null || _context.AdditionalImpactAssessmentReviewers == null)
                return NotFound();

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (additionalImpactAssessmentReviewers == null)
                return NotFound();

            return View(additionalImpactAssessmentReviewers);
        }

        // POST: AdditionalImpactAssessmentReviewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int changeRequestId, string tab)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.AdditionalImpactAssessmentReviewers == null)
                return Problem("Entity set 'Management_of_ChangeContext.AdditionalImpactAssessmentReviewers'  is null.");

            var additionalImpactAssessmentReviewers = await _context.AdditionalImpactAssessmentReviewers.FindAsync(id);
            if (additionalImpactAssessmentReviewers != null)
                _context.AdditionalImpactAssessmentReviewers.Remove(additionalImpactAssessmentReviewers);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { changeRequestId = changeRequestId, tab = tab });
        }

        private bool AdditionalImpactAssessmentReviewersExists(int id)
        {
            return (_context.AdditionalImpactAssessmentReviewers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
