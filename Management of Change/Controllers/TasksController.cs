using Azure.Core;
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
        public async Task<IActionResult> Index(string taskStatusFilter, string prevTaskStatusFilter = null, string sort = null, string prevSort = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            // if no filter selected, keep previous
            if (taskStatusFilter == null)
                taskStatusFilter = prevTaskStatusFilter;

            // Create Dropdown List of Status...
            List<SelectListItem> statusDropdown = new List<SelectListItem>();
            SelectListItem item = new SelectListItem { Value = "AllCurrent", Text = "All Current (non complete/cancelled)" };
            if (taskStatusFilter == null || taskStatusFilter == "AllCurrent")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "All", Text = "All" };
            if (taskStatusFilter == "All")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "Open", Text = "Open" };
            if (taskStatusFilter == "Open")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "In-Progress", Text = "In-Progress" };
            if (taskStatusFilter == "InProgress")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "On Hold", Text = "On Hold" };
            if (taskStatusFilter == "OnHold")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "Complete", Text = "Complete" };
            if (taskStatusFilter == "Complete")
                item.Selected = true;
            statusDropdown.Add(item);
            item = new SelectListItem { Value = "Cancelled", Text = "Cancelled" };
            if (taskStatusFilter == "Cancelled")
                item.Selected = true;
            statusDropdown.Add(item);
            //foreach (var status in statusList)
            //{
            //    item = new SelectListItem { Value = status.Status, Text = status.Description };
            //    if (item.Value == statusFilter)
            //        item.Selected = true;
            //    else
            //        item.Selected = false;
            //    statusDropdown.Add(item);
            //}
            ViewBag.StatusList = statusDropdown;

            var taskList = await _context.Task.ToListAsync(); // OrderBy(m => m.Priority).ThenBy(m => m.DueDate).ToListAsync();

            switch (taskStatusFilter)
            {
                case null:
                    taskList = taskList.Where(m => m.Status == "Open" || m.Status == "In-Progress" || m.Status == "On Hold").ToList();
                    ViewBag.PrevTaskStatusFilter = "AllCurrent";
                    break;
                case "AllCurrent":
                    taskList = taskList.Where(m => m.Status == "Open" || m.Status == "In-Progress" || m.Status == "On Hold").ToList();
                    ViewBag.PrevTaskStatusFilter = "AllCurrent";
                    break;
                case "All":
                    ViewBag.PrevTaskStatusFilter = "All";
                    break;
                default:
                    taskList = taskList.Where(m => m.Status == taskStatusFilter).ToList();
                    ViewBag.PrevTaskStatusFilter = taskStatusFilter;
                    break;
            }

            // no sort selected, use previous sort...
            if (sort == null)
            {
                sort = prevSort;
                switch (sort)
                {
                    case null:
                        taskList = taskList.OrderBy(m => m.Priority).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = null;
                        break;
                    case "TaskAsc":
                        taskList = taskList.OrderBy(m => m.Id).ToList();
                        ViewBag.PrevSort = "TaskAsc";
                        break;
                    case "TaskDesc":
                        taskList = taskList.OrderByDescending(m => m.Id).ToList();
                        ViewBag.PrevSort = "TaskDesc";
                        break;
                    case "MocAsc":
                        taskList = taskList.OrderBy(m => m.MocNumber).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "MocAsc";
                        break;
                    case "MocDesc":
                        taskList = taskList.OrderByDescending(m => m.MocNumber).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "MocDesc";
                        break;
                    case "ImplementationTypeAsc":
                        taskList = taskList.OrderBy(m => m.ImplementationType).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "ImplementationTypeAsc";
                        break;
                    case "ImplementationTypeDesc":
                        taskList = taskList.OrderByDescending(m => m.ImplementationType).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "ImplementationTypeDesc";
                        break;
                    case "TitleAsc":
                        taskList = taskList.OrderBy(m => m.Title).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "TitleAsc";
                        break;
                    case "TitleDesc":
                        taskList = taskList.OrderByDescending(m => m.Title).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "TitleDesc";
                        break;
                    case "StatusAsc":
                        taskList = taskList.OrderBy(m => m.Status).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "StatusAsc";
                        break;
                    case "StatusDesc":
                        taskList = taskList.OrderByDescending(m => m.Status).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "StatusDesc";
                        break;
                    case "AssignedToAsc":
                        taskList = taskList.OrderBy(m => m.AssignedToUserFullName).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "AssignedToAsc";
                        break;
                    case "AssignedToDesc":
                        taskList = taskList.OrderByDescending(m => m.AssignedToUserFullName).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "AssignedToDesc";
                        break;
                    case "AssignedByAsc":
                        taskList = taskList.OrderBy(m => m.AssignedByUserFullName).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "AssignedByAsc";
                        break;
                    case "AssignedByDesc":
                        taskList = taskList.OrderByDescending(m => m.AssignedByUserFullName).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "AssignedByDesc";
                        break;
                    case "DueDateAsc":
                        taskList = taskList.OrderBy(m => m.DueDate).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "DueDateAsc";
                        break;
                    case "DueDateDesc":
                        taskList = taskList.OrderByDescending(m => m.DueDate).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "DueDateDesc";
                        break;
                    case "CompletionDateAsc":
                        taskList = taskList.OrderBy(m => m.DueDate).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "CompletionDateAsc";
                        break;
                    case "CompletionDateDesc":
                        taskList = taskList.OrderByDescending(m => m.DueDate).ThenBy(m => m.DueDate).ToList();
                        ViewBag.PrevSort = "CompletionDateDesc";
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "Task":
                        if (prevSort != null && prevSort == "TaskAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.Id).ToList();
                            ViewBag.PrevSort = "TaskDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.Id).ToList();
                            ViewBag.PrevSort = "TaskAsc";
                        }
                        break;
                    case "Moc":
                        if (prevSort != null && prevSort == "MocAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.MocNumber).ToList();
                            ViewBag.PrevSort = "MocDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.MocNumber).ToList();
                            ViewBag.PrevSort = "MocAsc";
                        }
                        break;
                    case "ImplementationType":
                        if (prevSort != null && prevSort == "ImplementationTypeAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.ImplementationType).ToList();
                            ViewBag.PrevSort = "ImplementationTypeDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.ImplementationType).ToList();
                            ViewBag.PrevSort = "ImplementationTypeAsc";
                        }
                        break;
                    case "Title":
                        if (prevSort != null && prevSort == "TitleAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.Title).ToList();
                            ViewBag.PrevSort = "TitleDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.Title).ToList();
                            ViewBag.PrevSort = "TitleAsc";
                        }
                        break;
                    case "Status":
                        if (prevSort != null && prevSort == "StatusAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.Status).ToList();
                            ViewBag.PrevSort = "StatusDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.Status).ToList();
                            ViewBag.PrevSort = "StatusAsc";
                        }
                        break;
                    case "AssignedTo":
                        if (prevSort != null && prevSort == "AssignedToAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.AssignedToUserFullName).ToList();
                            ViewBag.PrevSort = "AssignedToDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.AssignedToUserFullName).ToList();
                            ViewBag.PrevSort = "AssignedToAsc";
                        }
                        break;
                    case "AssignedBy":
                        if (prevSort != null && prevSort == "AssignedByAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.AssignedByUserFullName).ToList();
                            ViewBag.PrevSort = "AssignedByDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.AssignedByUserFullName).ToList();
                            ViewBag.PrevSort = "AssignedByAsc";
                        }
                        break;
                    case "DueDate":
                        if (prevSort != null && prevSort == "DueDateAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.DueDate).ToList();
                            ViewBag.PrevSort = "DueDateDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.DueDate).ToList();
                            ViewBag.PrevSort = "DueDateAsc";
                        }
                        break;
                    case "CompletionDate":
                        if (prevSort != null && prevSort == "CompletionDateAsc")
                        {
                            taskList = taskList.OrderByDescending(m => m.CompletionDate).ToList();
                            ViewBag.PrevSort = "CompletionDateDesc";
                        }
                        else
                        {
                            taskList = taskList.OrderBy(m => m.CompletionDate).ToList();
                            ViewBag.PrevSort = "CompletionDateAsc";
                        }
                        break;
                    default:
                        taskList = taskList.OrderBy(m => m.Priority).ThenBy(m => m.CompletionDate).ToList();
                        ViewBag.PrevSort = null;
                        break;
                }
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View("Index", taskList);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id, string fileAttachmentError = null, string destinationPage = null, string previousAction = null, string? tab = "Details")
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

            TaskVM taskVM = new TaskVM();
            taskVM.Task = task;
            taskVM.TabActiveDetail = "";
            taskVM.TabActiveAttachments = "";

            switch (tab)
            {
                case null:
                    taskVM.TabActiveDetail = "active";
                    break;
                case "":
                    taskVM.TabActiveDetail = "active";
                    break;
                case "Details":
                    taskVM.TabActiveDetail = "active";
                    break;
                case "Attachments":
                    taskVM.TabActiveAttachments = "active";
                    break;
                default:
                    taskVM.TabActiveDetail = "active";
                    break;
            }

            ViewBag.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(s => s.MOC_Number).FirstOrDefaultAsync();
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.CreatedUserDisplayName = getUserDisplayName(task.CreatedUser);
            ViewBag.ModifiedUserDisplayName = getUserDisplayName(task.ModifiedUser);
            ViewBag.DeletedUserDisplayName = getUserDisplayName(task.DeletedUser);
            ViewBag.PreviousAction = previousAction;
            ViewBag.FileAttachmentError = fileAttachmentError;
            taskVM.FileAttachmentError = fileAttachmentError;
            ViewBag.DestinationPage = destinationPage;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TASK ATTACHMENTS                                                                                   \\BAY1VPRD-MOC01\Tasks\{task Id}
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.TaskDirectory, task.Id.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.TaskDirectory, task.Id.ToString())))
                path.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<Attachment> attachments = new List<ViewModels.Attachment>();
            foreach (FileInfo i in Files)
            {
                Attachment attachment = new Attachment
                {
                    Directory = i.DirectoryName,
                    Name = i.Name,
                    Extension = i.Extension,
                    FullPath = i.FullName,
                    CreatedDate = i.CreationTimeUtc.Date,
                    Size = Convert.ToInt32(i.Length)
                };
                attachments.Add(attachment);

                //var blah = i.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            taskVM.Attachments = attachments.OrderBy(m => m.Name).ToList();
            ViewBag.MocTitle = await _context.ChangeRequest.Where(m=>m.Id == task.ChangeRequestId).Select(m=>m.Title_Change_Description).FirstOrDefaultAsync();

            return View(taskVM);
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
                CreatedDate = DateTime.Now
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
        public async Task<IActionResult> Create([Bind("Id,ChangeRequestId,ImplementationType,MocNumber,Status,Priority,AssignedToUser,AssignedByUser,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,CancelledReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task, string source = null)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (task.DueDate == null)
                ModelState.AddModelError("DueDate", "Must Include a Valid Due Date");

            if (task.DueDate < DateTime.Today)
                ModelState.AddModelError("DueDate", "Date Cannot Be In The Past");

            if (task.Status == "On Hold" && String.IsNullOrWhiteSpace(task.OnHoldReason))
                ModelState.AddModelError("OnHoldReason", "If Task Status is 'On Hold', Reason is required.");

            if (task.Status == "Cancelled" && String.IsNullOrWhiteSpace(task.CancelledReason))
                ModelState.AddModelError("CancelledReason", "If Task Status is 'Cancelled', Reason is required.");

            if (task.Status == "Complete" && task.CompletionDate == null)
                ModelState.AddModelError("CompletionDate", "If Task Status is 'Completed, Completion Date is required.");

            if (task.Status != "Complete" && task.CompletionDate != null)
            {
                ModelState.AddModelError("CompletionDate", "Completion Date is entered, but Status is not 'Complete'. (Either Change Status to 'Complete' or clear 'Completion Date')");
                ModelState.AddModelError("Status", "Completion Date is entered, but Status is not 'Complete'. (Either Change Status to 'Complete' or clear 'Completion Date')");
            }

            if (task.ChangeRequestId != null && task.ImplementationType == null)
                ModelState.AddModelError("ImplementationType", "If task is part of a Change Request, Implementation Type is required.");

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

                // get assigned-to person info....
                var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedToUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedToUserFullName = toPerson?.displayname;
                task.AssignedToUserEmail = toPerson?.mail;

                // get assigned-by person info....
                var fromPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedByUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedByUserFullName = fromPerson?.displayname;
                task.AssignedByUserEmail = fromPerson?.mail;

                _context.Add(task);
                await _context.SaveChangesAsync();

                // Send Email Out notifying the person who is assigned the task
                string subject = @"Management of Change (MoC) - Impact Assessment Response Task Assigned.";
                string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>Task Title: </strong>" + task.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";

                if (toPerson != null)
                {
                    Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, null, null, task.Priority);
                    AddEmailHistory(task.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, task.ChangeRequestId, null, null, task.Id, "Task", task.Status, DateTime.Now, _username);

                    //EmailHistory emailHistory = new EmailHistory
                    //{
                    //    Priority = task.Priority,
                    //    Subject = subject,
                    //    Body = body,
                    //    SentToDisplayName = toPerson.displayname,
                    //    SentToUsername = toPerson.onpremisessamaccountname,
                    //    SentToEmail = toPerson.mail,
                    //    ChangeRequestId = task.ChangeRequestId,
                    //    TaskId = task.Id,
                    //    Type = "Task",
                    //    Status = task.Status,
                    //    CreatedDate = DateTime.Now,
                    //    CreatedUser = _username
                    //};
                    //_context.Add(emailHistory);
                    //await _context.SaveChangesAsync();
                }

                if (source != null && source == "Home")
                    return RedirectToAction("Index", "Home", new { });
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
        public async Task<IActionResult> Edit(int? id, string? destinationPage = null)
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
            ViewBag.DestinationPage = destinationPage;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeRequestId,MocNumber,ImplementationType,Status,Priority,AssignedToUser,AssignedToUserFullName,AssignedToUserEmail,AssignedByUser,AssignedByUserFullName,AssignedByUserEmail,Title,Description,DueDate,CompletionDate,CompletionNotes,OnHoldReason,CancelledReason,ImpactAssessmentResponseAnswerId,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] Models.Task task, string? destinationPage = null)
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

            if (task.Status == "Cancelled" && String.IsNullOrWhiteSpace(task.CancelledReason))
                ModelState.AddModelError("CancelledReason", "If Task Status is 'Cancelled', Reason is required.");

            if (task.ChangeRequestId != null && task.ImplementationType == null)
                ModelState.AddModelError("ImplementationType", "If task is part of a Change Request, Implementation Type is required.");

            if (task.Status == "Complete" && task.CompletionDate == null)
                ModelState.AddModelError("CompletionDate", "If Task Status is 'Completed, Completion Date is required.");

            if (task.Status != "Complete" && task.CompletionDate != null)
            {
                ModelState.AddModelError("CompletionDate", "Completion Date is entered, but Status is not 'Complete'. (Either Change Status to 'Complete' or clear 'Completion Date')");
                ModelState.AddModelError("Status", "Completion Date is entered, but Status is not 'Complete'. (Either Change Status to 'Complete' or clear 'Completion Date')");
            }

            if (ModelState.IsValid)
            {
                var originalTaskStatus = await _context.Task.Where(m => m.Id == task.Id).Select(m => m.Status).FirstOrDefaultAsync();
                task.MocNumber = await _context.ChangeRequest.Where(m => m.Id == task.ChangeRequestId).Select(m => m.MOC_Number).FirstOrDefaultAsync();
                // get assigned-to person info....
                var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedToUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedToUserFullName = toPerson?.displayname;
                task.AssignedToUserEmail = toPerson?.mail;
                // get assigned-by person info....
                var fromPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedByUser.ToLower()).FirstOrDefaultAsync();
                task.AssignedByUserFullName = fromPerson?.displayname;
                task.AssignedByUserEmail = fromPerson?.mail;
                task.ModifiedUser = _username;
                task.ModifiedDate = DateTime.Now;
                _context.Update(task);
                await _context.SaveChangesAsync();

                // if status was changed to "Complete" then email task creator that it is now complete
                if (originalTaskStatus != task.Status && task.Status == "Complete")
                {
                    // Send Email Out notifying the person who is assigned the task
                    string subject = @"Management of Change (MoC) - Task has been completed.";
                    string body = @"A Management of Change task has been completed.  The task is listed under your Change Request, in the Tasks tab. <br/><br/>
                            <strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>Task Number: </strong>" + task.Id.ToString() + @"<br/><strong>Task Title: </strong>" + task.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";
                    var ccPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.CreatedUser.ToLower()).FirstOrDefaultAsync();
                    if (toPerson != null)
                    {
                        Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, ccPerson?.mail, null, task.Priority);
                        AddEmailHistory(task.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, task.ChangeRequestId, null, null, task.Id, "Task", task.Status, DateTime.Now, _username);

                        //EmailHistory emailHistory = new EmailHistory
                        //{
                        //    Priority = task.Priority,
                        //    Subject = subject,
                        //    Body = body,
                        //    SentToDisplayName = toPerson.displayname,
                        //    SentToUsername = toPerson.onpremisessamaccountname,
                        //    SentToEmail = toPerson.mail,                                
                        //    ChangeRequestId = task.ChangeRequestId,
                        //    TaskId = task.Id,
                        //    Type = "Task",
                        //    Status = task.Status,
                        //    CreatedDate = DateTime.Now,
                        //    CreatedUser = _username
                        //};
                        //_context.Add(emailHistory);
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", new { id = id, destinationPage = destinationPage });
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

        public async Task<IActionResult> TaskReminder(int id, string? destinationPage = null)
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
            string body = @"A Management of Change task has been assigned to you.  Please follow link below and review the task request. <br/><br/><strong>Change Request: </strong>" + task.MocNumber + @"<br/><strong>Task Number: </strong>" + task.Id.ToString() + @"<br/><strong>Task Title: </strong>" + task.Title + "<br/><strong>Link: <a href=\"" + Initialization.WebsiteUrl + "\" target=\"blank\" >MoC System</a></strong><br/><br/>";
            var toPerson = await _context.__mst_employee.Where(m => m.onpremisessamaccountname.ToLower() == task.AssignedToUser.ToLower()).FirstOrDefaultAsync();
            if (toPerson != null)
            {
                Initialization.EmailProviderSmtp.SendMessage(subject, body, toPerson?.mail, null, null, task.Priority);
                var emailHistory = AddEmailHistory(task.Priority, subject, body, toPerson?.displayname, toPerson?.onpremisessamaccountname, toPerson?.mail, task.ChangeRequestId, null, null, task.Id, "Task", task.Status, DateTime.Now, _username);
                //EmailHistory emailHistory = new EmailHistory
                //{
                //    Priority = task.Priority,
                //    Subject = subject,
                //    Body = body,
                //    SentToDisplayName = toPerson.displayname,
                //    SentToUsername = toPerson.onpremisessamaccountname,
                //    SentToEmail = toPerson.mail,
                //    ChangeRequestId = task.ChangeRequestId,
                //    TaskId = task.Id,
                //    Type = "Task",
                //    Status = task.Status,
                //    CreatedDate = DateTime.Now,
                //    CreatedUser = _username
                //};
                //_context.Add(emailHistory);
            }
            //await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id, destinationPage = destinationPage });
        }

        [HttpPost]
        [DisableRequestSizeLimit,RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _context.Task == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", "Tasks", new { id = id, fileAttachmentError = "No File Has Been Selected For Upload", tab = "Attachments" });

            // get PCCB (meeting) record...
            var taskRec = await _context.Task.FindAsync(id);
            if (taskRec == null)
                return RedirectToAction("Index", "Task");

            //// get ChangeRequest
            //var changeRequest = await _context.ChangeRequest.FirstOrDefaultAsync(m => m.Id == taskRec.ChangeRequestId);
            //if (changeRequest == null)
            //    return RedirectToAction("Index", "Home");

            // make sure the file being uploaded is an allowable file extension type....
            var extensionType = Path.GetExtension(fileAttachment.FileName);
            var found = _context.AllowedAttachmentExtensions
                .Where(m => m.ExtensionName == extensionType)
                .Any();

            if (!found)
                return RedirectToAction("Details", new
                {
                    id = id,
                    tab = "Attachments",
                    fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact MoC Admin to add, or change document to allowable type."
                });

            string filePath = Path.Combine(Initialization.TaskDirectory, taskRec.Id.ToString(), fileAttachment.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }

            return RedirectToAction("Details", new { id = id, previousAction = "File Uploaded", tab = "Attachments" });
        }

        public async Task<IActionResult> DownloadFile(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }

        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Details", new { id = id, previousAction = "File Deleted", tab = "Attachments" });
        }
    }
}
