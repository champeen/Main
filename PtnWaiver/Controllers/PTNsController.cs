using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class PTNsController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public PTNsController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: PTNs
        public async Task<IActionResult> Index()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            var requests = await _contextPtnWaiver.PTN.Where(m => m.DeletedDate == null).ToListAsync();

            return View(requests);
        }

        // GET: PTNs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
                return NotFound();

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return View(pTN);
        }

        // GET: PTNs/Create
        public IActionResult Create()
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            var userInfo = getUserInfo(_username);
            if (userInfo == null)
            {
                errorViewModel = new ErrorViewModel() { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = "Invalid Username: " + _username });
            }

            ViewBag.Status = getStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();

            PTN ptn = new PTN()
            {
                CreatedDate = DateTime.Now,
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail
            };

            return View(ptn);
        }

        // POST: PTNs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] PTN ptn)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (ModelState.IsValid)
            {
                // This weird naming convention is striaght from how they are doing it in the spreadsheet.....
                int days = _getDaysSince1900;
                string docId = "";
                for (int i = 1; i < 10000; i++)
                {
                    docId = ptn.PtnPin + "-" + days.ToString() + "-" + i.ToString();
                    PTN record = await _contextPtnWaiver.PTN
                        .FirstOrDefaultAsync(m => m.DocId == docId);
                    if (record == null)
                        break;
                }
                ptn.DocId = docId;

                _contextPtnWaiver.Add(ptn);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Status = getStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            return View(ptn);
        }

        // GET: PTNs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN.FindAsync(id);
            if (pTN == null)
                return NotFound();

            ViewBag.Status = getStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();

            return View(pTN);
        }

        // POST: PTNs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] PTN pTN)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != pTN.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contextPtnWaiver.Update(pTN);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PTNExists(pTN.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Status = getStatus();
            ViewBag.PtnPins = getPtnPins();
            ViewBag.Areas = getAreas();
            ViewBag.Groups = getGroups();
            ViewBag.SubjectTypes = getSubjectTypes();
            return View(pTN);
        }

        // GET: PTNs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // make sure valid Username
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.PTN == null)
                return NotFound();

            var pTN = await _contextPtnWaiver.PTN
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pTN == null)
                return NotFound();

            return View(pTN);
        }

        // POST: PTNs/Delete/5
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

            if (_contextPtnWaiver.PTN == null)
                return Problem("Entity set 'PtnWaiverContext.PTN'  is null.");

            var pTN = await _contextPtnWaiver.PTN.FindAsync(id);
            if (pTN != null)
                  _contextPtnWaiver.PTN.Remove(pTN);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PTNExists(int id)
        {
          return (_contextPtnWaiver.PTN?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
