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
using Microsoft.Identity.Client;

namespace Management_of_Change.Controllers
{
    public class ImpactAssessmentResponseAnswersController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public ImpactAssessmentResponseAnswersController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: ImpactAssessmentResponseAnswers
        public async Task<IActionResult> Index()
        {
              return _context.ImpactAssessmentResponseAnswer != null ? 
                          View(await _context.ImpactAssessmentResponseAnswer.OrderBy(m => m.ImpactAssessmentResponseId).ThenBy(m => m.Order).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseAnswer'  is null.");
        }

        // GET: ImpactAssessmentResponseAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Create
        public async Task<IActionResult> Create()
        {
            ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer = new ImpactAssessmentResponseAnswer
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReviewType,Question,Order,Action,Title,DetailsOfActionNeeded,PreOrPostImplementation,ActionOwner,DateDue,ImpactAssessmentResponseId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(impactAssessmentResponseAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer.FindAsync(id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();

            // see if a task for this ImpactAssessmentResponseAnswers already exists (add it if it doesnt)
            Models.Task existingTask = await _context.Task.FirstOrDefaultAsync(m => m.ImpactAssessmentResponseAnswerId == impactAssessmentResponseAnswer.Id);
            //if (existingTask != null)
            //    ViewBag.ExistingTask = true;
            ViewBag.Task = existingTask;

            // Create Dropdown List of Users...
            var userList = await _context.__mst_employee
                .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                .Where(m => m.accountenabled == true)
                .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                .OrderBy(m => m.displayname)
                .ThenBy(m => m.onpremisessamaccountname)
                .ToListAsync();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (var user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.onpremisessamaccountname, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };
                if (user.onpremisessamaccountname == impactAssessmentResponseAnswer.ActionOwner)
                    item.Selected = true;
                users.Add(item);
            }
            ViewBag.Users = users;

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReviewType,Question,Order,Action,Title,DetailsOfActionNeeded,PreOrPostImplementation,ActionOwner,DateDue,ImpactAssessmentResponseId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ImpactAssessmentResponseAnswer impactAssessmentResponseAnswer)
        {
            if (id != impactAssessmentResponseAnswer.Id)
                return NotFound();

            if (string.IsNullOrWhiteSpace(impactAssessmentResponseAnswer.Action))
                    ModelState.AddModelError("Action", "Action is Required");

            if (impactAssessmentResponseAnswer.Action == "Yes")
            {
                if (string.IsNullOrWhiteSpace(impactAssessmentResponseAnswer.Title))
                    ModelState.AddModelError("Title", "Title is Required if there is an action needed");

                if (string.IsNullOrWhiteSpace(impactAssessmentResponseAnswer.DetailsOfActionNeeded))
                    ModelState.AddModelError("DetailsOfActionNeeded", "Details of Action is Required if there is an action needed");

                if (string.IsNullOrWhiteSpace(impactAssessmentResponseAnswer.PreOrPostImplementation))
                    ModelState.AddModelError("PreOrPostImplementation", "Pre or Post Implementation is Required if there is an action needed");

                if (string.IsNullOrWhiteSpace(impactAssessmentResponseAnswer.ActionOwner))
                    ModelState.AddModelError("ActionOwner", "Action Owner is Required if there is an action needed");

                if (impactAssessmentResponseAnswer.DateDue == null)
                    ModelState.AddModelError("DateDue", "Date Due is Required if there is an action needed");
            }

            if (ModelState.IsValid)
            {
                impactAssessmentResponseAnswer.ModifiedUser = _username;
                impactAssessmentResponseAnswer.ModifiedDate = DateTime.UtcNow;

                // if there is an action to take, create a task for it...
                if (impactAssessmentResponseAnswer.Action == "Yes")
                {
                    // If the task already exists for this Answer, update it.  If not, add it.
                    // TO DO!!!!!

                    // get ImpactAssessmentResponse Change Request that this Task will belong to
                    ImpactAssessmentResponse impactAssessmentResponse = await _context.ImpactAssessmentResponse
                        .Where(m => m.Id == impactAssessmentResponseAnswer.ImpactAssessmentResponseId)
                        .FirstOrDefaultAsync();

                    // get MOC Number this Task will belong to
                    var mocNumber = await _context.ChangeRequest
                        .Where(m => m.Id == impactAssessmentResponse.ChangeRequestId)
                        .Select(m => m.MOC_Number)
                        .FirstOrDefaultAsync();

                    // see if a task for this ImpactAssessmentResponseAnswers already exists (add it if it doesnt)
                    Models.Task existingTask = await _context.Task.FirstOrDefaultAsync(m => m.ImpactAssessmentResponseAnswerId == impactAssessmentResponseAnswer.Id);

                    if (existingTask == null)
                    {
                        Models.Task task = new Models.Task
                        {
                            ChangeRequestId = impactAssessmentResponse.ChangeRequestId,
                            MocNumber = mocNumber,
                            ImplementationType = impactAssessmentResponseAnswer.PreOrPostImplementation,
                            Status = "Open",
                            AssignedToUser = impactAssessmentResponseAnswer.ActionOwner,
                            AssignedByUser = _username,
                            Title = impactAssessmentResponseAnswer.Title,
                            Description = impactAssessmentResponseAnswer.DetailsOfActionNeeded,
                            DueDate = impactAssessmentResponseAnswer.DateDue,
                            ImpactAssessmentResponseAnswerId = impactAssessmentResponseAnswer.Id,
                            CreatedUser = _username,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.Add(task);
                    }                        
                    //else
                    //{
                    //    _context.Update(existingTask);
                    //}                        
                }

                try
                {
                    _context.Update(impactAssessmentResponseAnswer);
                    await _context.SaveChangesAsync();

                    // see if all of this ImpactAssessmentResponses have questions answered from reviewer.  If so, mark as complete...
                    bool found = await _context.ImpactAssessmentResponseAnswer
                        .Where(m => m.ImpactAssessmentResponseId == impactAssessmentResponseAnswer.ImpactAssessmentResponseId)
                        .Where(m => m.Action == null)
                        .AnyAsync();

                    if (!found)
                    {
                        // get ImpactAssessmentResponse Change Request that this Task will belong to
                        ImpactAssessmentResponse impactAssessmentResponse = await _context.ImpactAssessmentResponse
                            .Where(m => m.Id == impactAssessmentResponseAnswer.ImpactAssessmentResponseId)
                            .Where(m => m.ReviewCompleted != true)
                            .FirstOrDefaultAsync();
                        if (impactAssessmentResponse != null)
                        {
                            impactAssessmentResponse.ReviewCompleted = true;
                            impactAssessmentResponse.DateCompleted = DateTime.UtcNow;
                            impactAssessmentResponse.ModifiedUser= _username;
                            impactAssessmentResponse.ModifiedDate= DateTime.UtcNow;
                            _context.Update(impactAssessmentResponse);
                            await _context.SaveChangesAsync();
                        }

                        // see if all of this ChangeRequests ImpactAssessmentResponses are complete.  If so, promote/change status of the ChangeRequest...
                        bool foundIAR = await _context.ImpactAssessmentResponse
                            .Where(m => m.ChangeRequestId == impactAssessmentResponse.ChangeRequestId)
                            .Where(m => m.ReviewCompleted != true)
                            .AnyAsync();

                        if (!foundIAR) 
                        {
                            ChangeRequest changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == impactAssessmentResponse.ChangeRequestId);
                            if (changeRequest != null)
                            {
                                changeRequest.Change_Status = "Submitted for Final Approvals";
                                changeRequest.ModifiedDate= DateTime.UtcNow;
                                changeRequest.ModifiedUser = _username;
                                _context.Update(changeRequest);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpactAssessmentResponseAnswerExists(impactAssessmentResponseAnswer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "ImpactAssessmentResponses", new { Id = impactAssessmentResponseAnswer.ImpactAssessmentResponseId });
            }
            
            // Create Dropdown List of Users...
            var userList = await _context.__mst_employee
                .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                .Where(m => m.accountenabled == true)
                .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                .OrderBy(m => m.displayname)
                .ThenBy(m => m.onpremisessamaccountname)
                .ToListAsync();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (var user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.onpremisessamaccountname, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };
                if (user.onpremisessamaccountname == impactAssessmentResponseAnswer.ActionOwner)
                    item.Selected = true;
                users.Add(item);
            }
            ViewBag.Users = users;
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            return View(impactAssessmentResponseAnswer);
        }

        // GET: ImpactAssessmentResponseAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImpactAssessmentResponseAnswer == null)
                return NotFound();

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer
                .FirstOrDefaultAsync(m => m.Id == id);

            if (impactAssessmentResponseAnswer == null)
                return NotFound();

            return View(impactAssessmentResponseAnswer);
        }

        // POST: ImpactAssessmentResponseAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImpactAssessmentResponseAnswer == null)
                return Problem("Entity set 'Management_of_ChangeContext.ImpactAssessmentResponseAnswer'  is null.");

            var impactAssessmentResponseAnswer = await _context.ImpactAssessmentResponseAnswer.FindAsync(id);

            if (impactAssessmentResponseAnswer != null)
                _context.ImpactAssessmentResponseAnswer.Remove(impactAssessmentResponseAnswer);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImpactAssessmentResponseAnswerExists(int id)
        {
          return (_context.ImpactAssessmentResponseAnswer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
