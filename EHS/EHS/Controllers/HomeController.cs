using EHS.Data;
using EHS.Models;
using EHS.Utilities;
using EHS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EHS.Controllers
{
    public class HomeController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public HomeController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        public async Task<IActionResult> Index()
        {
            var employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();

            if (employee == null)
            {
                var ex = new Exception($"User '{_username}' not found.");
                ErrorHandling.HandleException(ex);

                return RedirectToAction("UserNotFound", "Error", new { username = _username });
            }

            ViewBag.Username = employee.onpremisessamaccountname;
            ViewBag.UserDisplayName = employee.displayname;

            // GET SEG RISK ASSESSMENT MATRIX DASHBOARD.....
            var assessments = await _contextEHS.seg_risk_assessment
                    .Where(r => r.exposure_rating.HasValue && r.health_effect_rating.HasValue)
                    .ToListAsync();

            var matrix = new List<RiskCell>();

            for (int severity = 1; severity <= 4; severity++)
            {
                for (int likelihood = 1; likelihood <= 4; likelihood++)
                {
                    var count = assessments.Count(a =>
                        a.exposure_rating == severity &&
                        a.health_effect_rating == likelihood);

                    var color = GetColorForScore(severity * likelihood); // Example logic
                    matrix.Add(new RiskCell
                    {
                        Severity = severity,
                        Likelihood = likelihood,
                        Quantity = count,
                        Color = color
                    });
                }
            }

            ViewBag.RiskMatrix = matrix;

            // GET REVIEWS NEEDED WITHIN 30 DAYS....
            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);

            ViewBag.UpcomingReviews = _contextEHS.seg_risk_assessment
                .Where(r =>
                    r.date_conducted != null && (
                        (r.date_reviewed == null &&
                            r.date_conducted.Value.AddYears(5) <= thirtyDaysFromNow)
                        ||
                        (r.date_reviewed != null &&
                            r.date_reviewed.Value.AddYears(5) <= thirtyDaysFromNow)
                    )
                )
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
        public IActionResult StatusCode(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }

            return View("Error");
        }

        private string GetColorForScore(int score)
        {
            if (score >= 12) return "bg-danger text-white";
            if (score >= 6) return "bg-warning text-dark";
            //if (score >= 4) return "bg-info text-white";
            return "bg-success text-white";
        }
    }
}
