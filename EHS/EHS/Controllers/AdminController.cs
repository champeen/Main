using EHS.Data;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EHS.Controllers
{
    public class AdminController : BaseController
    {
        private readonly EHSContext _contextEhs;
        private readonly MOCContext _contextMoc;
        public AdminController(EHSContext contextEhs, MOCContext contextMoc, ILogger<AdminController> logger) : base(contextEhs, contextMoc)
        {
            _contextEhs = contextEhs;
            _contextMoc = contextMoc;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult dropdownSEG()
        {
            return View();
        }

        public IActionResult dropdownCRA()
        {
            return View();
        }
    }
}
