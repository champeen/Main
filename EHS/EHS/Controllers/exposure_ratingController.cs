using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers
{
    public class exposure_ratingController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public exposure_ratingController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: exposure_rating
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.exposure_rating.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: exposure_rating/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_rating = await _contextEHS.exposure_rating.FirstOrDefaultAsync(m => m.id == id);
            if (exposure_rating == null)
                return NotFound();

            return View(exposure_rating);
        }

        // GET: exposure_rating/Create
        public async Task<IActionResult> Create()
        {
            exposure_rating exposure_rating = new exposure_rating();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            exposure_rating.created_user = employee.onpremisessamaccountname;
            exposure_rating.created_user_fullname = employee.displayname;
            exposure_rating.created_user_email = employee.mail;
            exposure_rating.created_date = DateTime.Now;
            return View(exposure_rating);
        }

        // POST: exposure_rating/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,value,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] exposure_rating exposure_rating)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(exposure_rating);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exposure_rating);
        }

        // GET: exposure_rating/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_rating = await _contextEHS.exposure_rating.FindAsync(id);
            if (exposure_rating == null)
                return NotFound();

            return View(exposure_rating);
        }

        // POST: exposure_rating/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,value,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] exposure_rating exposure_rating)
        {
            if (id != exposure_rating.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    exposure_rating.modified_user = employee.onpremisessamaccountname;
                    exposure_rating.modified_user_fullname = employee.displayname;
                    exposure_rating.modified_user_email = employee.mail;
                    exposure_rating.modified_date = DateTime.Now;
                    _contextEHS.Update(exposure_rating);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!exposure_ratingExists(exposure_rating.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exposure_rating);
        }

        // GET: exposure_rating/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_rating = await _contextEHS.exposure_rating.FirstOrDefaultAsync(m => m.id == id);
            if (exposure_rating == null)
                return NotFound();

            return View(exposure_rating);
        }

        // POST: exposure_rating/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exposure_rating = await _contextEHS.exposure_rating.FindAsync(id);
            if (exposure_rating == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            exposure_rating.deleted_user = _username;
            exposure_rating.deleted_user_fullname = employee.displayname;
            exposure_rating.deleted_user_email = employee.mail;
            exposure_rating.deleted_date = DateTime.Now;
            _contextEHS.Update(exposure_rating);
            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool exposure_ratingExists(int id)
        {
            return _contextEHS.exposure_rating.Any(e => e.id == id);
        }
    }
}
