using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ChangeRequestsController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ChangeRequestsController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ChangeRequests
        public async Task<IActionResult> Index()
        {
            return _context.ChangeRequest != null ?
                        View(await _context.ChangeRequest.OrderBy(m => m.Id).ToListAsync()) :
                        Problem("Entity set 'Management_of_ChangeContext.ChangeRequest'  is null.");
        }

        // GET: ChangeRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeRequest == null)
                return NotFound();

            // Get all the General MOC Responses associated with this request...
            changeRequest.GeneralMocResponses = await _context.GeneralMocResponses
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.Order)
                .ToListAsync();

            // Get all the Impact Assessment Responses associated with this request...
            changeRequest.ImpactAssessmentResponses = await _context.ImpactAssessmentResponse
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.ReviewType)
                .ThenBy(m => m.ChangeType)
                .ToListAsync();

            // Get all the Impact Assessment Responses Questions/Answers associated with this request...
            if (changeRequest.ImpactAssessmentResponses.Any())
            {
                foreach (var record in changeRequest.ImpactAssessmentResponses)
                {
                    record.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
                    .Where(m => m.ImpactAssessmentResponseId == record.Id)
                    .OrderBy(m => m.ReviewType)
                    .ThenBy(m => m.Order)
                    .ToListAsync();
                }
            }
            //// Get all the Impact Assessment Responses Questions/Answers associated with this request...
            //changeRequest.ImpactAssessmentResponses.ImpactAssessmentResponseAnswers = await _context.ImpactAssessmentResponseAnswer
            //    .Where(m => m.ImpactAssessmentResponseId == changeRequest.ImpactAssessmentResponses.Id)
            //    .OrderBy(m => m.ReviewType)
            //    .ThenBy(m => m.Order)
            //    .ToListAsync();

            // Get all the Final Approval Responses associated with this request...
            changeRequest.ImplementationFinalApprovalResponses = await _context.ImplementationFinalApprovalResponse
                .Where(m => m.ChangeRequestId == id)
                .OrderBy(m => m.FinalReviewType)
                .ThenBy(m => m.ChangeType)
                .ToListAsync();

            return View(changeRequest);
        }

        // GET: ChangeRequests/Create
        public async Task<IActionResult> Create()
        {
            ChangeRequest changeRequest = new ChangeRequest
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            // Persist Dropdown Selection Lists
            ViewBag.Levels = await _context.ChangeLevel.OrderBy(m => m.Order).Select(m => m.Level).ToListAsync();
            ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            if (ModelState.IsValid)
            {
                // add General MOC Questions
                List<GeneralMocQuestions> questions = await _context.GeneralMocQuestions.OrderBy(m => m.Order).ToListAsync();
                if (questions.Count > 0)
                {
                    changeRequest.GeneralMocResponses = new List<GeneralMocResponses>();
                    foreach (var question in questions)
                    {
                        GeneralMocResponses response = new GeneralMocResponses
                        {
                            Question = question.Question,
                            Order = question.Order,
                            CreatedUser = "Michael Wilson",
                            CreatedDate = DateTime.Now
                        };
                        changeRequest.GeneralMocResponses.Add(response);
                    }
                }

                // add Impact Assessment Responses
                List<ImpactAssessmentMatrix> impactAssessmentMatrix = await _context.ImpactAssessmentMatrix
                    .Where(m => m.ChangeType == changeRequest.Change_Type)
                    .OrderBy(m => m.ReviewType)
                    .ThenBy(m => m.ChangeType)
                    .ToListAsync();
                if (impactAssessmentMatrix.Count > 0)
                {
                    changeRequest.ImpactAssessmentResponses = new List<ImpactAssessmentResponse>();
                    foreach (var assessment in impactAssessmentMatrix)
                    {
                        ReviewType review = _context.ReviewType.Where(m => m.Type == assessment.ReviewType).FirstOrDefault();
                        if (review != null)
                        {
                            ImpactAssessmentResponse response = new ImpactAssessmentResponse
                            {
                                ReviewType = assessment.ReviewType,
                                ChangeType = assessment.ChangeType,
                                Reviewer = review.Reviewer,
                                ReviewerEmail = review.Email,
                                Required = true,
                                CreatedUser = "Michael Wilson",
                                CreatedDate = DateTime.Now
                            };
                            changeRequest.ImpactAssessmentResponses.Add(response);
                        }
                    }
                }

                // add Impact Assessment Response Quesion/Answers
                if (changeRequest.ImpactAssessmentResponses != null && changeRequest.ImpactAssessmentResponses.Count > 0)
                {
                    foreach (var record in changeRequest.ImpactAssessmentResponses)
                    {
                        record.ImpactAssessmentResponseAnswers = new List<ImpactAssessmentResponseAnswer>();

                        List<ImpactAssessmentResponseQuestions> IARQuestions = await _context.ImpactAssessmentResponseQuestions.Where(m => m.ReviewType == record.ReviewType).ToListAsync();

                        if (IARQuestions != null && IARQuestions.Count > 0)
                        {
                            foreach (var question in IARQuestions)
                            {
                                ImpactAssessmentResponseAnswer rec = new ImpactAssessmentResponseAnswer
                                {
                                    ReviewType = record.ReviewType,
                                    Question = question.Question,
                                    Order = question.Order,
                                    CreatedUser = "Michael Wilson",
                                    CreatedDate = DateTime.Now
                                };
                                record.ImpactAssessmentResponseAnswers.Add(rec);  //NEED TO INSTANTIATE HERE!!!
                            }
                        }
                    }
                }

                // add Implementation Final Approval Responses
                List<ImplementationFinalApprovalMatrix> implementationFinalApprovalMatrix = await _context.ImplementationFinalApprovalMatrix
                    .Where(m => m.ChangeType == changeRequest.Change_Type)
                    .OrderBy(m => m.FinalReviewType)
                    .ThenBy(m => m.ChangeType)
                    .ToListAsync();
                if (implementationFinalApprovalMatrix.Count > 0)
                {
                    changeRequest.ImplementationFinalApprovalResponses = new List<ImplementationFinalApprovalResponse>();
                    foreach (var assessment in implementationFinalApprovalMatrix)
                    {
                        FinalReviewType review = _context.FinalReviewType.Where(m => m.Type == assessment.FinalReviewType).FirstOrDefault();
                        if (review != null)
                        {
                            ImplementationFinalApprovalResponse response = new ImplementationFinalApprovalResponse
                            {
                                FinalReviewType = assessment.FinalReviewType,
                                ChangeType = assessment.ChangeType,
                                Reviewer = review.Reviewer,
                                ReviewerEmail = review.Email,
                                CreatedUser = "Michael Wilson",
                                CreatedDate = DateTime.Now
                            };
                            changeRequest.ImplementationFinalApprovalResponses.Add(response);
                        }
                    }
                }

                changeRequest.MOC_Number = "MOC-";
                changeRequest.Request_Date = DateTime.Now.Date;
                _context.Add(changeRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeRequest);
        }

        // GET: ChangeRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest.FindAsync(id);

            if (changeRequest == null)
                return NotFound();

            // Persist Dropdown Selection Lists
            ViewBag.Levels = await _context.ChangeLevel.OrderBy(m => m.Order).Select(m => m.Level).ToListAsync();
            ViewBag.Status = await _context.ChangeStatus.OrderBy(m => m.Order).Select(m => m.Status).ToListAsync();
            ViewBag.Types = await _context.ChangeType.OrderBy(m => m.Order).Select(m => m.Type).ToListAsync();
            ViewBag.Responses = await _context.ResponseDropdownSelections.OrderBy(m => m.Order).Select(m => m.Response).ToListAsync();
            ViewBag.ProductLines = await _context.ProductLine.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.SiteLocations = await _context.SiteLocation.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MOC_Number,Change_Owner,Location_Site,Title_Change_Description,Scope_of_the_Change,Justification_of_the_Change,Change_Status,Request_Date,Proudct_Line,Change_Type,Estimated_Completion_Date,Raw_Material_Component_Numbers_Impacted,Change_Level,Area_of_Change,Expiration_Date_Temporary,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeRequest changeRequest)
        {
            if (id != changeRequest.Id)
                return NotFound();

            changeRequest.ModifiedUser = "Michael Wilson";
            changeRequest.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeRequestExists(changeRequest.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeRequest);
        }

        // GET: ChangeRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChangeRequest == null)
                return NotFound();

            var changeRequest = await _context.ChangeRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (changeRequest == null)
                return NotFound();

            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChangeRequest == null)
            {
                return Problem("Entity set 'Management_of_ChangeContext.ChangeRequest'  is null.");
            }
            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest != null)
            {
                _context.ChangeRequest.Remove(changeRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequest?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
