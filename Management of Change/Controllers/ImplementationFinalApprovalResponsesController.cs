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
    public class ImplementationFinalApprovalResponsesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ImplementationFinalApprovalResponsesController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ImplementationFinalApprovalResponses
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ImplementationFinalApprovalResponse != null ?
                          View(await _context.ImplementationFinalApprovalResponse.OrderBy(m => m.FinalReviewType).ThenBy(m => m.ChangeType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalResponse'  is null.");
        }

        // GET: ImplementationFinalApprovalResponses/Details/5
        public async Task<IActionResult> Details(int? id, string tab = "FinalApprovals")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.Tab = tab;
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalResponse == null)
                return NotFound();

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (implementationFinalApprovalResponse == null)
                return NotFound();
            //if (tab == "FinalApprovals")
            //    return RedirectToAction("Details", "ChangeRequests", new { Id = implementationFinalApprovalResponse.ChangeRequestId, tab = "FinalApprovals" });
            //else
            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ImplementationFinalApprovalResponse implementationFinalApprovalResponse = new ImplementationFinalApprovalResponse
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeType,FinalReviewType,Reviewer,ReviewerEmail,Username,ReviewResult,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalResponse implementationFinalApprovalResponse)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                _context.Add(implementationFinalApprovalResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Edit/5
        public async Task<IActionResult> Edit(int? id, string tab = "FinalApprovals")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalResponse == null)
                return NotFound();

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FindAsync(id);

            if (implementationFinalApprovalResponse == null)
                return NotFound();

            ViewBag.Tab = tab;
            ViewBag.Users = getUserList(implementationFinalApprovalResponse.Username);

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeType,FinalReviewType,Username,Reviewer,ReviewerEmail,ReviewResult,ReviewCompleted,DateCompleted,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImplementationFinalApprovalResponse implementationFinalApprovalResponse/*, string tab = "FinalApprovals"*/)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != implementationFinalApprovalResponse.Id)
                return NotFound();

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname == implementationFinalApprovalResponse.Username);
            if (employee != null)
            {
                implementationFinalApprovalResponse.Reviewer = employee.displayname;
                implementationFinalApprovalResponse.ReviewerEmail = employee.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + implementationFinalApprovalResponse.Username);

            if (String.IsNullOrWhiteSpace(implementationFinalApprovalResponse.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(implementationFinalApprovalResponse.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(implementationFinalApprovalResponse.ReviewerEmail))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (String.IsNullOrWhiteSpace(implementationFinalApprovalResponse.ReviewResult))
                ModelState.AddModelError("ReviewResult", "Review Result Required");

            if (ModelState.IsValid)
            {
                implementationFinalApprovalResponse.ModifiedUser = _username;
                implementationFinalApprovalResponse.ModifiedDate = DateTime.UtcNow;
                _context.Update(implementationFinalApprovalResponse);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "ChangeRequests", new { Id = implementationFinalApprovalResponse.ChangeRequestId, tab = "FinalApprovals" });
            }
            ViewBag.Users = getUserList(implementationFinalApprovalResponse.Username);
            return View(implementationFinalApprovalResponse);
        }

        // GET: ImplementationFinalApprovalResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImplementationFinalApprovalResponse == null)
                return NotFound();

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse
                .FirstOrDefaultAsync(m => m.Id == id);

            if (implementationFinalApprovalResponse == null)
                return NotFound();

            return View(implementationFinalApprovalResponse);
        }

        // POST: ImplementationFinalApprovalResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ImplementationFinalApprovalResponse == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImplementationFinalApprovalResponse'  is null.");

            var implementationFinalApprovalResponse = await _context.ImplementationFinalApprovalResponse.FindAsync(id);

            if (implementationFinalApprovalResponse != null)
                _context.ImplementationFinalApprovalResponse.Remove(implementationFinalApprovalResponse);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImplementationFinalApprovalResponseExists(int id)
        {
            return (_context.ImplementationFinalApprovalResponse?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
