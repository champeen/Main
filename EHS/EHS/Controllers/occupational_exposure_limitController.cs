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
    public class occupational_exposure_limitController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public occupational_exposure_limitController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: occupational_exposure_limit
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.occupational_exposure_limit.OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: occupational_exposure_limit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var occupational_exposure_limit = await _contextEHS.occupational_exposure_limit.FirstOrDefaultAsync(m => m.id == id);
            if (occupational_exposure_limit == null)
                return NotFound();

            return View(occupational_exposure_limit);
        }

        // GET: occupational_exposure_limit/Create
        public async Task<IActionResult> Create()
        {
            occupational_exposure_limit occupational_exposure_limit = new occupational_exposure_limit();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            occupational_exposure_limit.created_user = employee.onpremisessamaccountname;
            occupational_exposure_limit.created_user_fullname = employee.displayname;
            occupational_exposure_limit.created_user_email = employee.mail;
            occupational_exposure_limit.created_date = DateTime.Now;
            return View(occupational_exposure_limit);
        }

        // POST: occupational_exposure_limit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] occupational_exposure_limit occupational_exposure_limit)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(occupational_exposure_limit);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(occupational_exposure_limit);
        }

        // GET: occupational_exposure_limit/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var occupational_exposure_limit = await _contextEHS.occupational_exposure_limit.FindAsync(id);
            if (occupational_exposure_limit == null)
                return NotFound();

            return View(occupational_exposure_limit);
        }

        // POST: occupational_exposure_limit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] occupational_exposure_limit occupational_exposure_limit)
        {
            if (id != occupational_exposure_limit.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    occupational_exposure_limit.modified_user = employee.onpremisessamaccountname;
                    occupational_exposure_limit.modified_user_fullname = employee.displayname;
                    occupational_exposure_limit.modified_user_email = employee.mail;
                    occupational_exposure_limit.modified_date = DateTime.Now;
                    _contextEHS.Update(occupational_exposure_limit);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!occupational_exposure_limitExists(occupational_exposure_limit.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(occupational_exposure_limit);
        }

        // GET: occupational_exposure_limit/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var occupational_exposure_limit = await _contextEHS.occupational_exposure_limit.FirstOrDefaultAsync(m => m.id == id);
            if (occupational_exposure_limit == null)
                return NotFound();

            return View(occupational_exposure_limit);
        }

        // POST: occupational_exposure_limit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var occupational_exposure_limit = await _contextEHS.occupational_exposure_limit.FindAsync(id);
            if (occupational_exposure_limit != null)
                _contextEHS.occupational_exposure_limit.Remove(occupational_exposure_limit);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool occupational_exposure_limitExists(int id)
        {
            return _contextEHS.occupational_exposure_limit.Any(e => e.id == id);
        }
    }
}
