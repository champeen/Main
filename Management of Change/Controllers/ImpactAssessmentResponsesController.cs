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
    public class ImpactAssessmentResponsesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public ImpactAssessmentResponsesController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: ImpactAssessmentResponses
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ImpactAssessmentResponse != null ?
                          View(await _context.ImpactAssessmentResponse.OrderBy(m => m.ReviewType).ThenBy(m => m.ChangeType).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponse'  is null.");
        }

        // GET: ImpactAssessmentResponses/Details/5
        public async Task<IActionResult> Details(int? id, string? rec = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ImpactAssessmentResponseViewModel impactAssessmentResponseVM = new ImpactAssessmentResponseViewModel();

            // Get Impact Assessment Response...
            impactAssessmentResponseVM.ImpactAssessmentResponse = await _context.ImpactAssessmentResponse.FirstOrDefaultAsync(m => m.Id == id);
            if (impactAssessmentResponseVM.ImpactAssessmentResponse == null)
                return NotFound();

            // Get the Change Request the Impact Assessment Response belongs to...
            impactAssessmentResponseVM.ChangeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == impactAssessmentResponseVM.ImpactAssessmentResponse.ChangeRequestId);

            // Get all the ImpactAssessmentResponsesQuestions/Answers associated with this request...
            impactAssessmentResponseVM.ImpactAssessmentResponse.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
                    .Where(m => m.ImpactAssessmentResponseId == impactAssessmentResponseVM.ImpactAssessmentResponse.Id)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

            // Get all tasks associated with each ImpactAssessmentResponseAnswer
            foreach (var record in impactAssessmentResponseVM.ImpactAssessmentResponse.ImpactAssessmentResponseAnswers)
            {
                Models.Task task = await _context.Task.FirstOrDefaultAsync(m => m.ImpactAssessmentResponseAnswerId == record.Id);
                record.Task = task;
            }

            impactAssessmentResponseVM.IARrecord = rec;

            return View(impactAssessmentResponseVM);
        }

        // GET: ImpactAssessmentResponses/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ImpactAssessmentResponse impactAssessmentResponse = new ImpactAssessmentResponse
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            //ViewBag.ChangeTypes = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            //ViewBag.ReviewTypes = await _context.ReviewType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,ChangeType,ChangeArea,Reviewer,ReviewerEmail,Username,ReviewCompleted,DateCompleted,QuestionsAnswered,Comments,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponse impactAssessmentResponse)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponse);
        }

        // GET: ImpactAssessmentResponses/Edit/5
        public async Task<IActionResult> Edit(int? id, string tab = "ImpactAssessments")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse.FindAsync(id);

            if (impactAssessmentResponse == null)
                return NotFound();

            ViewBag.Tab = tab;
            ViewBag.Users = getUserList(impactAssessmentResponse.Username);

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,ChangeType,ChangeArea,Username,ReviewCompleted,DateCompleted,Comments,QuestionsAnswered,ChangeRequestId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponse impactAssessmentResponse, string tab = "ImpactAssessments")
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != impactAssessmentResponse.Id)
                return NotFound();

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname.ToLower() == impactAssessmentResponse.Username.ToLower());
            if (employee != null)
            {
                impactAssessmentResponse.Reviewer = employee?.displayname;
                impactAssessmentResponse.ReviewerEmail = employee?.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + impactAssessmentResponse.Username);

            if (String.IsNullOrWhiteSpace(impactAssessmentResponse.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(impactAssessmentResponse.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(impactAssessmentResponse.ReviewerEmail))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (ModelState.IsValid)
            {
                impactAssessmentResponse.ModifiedUser = _username;
                impactAssessmentResponse.ModifiedDate = DateTime.Now;
                try
                {
                    _context.Update(impactAssessmentResponse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentResponseExists(impactAssessmentResponse.Id))
                        return NotFound();
                    else
                        throw;
                }
                if (tab == "IARDetails")
                    return RedirectToAction("Details", "ImpactAssessmentResponses", new { Id = impactAssessmentResponse.Id, tab = tab });
                else
                    return RedirectToAction("Details", "ChangeRequests", new { Id = impactAssessmentResponse.ChangeRequestId, tab = tab });
            }
            ViewBag.Users = getUserList(impactAssessmentResponse.Username);
            return View(impactAssessmentResponse);
        }

        // GET: ImpactAssessmentResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ImpactAssessmentResponse == null)
                return NotFound();

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponse == null)
                return NotFound();

            return View(impactAssessmentResponse);
        }

        // POST: ImpactAssessmentResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ImpactAssessmentResponse == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponse'  is null.");

            var impactAssessmentResponse = await _context.ImpactAssessmentResponse.FindAsync(id);

            if (impactAssessmentResponse != null)
                _context.ImpactAssessmentResponse.Remove(impactAssessmentResponse);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentResponseExists(int id)
        {
            return (_context.ImpactAssessmentResponse?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> MarkNoAction(int id, string? rec = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer.FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseAnswer != null)
            {
                impactAssessmentResponseAnswer.Action = "No";
                impactAssessmentResponseAnswer.ModifiedDate = DateTime.Now;
                impactAssessmentResponseAnswer.ModifiedUser = _username;
                _context.Update(impactAssessmentResponseAnswer);
                await _context.SaveChangesAsync();
            }

            // see if there is at least one incomplete impact assessment response review.....
            bool foundIncomplete = await _context.ImpactAssessmentResponseAnswer
                .Where(m => m.ImpactAssessmentResponseId == impactAssessmentResponseAnswer.ImpactAssessmentResponseId)
                .Where(m => m.Action == null)
                .AnyAsync();

            // get ImpactAssessmentResponse ChangeRequest that this Task will belong to
            ImpactAssessmentResponse impactAssessmentResponse = await _context.ImpactAssessmentResponse
                .Where(m => m.Id == impactAssessmentResponseAnswer.ImpactAssessmentResponseId)
                //.Where(m => m.ReviewCompleted != true)
                .FirstOrDefaultAsync();

            if (impactAssessmentResponse != null)
            {
                // found at least 1 incomplete review from reviewer - make sure to mark ImpactAssessmentResponse as not fully answered
                if (foundIncomplete)
                    impactAssessmentResponse.QuestionsAnswered = false;
                // all review questions for reviewers impactAssessment have been answered.  Mark as fully answered
                else
                    impactAssessmentResponse.QuestionsAnswered = true;

                impactAssessmentResponse.ModifiedUser = _username;
                impactAssessmentResponse.ModifiedDate = DateTime.Now;
                _context.Update(impactAssessmentResponse);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { Id = impactAssessmentResponseAnswer.ImpactAssessmentResponseId, rec=rec });
        }
    }
}
