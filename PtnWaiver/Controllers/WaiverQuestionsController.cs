using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class WaiverQuestionsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public WaiverQuestionsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: WaiverQuestions
        public async Task<IActionResult> Index()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.WaiverQuestion != null ? 
                          View(await _contextPtnWaiver.WaiverQuestion.OrderBy(m => m.Order).ThenBy(m=>m.Question).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.WaiverQuestion'  is null.");
        }

        // GET: WaiverQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.WaiverQuestion == null)
                return NotFound();

            var waiverQuestion = await _contextPtnWaiver.WaiverQuestion.FirstOrDefaultAsync(m => m.Id == id);
            if (waiverQuestion == null)
                return NotFound();

            return View(waiverQuestion);
        }

        // GET: WaiverQuestions/Create
        public IActionResult Create()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.GroupApprovers = getGroupApprovers();

            var userInfo = getUserInfo(_username);
            if (userInfo == null)
            {
                errorViewModel = new ErrorViewModel() { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = "Invalid Username: " + _username });
            }

            WaiverQuestion waiverQuestion = new WaiverQuestion
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now,
            };

            return View(waiverQuestion);
        }

        // POST: WaiverQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,GroupApprover,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] WaiverQuestion waiverQuestion)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<WaiverQuestion> checkDupes = await _contextPtnWaiver.WaiverQuestion
                .Where(m => m.Question == waiverQuestion.Question)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Question", "Question already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(waiverQuestion);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GroupApprovers = getGroupApprovers();
            return View(waiverQuestion);
        }

        // GET: WaiverQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;
            ViewBag.GroupApprovers = getGroupApprovers();

            if (id == null || _contextPtnWaiver.WaiverQuestion == null)
                return NotFound();

            var waiverQuestion = await _contextPtnWaiver.WaiverQuestion.FindAsync(id);
            if (waiverQuestion == null)
                return NotFound();

            return View(waiverQuestion);
        }

        // POST: WaiverQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,GroupApprover,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] WaiverQuestion waiverQuestion)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != waiverQuestion.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    waiverQuestion.ModifiedUser = userInfo.onpremisessamaccountname;
                    waiverQuestion.ModifiedUserFullName = userInfo.displayname;
                    waiverQuestion.ModifiedUserEmail = userInfo.mail;
                    waiverQuestion.ModifiedDate = DateTime.Now;
                }
                try
                {
                    _contextPtnWaiver.Update(waiverQuestion);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaiverQuestionExists(waiverQuestion.Id))
                        return NotFound();
                    else
                         throw;
                }
                ViewBag.GroupApprovers = getGroupApprovers();
                return RedirectToAction(nameof(Index));
            }
            return View(waiverQuestion);
        }

        // GET: WaiverQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.WaiverQuestion == null)
                return NotFound();

            var waiverQuestion = await _contextPtnWaiver.WaiverQuestion.FirstOrDefaultAsync(m => m.Id == id);
            if (waiverQuestion == null)
                return NotFound();

            return View(waiverQuestion);
        }

        // POST: WaiverQuestions/Delete/5
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

            if (_contextPtnWaiver.WaiverQuestion == null)
                return Problem("Entity set 'PtnWaiverContext.WaiverQuestion' is null.");

            var waiverQuestion = await _contextPtnWaiver.WaiverQuestion.FindAsync(id);
            if (waiverQuestion != null)
                _contextPtnWaiver.WaiverQuestion.Remove(waiverQuestion);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaiverQuestionExists(int id)
        {
          return (_contextPtnWaiver.WaiverQuestion?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
