using Microsoft.AspNetCore.Mvc;

namespace EHS.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/Generic")]
        public IActionResult Generic()
        {
            return View("Generic");
        }

        [Route("Error/UserNotFound")]
        public IActionResult UserNotFound(string username)
        {
            ViewBag.Username = username;
            return View("UserNotFound");
        }
    }
}
