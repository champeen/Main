using Management_of_Change.Models;
using Management_of_Change.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Management_of_Change.ViewModels;
using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Controllers
{
    public class HomeController : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Management_of_ChangeContext context, ILogger<HomeController> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = message });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Unauthorized(string? message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = message });
        }
    }
}