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
    public class ReviewTypesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public ReviewTypesController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: ReviewTypes
        public async Task<IActionResult> Index(string typeFilter = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.ReviewTypes = (await _context.ReviewType.Select(m => m.Type).Distinct().ToListAsync()).OrderBy(x => x).ToList();

            List<ReviewType> reviewTypes = new List<ReviewType>();
            if (typeFilter == null)
                reviewTypes = await _context.ReviewType.OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ThenBy(m => m.Reviewer).ToListAsync();
            else
                reviewTypes = await _context.ReviewType.Where(m => m.Type == typeFilter).OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ThenBy(m => m.Reviewer).ToListAsync();

            return View(reviewTypes);
        }

        public async Task<IActionResult> IndexHelp(string typeFilter = null)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.ReviewTypes = (await _context.ReviewType.Select(m => m.Type).Distinct().ToListAsync()).OrderBy(x => x).ToList();

            List<ReviewType> reviewTypes = new List<ReviewType>();
            if (typeFilter == null)
                reviewTypes = await _context.ReviewType.OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ThenBy(m => m.Reviewer).ToListAsync();
            else
                reviewTypes = await _context.ReviewType.Where(m => m.Type == typeFilter).OrderBy(m => m.Type).ThenBy(m => m.ChangeArea).ThenBy(m => m.Reviewer).ToListAsync();

            return View(reviewTypes);
        }

        // GET: ReviewTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reviewType == null)
                return NotFound();

            return View(reviewType);
        }

        // GET: ReviewTypes/Create
        public async Task<IActionResult> Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ReviewType reviewType = new ReviewType
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            // Create Dropdown List of ChangeAreas...
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Users = getUserList();

            return View(reviewType);
        }

        // POST: ReviewTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,ChangeArea,Order,Username,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ReviewType reviewType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ReviewType> checkDupes = await _context.ReviewType
                .Where(m => m.Type == reviewType.Type && m.ChangeArea == reviewType.ChangeArea && m.Username == reviewType.Username)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Type", "Review Type already exists.");

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname.ToLower() == reviewType.Username.ToLower());
            if (employee != null)
            {
                reviewType.Reviewer = employee.displayname;
                reviewType.Email = employee.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + reviewType.Username);

            if (String.IsNullOrWhiteSpace(reviewType.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(reviewType.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(reviewType.Email))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (ModelState.IsValid)
            {
                _context.Add(reviewType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Create Dropdown List of ChangeAreas...
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();

            // Create Dropdown List of Users...
            ViewBag.Users = getUserList();

            return View(reviewType);
        }

        // GET: ReviewTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType.FindAsync(id);

            if (reviewType == null)
                return NotFound();

            // Create Dropdown List of ChangeAreas...
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();

            //// Create Dropdown List of Users...
            //var userList = await _context.__mst_employee
            //    .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
            //    .Where(m => m.accountenabled == true)
            //    .Where(m => !String.IsNullOrWhiteSpace(m.mail))
            //    .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
            //    .OrderBy(m => m.displayname)
            //    .ThenBy(m => m.onpremisessamaccountname)
            //    .ToListAsync();
            //List<SelectListItem> users = new List<SelectListItem>();
            //foreach (var user in userList)
            //{
            //    SelectListItem item = new SelectListItem { Value = user.onpremisessamaccountname, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };
            //    if (user.onpremisessamaccountname == reviewType.Username)
            //        item.Selected = true;
            //    users.Add(item);
            //}
            //ViewBag.Users = users;
            ViewBag.Users = getUserList(reviewType.Username);

            return View(reviewType);
        }

        // POST: ReviewTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,ChangeArea,Order,Username,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ReviewType reviewType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != reviewType.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ReviewType> checkDupes = await _context.ReviewType
                .Where(m => m.Type == reviewType.Type && m.Id != reviewType.Id && m.ChangeArea == reviewType.ChangeArea && m.Username == reviewType.Username)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Type", "Review Type already exists.");

            // make sure all selected employee data is found, valid and correct
            __mst_employee employee = await _context.__mst_employee.FirstOrDefaultAsync(m => m.onpremisessamaccountname.ToLower() == reviewType.Username.ToLower());
            if (employee != null)
            {
                reviewType.Reviewer = employee.displayname;
                reviewType.Email = employee.mail;
            }
            else
                ModelState.AddModelError("Username", "Employee record not found for Username: " + reviewType.Username);

            if (String.IsNullOrWhiteSpace(reviewType.Username))
                ModelState.AddModelError("Username", "Employee record has a blank Username");
            if (String.IsNullOrWhiteSpace(reviewType.Reviewer))
                ModelState.AddModelError("Username", "Employee record has a blank Display Name");
            if (String.IsNullOrWhiteSpace(reviewType.Email))
                ModelState.AddModelError("Username", "Employee record has a blank Email");

            if (ModelState.IsValid)
            {
                reviewType.ModifiedUser = _username;
                reviewType.ModifiedDate = DateTime.Now;
                try
                {
                    _context.Update(reviewType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewTypeExists(reviewType.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Create Dropdown List of ChangeAreas...
            ViewBag.ChangeAreas = await _context.ChangeArea.OrderBy(m => m.Order).Select(m => m.Description).ToListAsync();
            ViewBag.Users = getUserList(reviewType.Username);

            return View(reviewType);
        }

        // GET: ReviewTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ReviewType == null)
                return NotFound();

            var reviewType = await _context.ReviewType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reviewType == null)
                return NotFound();

            return View(reviewType);
        }

        // POST: ReviewTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ReviewType == null)
                return Problem("Entity set 'Management_of_ChangeContext.ReviewType'  is null.");

            var reviewType = await _context.ReviewType.FindAsync(id);

            if (reviewType != null)
                _context.ReviewType.Remove(reviewType);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewTypeExists(int id)
        {
          return (_context.ReviewType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
