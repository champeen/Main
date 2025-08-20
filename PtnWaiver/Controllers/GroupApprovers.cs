using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class GroupApproversController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public GroupApproversController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: Areas
        public async Task<IActionResult> Index()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.GroupApprovers != null ?
                          View(await _contextPtnWaiver.GroupApprovers.OrderBy(m => m.Order).ThenBy(m => m.Group).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.Area'  is null.");
        }

        // GET: Areas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.GroupApprovers == null)
                return NotFound();

            var area = await _contextPtnWaiver.GroupApprovers.FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // GET: Areas/Create
        public IActionResult Create()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Users = getUserList();

            var userInfo = getUserInfo(_username);
            if (userInfo == null)
            {
                errorViewModel = new ErrorViewModel() { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = "Invalid Username: " + _username });
            }

            GroupApprovers departmentArea = new GroupApprovers()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(departmentArea);
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Group,PrimaryApproverUsername,SecondaryApproverUsername,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] GroupApprovers departmentArea)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<GroupApprovers> checkDupes = await _contextPtnWaiver.GroupApprovers
                .Where(m => m.Group == departmentArea.Group)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "Area Code already exists.");

            if (departmentArea.PrimaryApproverUsername != null)
            {
                var primaryUser = await _contextMoc.__mst_employee.FirstOrDefaultAsync(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (departmentArea.PrimaryApproverUsername ?? string.Empty).ToLower());
                if (primaryUser != null)
                {
                    departmentArea.PrimaryApproverEmail = primaryUser.mail;
                    departmentArea.PrimaryApproverFullName = primaryUser.displayname;
                    departmentArea.PrimaryApproverTitle = primaryUser.jobtitle;
                }
            }
            else
                ModelState.AddModelError("PrimaryApproverUsername", "Primary Approver needs to be selected or does not exist in database.");

            if (departmentArea.SecondaryApproverUsername != null)
            {
                var secondaryUser = await _contextMoc.__mst_employee.FirstOrDefaultAsync(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (departmentArea.SecondaryApproverUsername ?? string.Empty).ToLower());
                if (secondaryUser != null)
                {
                    departmentArea.SecondaryApproverEmail = secondaryUser.mail;
                    departmentArea.SecondaryApproverFullName = secondaryUser.displayname;
                    departmentArea.SecondaryApproverTitle = secondaryUser.jobtitle;
                }
            }

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(departmentArea);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = getUserList();
            return View(departmentArea);
        }

        // GET: Areas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.Users = getUserList();

            if (id == null || _contextPtnWaiver.GroupApprovers == null)
                return NotFound();

            var area = await _contextPtnWaiver.GroupApprovers.FindAsync(id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Group,PrimaryApproverUsername,SecondaryApproverUsername,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] GroupApprovers departmentArea)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != departmentArea.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<GroupApprovers> checkDupes = await _contextPtnWaiver.GroupApprovers
                .Where(m => m.Group == departmentArea.Group && m.Id != departmentArea.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "Area Code already exists.");

            if (departmentArea.PrimaryApproverUsername != null)
            {
                var primaryUser = await _contextMoc.__mst_employee.FirstOrDefaultAsync(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (departmentArea.PrimaryApproverUsername ?? string.Empty).ToLower());
                if (primaryUser != null)
                {
                    departmentArea.PrimaryApproverEmail = primaryUser.mail;
                    departmentArea.PrimaryApproverFullName = primaryUser.displayname;
                    departmentArea.PrimaryApproverTitle = primaryUser.jobtitle;
                }
            }
            else
                ModelState.AddModelError("PrimaryApproverUsername", "Primary Approver needs to be selected or does not exist in database.");

            if (departmentArea.SecondaryApproverUsername != null)
            {
                var secondaryUser = await _contextMoc.__mst_employee.FirstOrDefaultAsync(m => (m.onpremisessamaccountname ?? string.Empty).ToLower() == (departmentArea.SecondaryApproverUsername ?? string.Empty).ToLower());
                if (secondaryUser != null)
                {
                    departmentArea.SecondaryApproverEmail = secondaryUser.mail;
                    departmentArea.SecondaryApproverFullName = secondaryUser.displayname;
                    departmentArea.SecondaryApproverTitle = secondaryUser.jobtitle;
                }
            }

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    departmentArea.ModifiedUser = userInfo.onpremisessamaccountname;
                    departmentArea.ModifiedUserFullName = userInfo.displayname;
                    departmentArea.ModifiedUserEmail = userInfo.mail;
                    departmentArea.ModifiedDate = DateTime.Now;
                }
                try
                {
                    _contextPtnWaiver.Update(departmentArea);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(departmentArea.Id))
                        return NotFound();
                    else
                        throw;
                }                
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = getUserList();
            return View(departmentArea);
        }

        // GET: Areas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.GroupApprovers == null)
                return NotFound();

            var area = await _contextPtnWaiver.GroupApprovers.FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // POST: Areas/Delete/5
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

            if (_contextPtnWaiver.GroupApprovers == null)
                return Problem("Entity set 'PtnWaiverContext.Area' is null.");

            var area = await _contextPtnWaiver.GroupApprovers.FindAsync(id);
            if (area != null)
                _contextPtnWaiver.GroupApprovers.Remove(area);

            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(int id)
        {
            return (_contextPtnWaiver.GroupApprovers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
