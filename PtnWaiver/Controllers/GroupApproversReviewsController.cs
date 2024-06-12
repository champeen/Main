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
    public class GroupApproversReviewsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public GroupApproversReviewsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: GroupApproversReviews
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.GroupApproversReview != null ? 
                          View(await _contextPtnWaiver.GroupApproversReview.OrderBy(m => m.Order).ThenBy(m => m.Group).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.GroupApproversReview'  is null.");
        }

        // GET: GroupApproversReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.GroupApproversReview == null)
                return NotFound();

            var groupApproversReview = await _contextPtnWaiver.GroupApproversReview
                .FirstOrDefaultAsync(m => m.Id == id);

            if (groupApproversReview == null)
                return NotFound();

            return View(groupApproversReview);
        }

        // GET: GroupApproversReviews/Create
        public IActionResult Create()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View();
        }

        // POST: GroupApproversReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Group,Status,PrimaryApproverUsername,PrimaryApproverFullName,PrimaryApproverEmail,PrimaryApproverTitle,PrimaryApproverDateApproved,SecondaryApproverUsername,SecondaryApproverFullName,SecondaryApproverEmail,SecondaryApproverTitle,SecondaryApproverDateApproved,Comment,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] GroupApproversReview groupApproversReview)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(groupApproversReview);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupApproversReview);
        }

        // GET: GroupApproversReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.GroupApproversReview == null)
                return NotFound();

            var groupApproversReview = await _contextPtnWaiver.GroupApproversReview.FindAsync(id);

            if (groupApproversReview == null)
                return NotFound();

            return View(groupApproversReview);
        }

        // POST: GroupApproversReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Group,Status,PrimaryApproverUsername,PrimaryApproverFullName,PrimaryApproverEmail,PrimaryApproverTitle,PrimaryApproverDateApproved,SecondaryApproverUsername,SecondaryApproverFullName,SecondaryApproverEmail,SecondaryApproverTitle,SecondaryApproverDateApproved,Comment,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] GroupApproversReview groupApproversReview)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != groupApproversReview.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contextPtnWaiver.Update(groupApproversReview);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupApproversReviewExists(groupApproversReview.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(groupApproversReview);
        }

        // GET: GroupApproversReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.GroupApproversReview == null)
                return NotFound();

            var groupApproversReview = await _contextPtnWaiver.GroupApproversReview
                .FirstOrDefaultAsync(m => m.Id == id);

            if (groupApproversReview == null)
                return NotFound();

            return View(groupApproversReview);
        }

        // POST: GroupApproversReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.GroupApproversReview == null)
                return Problem("Entity set 'PtnWaiverContext.GroupApproversReview'  is null.");

            var groupApproversReview = await _contextPtnWaiver.GroupApproversReview.FindAsync(id);

            if (groupApproversReview != null)
                _contextPtnWaiver.GroupApproversReview.Remove(groupApproversReview);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupApproversReviewExists(int id)
        {
          return (_contextPtnWaiver.GroupApproversReview?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
