using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class AdminController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(Management_of_ChangeContext context, ILogger<AdminController> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (!_isAdmin)
                return RedirectToAction("Unauthorized", "Home", new { message = "Must be setup as an Administrator to have access." });

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
        public async Task<IActionResult> CancelConfirm(CancelChangeRequest cancelChangeRequest)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (string.IsNullOrWhiteSpace(cancelChangeRequest.MocNumber))
            {
                ModelState.AddModelError("MocNumber", "MoC Number is Required");
                return View("CancelSelect", cancelChangeRequest);
            }

            ChangeRequest changeRequest = await _context.ChangeRequest.Where(m => m.MOC_Number == cancelChangeRequest.MocNumber).FirstOrDefaultAsync();
            if (changeRequest == null)
            {
                ModelState.AddModelError("MocNumber", "Change Request Does Not Exist - Enter A Valid MoC Number");
                return View("CancelSelect");
            }

            if (string.IsNullOrWhiteSpace(cancelChangeRequest.CancelReason))
            {
                ModelState.AddModelError("CancelReason", "Cancel Reason is Required");
                return View("CancelSelect", cancelChangeRequest);
            }

            changeRequest.Cancel_Username = _username;
            changeRequest.Cancel_Date = DateTime.Now;
            changeRequest.Cancel_Reason = cancelChangeRequest.CancelReason;
            //cancelChangeRequest.ChangeRequest = changeRequest;

            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string cancelReason)
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
                changeRequest.Cancel_Username = _username;
                changeRequest.Cancel_Date = DateTime.Now;
                changeRequest.Cancel_Reason = cancelReason;
                changeRequest.ModifiedDate = DateTime.Now;
                changeRequest.ModifiedUser = _username;
                _context.Update(changeRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
