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
        public async Task<IActionResult> Index(string statusFilter)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var requests = from m in _context.Task
                           select m;

            if (statusFilter != null)
                requests = requests.Where(r => r.Status == statusFilter);

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
        public async Task<IActionResult> Create()
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
                users.Add(item);
            }
            ViewBag.Users = users;

            return View(task);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,ImplementationType,MocNumber,Status,AssignedToUser,AssignedByUser,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (ModelState.IsValid)
            {
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Task == null)
                return NotFound();

            var task = await _context.Task.FindAsync(id);

            if (task == null)
                return NotFound();

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

                if (user.onpremisessamaccountname == task.AssignedToUser)
                    item.Selected = true;
                users.Add(item);
            }
            ViewBag.Users = users;

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

            task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
            task.ModifiedUser = _username;
            task.ModifiedDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
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
    }
}
