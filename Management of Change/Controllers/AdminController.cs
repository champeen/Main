using Management_of_Change.Data;
using Management_of_Change.Utilities;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}
