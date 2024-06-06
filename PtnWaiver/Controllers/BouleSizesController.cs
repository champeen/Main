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
    public class ProductSizesController : BaseController
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        public ProductSizesController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc) : base(contextPtnWaiver, contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }

        // GET: ProductSizes
        public async Task<IActionResult> Index()
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            return _contextPtnWaiver.ProductSize != null ?
                          View(await _contextPtnWaiver.ProductSize.OrderBy(m=>m.Order).ThenBy(m=>m.Description).ToListAsync()) :
                          Problem("Entity set 'PtnWaiverContext.ProductSize'  is null.");
        }

        // GET: ProductSizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductSize == null)
                return NotFound();

            var productSize = await _contextPtnWaiver.ProductSize
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productSize == null)
                return NotFound();

            return View(productSize);
        }

        // GET: ProductSizes/Create
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

            ProductSize productSize = new ProductSize()
            {
                CreatedUser = userInfo.onpremisessamaccountname,
                CreatedUserFullName = userInfo.displayname,
                CreatedUserEmail = userInfo.mail,
                CreatedDate = DateTime.Now
            };

            return View(productSize);
        }

        // POST: ProductSizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate")] ProductSize productSize)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            // Make sure duplicates are not entered...
            List<ProductSize> checkDupes = await _contextPtnWaiver.ProductSize
                .Where(m => m.Code == productSize.Code)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "ProductSize Code already exists.");

            if (ModelState.IsValid)
            {
                _contextPtnWaiver.Add(productSize);
                await _contextPtnWaiver.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productSize);
        }

        // GET: ProductSizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductSize == null)
                return NotFound();

            var productSize = await _contextPtnWaiver.ProductSize.FindAsync(id);

            if (productSize == null)
                return NotFound();

            return View(productSize);
        }

        // POST: ProductSizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Description,Order,CreatedUser,CreatedUserFullName,CreatedUserEmail,CreatedDate,ModifiedUser,ModifiedUserFullName,ModifiedUserEmail,ModifiedDate,DeletedUser,DeletedUserFullName,DeletedUserEmail,DeletedDate")] ProductSize productSize)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id != productSize.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ProductSize> checkDupes = await _contextPtnWaiver.ProductSize
                .Where(m => m.Code == productSize.Code && m.Id != productSize.Id)
                .ToListAsync();
            if (checkDupes.Count > 0)
                ModelState.AddModelError("Code", "ProductSize Code already exists.");

            if (ModelState.IsValid)
            {
                var userInfo = getUserInfo(_username);
                if (userInfo != null)
                {
                    productSize.ModifiedDate = DateTime.Now;
                    productSize.ModifiedUser = userInfo.onpremisessamaccountname;
                    productSize.ModifiedUserFullName = userInfo.displayname;
                    productSize.ModifiedUserEmail = userInfo.mail;
                }
                try
                {
                    _contextPtnWaiver.Update(productSize);
                    await _contextPtnWaiver.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizeExists(productSize.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productSize);
        }

        // GET: ProductSizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (id == null || _contextPtnWaiver.ProductSize == null)
                return NotFound();

            var productSize = await _contextPtnWaiver.ProductSize
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productSize == null)
                return NotFound();

            return View(productSize);
        }

        // POST: ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ErrorViewModel errorViewModel = CheckAuthorization();
            if (errorViewModel != null && !String.IsNullOrEmpty(errorViewModel.ErrorMessage))
                return RedirectToAction(errorViewModel.Action, errorViewModel.Controller, new { message = errorViewModel.ErrorMessage });

            ViewBag.IsAdmin = _isAdmin;
            ViewBag.Username = _username;

            if (_contextPtnWaiver.ProductSize == null)
                return Problem("Entity set 'PtnWaiverContext.ProductSize'  is null.");

            var productSize = await _contextPtnWaiver.ProductSize.FindAsync(id);

            if (productSize != null)
                _contextPtnWaiver.ProductSize.Remove(productSize);

            await _contextPtnWaiver.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSizeExists(int id)
        {
            return (_contextPtnWaiver.ProductSize?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
