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

            if (ModelState.IsValid)
            {
                implementationFinalApprovalResponse.ModifiedUser = _username;
                implementationFinalApprovalResponse.ModifiedDate = DateTime.UtcNow;

                if (implementationFinalApprovalResponse.ReviewCompleted == null || implementationFinalApprovalResponse.ReviewCompleted == false)
                    implementationFinalApprovalResponse.DateCompleted = null;
                else
                    implementationFinalApprovalResponse.DateCompleted = DateTime.UtcNow;

                try
                {
                    _context.Update(implementationFinalApprovalResponse);
                    await _context.SaveChangesAsync();

                    // check to see if all final reviews are complete for this change request.  If so, advanced to next stage/status...
                    bool found = await _context.ImplementationFinalApprovalResponse
                        .Where(m => m.ChangeRequestId == implementationFinalApprovalResponse.ChangeRequestId)
                        .Where(m => m.ReviewCompleted == null || m.ReviewCompleted == false)
                        .AnyAsync();

                    if (!found)
                    {
                        // There are no incomplete final approvals.  Advance to next stage.
                        var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == implementationFinalApprovalResponse.ChangeRequestId);
                        if (changeRequest != null)
                        {
                            changeRequest.Change_Status = "Submitted for Implementation";
                            changeRequest.ModifiedDate = DateTime.UtcNow;
                            changeRequest.ModifiedUser = _username;
                            _context.Update(changeRequest);
                            await _context.SaveChangesAsync();

                            // Email all admins with 'Approver' rights that this Change Request has been submitted for Implementation....
                            // TODO MJWII
                            var adminApproverList = await _context.Administrators.Where(m => m.Approver == true).ToListAsync();
                            foreach(var record in adminApproverList)
                            {
                                var admin = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == record.Username).FirstOrDefaultAsync();
                                string subject = @"Management of Change (MoC) - Close-Out/Complete Needed";
                                string body = @"Your Close-Out/Complete of an MoC Change Request is needed.  Please follow link below and review/respond to the following Management of Change request. <br/><br/><strong>Change Request: </strong>" + changeRequest.MOC_Number + @"<br/><strong>MoC Title: </strong>" + changeRequest.Title_Change_Description + @"<br/><strong>Link: http://appdevbaub01/</strong><br/><br/>";
                                Initialization.EmailProviderSmtp.SendMessage(subject, body, admin.mail, null, null);

                                EmailHistory emailHistory = new EmailHistory
                                {
                                    Subject = subject,
                                    Body = body,
                                    SentToDisplayName = admin.displayname,
                                    SentToUsername = record.Username,
                                    SentToEmail = admin.mail,
                                    ChangeRequestId = changeRequest.Id,
                                    ImplementationFinalApprovalResponseId = implementationFinalApprovalResponse.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    CreatedUser = _username
                                };
                                _context.Add(emailHistory);
                                await _context.SaveChangesAsync();
                            }
                        }                            
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImplementationFinalApprovalResponseExists(implementationFinalApprovalResponse.Id))
                        return NotFound();
                    else
                        throw;
                }
                //if (tab == "IARDetails")
                //    return RedirectToAction("Details", "ImplementationFinalApprovalResponses", new { Id = implementationFinalApprovalResponse.Id, tab = tab });
                //else
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
