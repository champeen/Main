using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class HelpController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly ILogger<AdminController> _logger;

        public HelpController(Management_of_ChangeContext context, ILogger<AdminController> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            //if (!_isAdmin)
            //    return RedirectToAction("Unauthorized", "Home", new { message = "Must be setup as an Administrator to have access." });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View();
        }

        public async Task<IActionResult> CancelSelect()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            //if (id == null || _context.ChangeRequest == null)
            //    return NotFound();

            //var changeRequest = await _context.ChangeRequest
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (changeRequest == null)
            //    return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View();
        }

        //[HttpPost, ActionName("CancelConfirm")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirm(string MocNumber)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (string.IsNullOrWhiteSpace(MocNumber))
            {
                ModelState.AddModelError("MocNumber", "MoC Number Required");
                return View("CancelSelect");
            }

            ChangeRequest changeRequest = await _context.ChangeRequest.Where(m => m.MOC_Number == MocNumber).FirstOrDefaultAsync();
            if (changeRequest == null)
            {
                ModelState.AddModelError("MocNumber", "Change Request Does Not Exist - Enter A Valid MoC Number");
                return View("CancelSelect");
            }

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            var changeRequest = await _context.ChangeRequest.FindAsync(id);
            if (changeRequest == null)
            {                
                ModelState.AddModelError("MocNumber", "MoC Did Not Exist at time of Cancel");
                return View("CancelSelect");
            }
            else
            {
                changeRequest.Change_Status = "Cancelled";
                changeRequest.Change_Status_Description = await _context.ChangeStatus.Where(m => m.Status == "Cancelled").Select(m => m.Description).FirstOrDefaultAsync();
                changeRequest.ModifiedDate = DateTime.UtcNow;
                changeRequest.ModifiedUser = _username;
                _context.Update(changeRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
