using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class TasksController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public TasksController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string taskStatusFilter)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var requests = from m in _context.Task
                           select m;

            if (taskStatusFilter != null)
                requests = requests.Where(r => r.Status == taskStatusFilter);

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View("Index", await requests.OrderBy(m => m.DueDate).ThenBy(m => m.CreatedDate).ToListAsync());

            //return _context.Task != null ?
            //            View(await _context.Task.OrderBy(m => m.DueDate).ThenBy(m => m.CreatedDate).ToListAsync()) :
            //            Problem("Entity set 'Management_of_ChangeContext.Task'  is null.");
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.Task == null)
                return NotFound();

            var task = await _context.Task.FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
                return NotFound();

            ViewBag.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(s => s.MOC_Number).FirstOrDefaultAsync();
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(task);
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create(string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            Models.Task task = new Models.Task
            {
                AssignedByUser = _username,
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            // Create Dropdown List of ChangeRequests...
            var requestList = await _context.ChangeRequest.Where(m => m.DeletedDate == null).OrderBy(m => m.MOC_Number).ThenBy(m => m.CreatedDate).ToListAsync();
            List<SelectListItem> requests = new List<SelectListItem>();
            foreach (var request in requestList)
            {
                SelectListItem item = new SelectListItem { Value = request.Id.ToString(), Text = request.MOC_Number + " : " + request.Title_Change_Description };
                requests.Add(item);
            }
            ViewBag.ChangeRequests = requests;
            ViewBag.Users = getUserList();
            ViewBag.Source = source;

            return View(task);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,ImplementationType,MocNumber,Status,AssignedToUser,AssignedByUser,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task, string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (task.DueDate == null)
                ModelState.AddModelError("DueDate", "Must Include a Completion Date");

            if (task.DueDate < DateTime.Today)
                ModelState.AddModelError("DueDate", "Date Cannot Be In The Past");

            if (task.Status == "On Hold" && String.IsNullOrWhiteSpace(task.OnHoldReason))
                ModelState.AddModelError("OnHoldReason", "If Task Status is 'On Hold', Reason is required.");

            if (task.ChangeRequestId != null)
            {
                // cannot create a Task for a Change Request after it is at a certain status....
                ChangeRequest changeRequest = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).FirstOrDefaultAsync();

                if (changeRequest != null)
                {
                    if ((task.ImplementationType == "Pre") &&
                        (changeRequest.Change_Status == "Closeout" || changeRequest.Change_Status == "Closed"))
                        ModelState.AddModelError("ChangeRequestId", "Task Status is beyond the stage to add a Pre Implementation Task.");
                    if ((task.ImplementationType == "Post") &&
                        (changeRequest.Change_Status == "Closed"))
                        ModelState.AddModelError("ChangeRequestId", "Task Status is beyond the stage to add a Post Implementation Task.");
                    if (changeRequest.Change_Status == "Cancelled")
                        ModelState.AddModelError("ChangeRequestId", "Cannot Create a Task under a Cancelled Change Request.");
                }
            }

            if (ModelState.IsValid)
            {
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
                _context.Add(task);
                await _context.SaveChangesAsync();

                // Send Email Out notifying the person who is assigned the task
                string subject = @"Management of Change (MoC) - Impact Assessment Response Task Assigned.";
                string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>MoC Title: </strong>" + task.Title + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
                var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == task.AssignedToUser).FirstOrDefaultAsync();
                if (toPerson != null)
                {
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson.mail, null, null);

                    EmailHistory emailHistory = new EmailHistory
                    {
                        Subject = subject,
                        Body = body,
                        SentToDisplayName = toPerson.displayname,
                        SentToUsername = toPerson.onpremisessamaccountname,
                        SentToEmail = toPerson.mail,
                        ChangeRequestId = task.ChangeRequestId,
                        TaskId = task.Id,
                        Type = "Task",
                        Status = task.Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUser = _username
                    };
                    _context.Add(emailHistory);
                    await _context.SaveChangesAsync();
                }                

                if (source == "Home")
                    return RedirectToAction("Index", "Home", new {});
                else
                    return RedirectToAction(nameof(Index));
            }

            // Create Dropdown List of ChangeRequests...
            var requestList = await _context.ChangeRequest.Where(m => m.DeletedDate == null).OrderBy(m => m.MOC_Number).ThenBy(m => m.CreatedDate).ToListAsync();
            List<SelectListItem> requests = new List<SelectListItem>();
            foreach (var request in requestList)
            {
                SelectListItem item = new SelectListItem { Value = request.Id.ToString(), Text = request.MOC_Number + " : " + request.Title_Change_Description };
                requests.Add(item);
            }

            ViewBag.ChangeRequests = requests;
            ViewBag.Users = getUserList();
            ViewBag.Source = source;
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.Task == null)
                return NotFound();

            var task = await _context.Task.FindAsync(id);

            if (task == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Create Dropdown List of Change Requests...
            var requestList = await _context.ChangeRequest.Where(m => m.DeletedDate == null).OrderBy(m => m.MOC_Number).ThenBy(m => m.CreatedDate).ToListAsync();
            List<SelectListItem> requests = new List<SelectListItem>();
            foreach (var request in requestList)
            {
                SelectListItem item = new SelectListItem { Value = request.Id.ToString(), Text = request.MOC_Number + " : " + request.Title_Change_Description };
                if (request.Id == task.ChangeRequestId)
                    item.Selected = true;
                requests.Add(item);
            }
            ViewBag.ChangeRequests = requests;
            ViewBag.Users = getUserList(task.AssignedToUser);

            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeRequestId,MocNumber,ImplementationType,Status,AssignedToUser,AssignedByUser,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id != task.Id)
                return NotFound();

            if (task.Status == "On Hold" && String.IsNullOrWhiteSpace(task.OnHoldReason))
                ModelState.AddModelError("OnHoldReason", "If Task Status is 'On Hold', Reason is required.");

            if (task.Status == "Complete" && task.CompletionDate == null)
                ModelState.AddModelError("CompletionDate", "If Task Status is 'Complete', Completion Date is required.");

            if (task.Status == "Complete" && String.IsNullOrWhiteSpace(task.CompletionNotes))
                ModelState.AddModelError("CompletionNotes", "If Task Status is 'Complete', Completion Notes is required.");


            if (ModelState.IsValid)
            {
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
                task.ModifiedUser = _username;
                task.ModifiedDate = DateTime.UtcNow;
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Create Dropdown List of Change Requests...
            var requestList = await _context.ChangeRequest.Where(m => m.DeletedDate == null).OrderBy(m => m.MOC_Number).ThenBy(m => m.CreatedDate).ToListAsync();
            List<SelectListItem> requests = new List<SelectListItem>();
            foreach (var request in requestList)
            {
                SelectListItem item = new SelectListItem { Value = request.Id.ToString(), Text = request.MOC_Number + " : " + request.Title_Change_Description };
                if (request.Id == task.ChangeRequestId)
                    item.Selected = true;
                requests.Add(item);
            }
            ViewBag.ChangeRequests = requests;
            ViewBag.Users = getUserList(task.AssignedToUser);

            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _context.Task == null)
                return NotFound();

            var task = await _context.Task.FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (_context.Task == null)
                return Problem("Entity set 'Management_of_ChangeContext.Task'  is null.");

            var task = await _context.Task.FindAsync(id);

            if (task != null)
                _context.Task.Remove(task);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return (_context.Task?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> TaskReminder(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (_context.Task == null)
                return Problem("Entity set 'Management_of_ChangeContext.Task'  is null.");

            var task = await _context.Task.FindAsync(id);
            if (task == null)
                return RedirectToAction("Index");

            // Send Email Out notifying the person who is assigned the task
            string subject = @"Management of Change (MoC) - Impact Assessment Response Task Reminder.";
            string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>MoC Title: </strong>" + task.Title + @"<br/><strong>Link: " + Initialization.WebsiteUrl + @" </strong><br/><br/>";
            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname == task.AssignedToUser).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson.mail, null, null);

                EmailHistory emailHistory = new EmailHistory
                {
                    Subject = subject,
                    Body = body,
                    SentToDisplayName = toPerson.displayname,
                    SentToUsername = toPerson.onpremisessamaccountname,
                    SentToEmail = toPerson.mail,
                    ChangeRequestId = task.ChangeRequestId,
                    TaskId = task.Id,
                    Type = "Task",
                    Status = task.Status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = _username
                };
                _context.Add(emailHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
        }
    }
}
