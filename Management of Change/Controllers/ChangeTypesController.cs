using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class ChangeTypesController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly PtnWaiverContext _contextPtnWaiver;

        public ChangeTypesController(Management_of_ChangeContext context, PtnWaiverContext contextPtnWaiver) : base(context, contextPtnWaiver)
        {
            _context = context;
            _contextPtnWaiver = contextPtnWaiver;
        }

        // GET: ChangeTypes
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ChangeType != null ? 
                          View(await _context.ChangeType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeType'  is null.");
        }

        public async Task<IActionResult> IndexHelp()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _context.ChangeType != null ?
                          View(await _context.ChangeType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ChangeType'  is null.");
        }

        // GET: ChangeTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeType == null)
                return NotFound();

            var changeType = await _context.ChangeType
                .FirstOrDefaultAsync(m => m.Id == id);

            if (changeType == null)
                return NotFound();

            return View(changeType);
        }

        // GET: ChangeTypes/Create
        public IActionResult Create()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            ChangeType changeType = new ChangeType
            {
                CreatedUser = _username,
                CreatedDate = DateTime.Now
            };

            return View(changeType);
        }

        // POST: ChangeTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeType changeType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ChangeType> checkDupes = await _context.ChangeType
                .Where(m => m.Type == changeType.Type)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Type", "Change Type already exists.");
                return View(changeType);
            }

            if (ModelState.IsValid)
            {
                _context.Add(changeType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(changeType);
        }

        // GET: ChangeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeType == null)
                return NotFound();

            var changeType = await _context.ChangeType.FindAsync(id);

            if (changeType == null)
                return NotFound();

            return View(changeType);
        }

        // POST: ChangeTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ChangeType changeType)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != changeType.Id)
                return NotFound();

            //// Make sure duplicates are not entered...
            //List<ChangeType> checkDupes = await _context.ChangeType
            //    .Where(m => m.Type == changeType.Type)
            //    .ToListAsync();
            //if (checkDupes.Count > 0)
            //{
            //    ModelState.AddModelError("Type", "Change Type already exists.");
            //    return View(changeType);
            //}

            changeType.ModifiedUser = _username;
            changeType.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(changeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChangeTypeExists(changeType.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(changeType);
        }

        // GET: ChangeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _context.ChangeType == null)
                return NotFound();

            var changeType = await _context.ChangeType.FirstOrDefaultAsync(m => m.Id == id);

            if (changeType == null)
                return NotFound();

            return View(changeType);
        }

        // POST: ChangeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_context.ChangeType == null)
                return Problem("Entity set 'Management_of_ChangeContext.ChangeType'  is null.");

            var changeType = await _context.ChangeType.FindAsync(id);

            if (changeType != null)
                _context.ChangeType.Remove(changeType);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChangeTypeExists(int id)
        {
          return (_context.ChangeType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
