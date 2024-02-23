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
    public class ProductProcessesController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public ProductProcessesController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: ProductProcesses
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.ProductProcess != null ? 
                          View(await _contextPtnWaiver.ProductProcess.ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.ProductProcess'  is null.");
        }

        // GET: ProductProcesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductProcess == null)
                return NotFound();

            var productProcess = await _contextPtnWaiver.ProductProcess
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productProcess == null)
                 return NotFound();

            return View(productProcess);
        }

        // GET: ProductProcesses/Create
        public IActionResult Create()
        {
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

            ProductProcess productProcess = new ProductProcess()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(productProcess);
        }

        // POST: ProductProcesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] ProductProcess productProcess)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ProductProcess> checkDupes = await _contextPtnWaiver.ProductProcess
                .Where(m => m.Code == productProcess.Code)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "Product/Process Code already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(productProcess);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productProcess);
        }

        // GET: ProductProcesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductProcess == null)
                return NotFound();

            var productProcess = await _contextPtnWaiver.ProductProcess.FindAsync(id);
            if (productProcess == null)
                return NotFound();

            return View(productProcess);
        }

        // POST: ProductProcesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] ProductProcess productProcess)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != productProcess.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ProductProcess> checkDupes = await _contextPtnWaiver.ProductProcess
                .Where(m => m.Code == productProcess.Code && m.Id != productProcess.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "Product/Process Code already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    productProcess.ModifiedDate = DateTime.Now;
                    productProcess.ModifiedUser = userInfo.onpremisessamaccountname;
                    productProcess.ModifiedUserFullName = userInfo.displayname;
                    productProcess.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(productProcess);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductProcessExists(productProcess.Id))
                         return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productProcess);
        }

        // GET: ProductProcesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductProcess == null)
                return NotFound();

            var productProcess = await _contextPtnWaiver.ProductProcess
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productProcess == null)
                return NotFound();

            return View(productProcess);
        }

        // POST: ProductProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.ProductProcess == null)
                return Problem("Entity set 'PtnWaiverContext.ProductProcess'  is null.");

            var productProcess = await _contextPtnWaiver.ProductProcess.FindAsync(id);
            if (productProcess != null)
                 _contextPtnWaiver.ProductProcess.Remove(productProcess);
            
            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductProcessExists(int id)
        {
          return (_contextPtnWaiver.ProductProcess?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
