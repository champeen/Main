using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.SEG;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers.SEG.Dropdowns
{
    public class health_effect_ratingController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public health_effect_ratingController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: health_effect_rating
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.health_effect_rating.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: health_effect_rating/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var health_effect_rating = await _contextEHS.health_effect_rating.FirstOrDefaultAsync(m => m.id == id);
            if (health_effect_rating == null)
                return NotFound();

            return View(health_effect_rating);
        }

        // GET: health_effect_rating/Create
        public async Task<IActionResult> Create()
        {
            health_effect_rating health_effect_rating = new health_effect_rating();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            health_effect_rating.created_user = employee.onpremisessamaccountname;
            health_effect_rating.created_user_fullname = employee.displayname;
            health_effect_rating.created_user_email = employee.mail;
            health_effect_rating.created_date = DateTime.Now;
            return View(health_effect_rating);
        }

        // POST: health_effect_rating/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,value,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] health_effect_rating health_effect_rating)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(health_effect_rating);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(health_effect_rating);
        }

        // GET: health_effect_rating/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var health_effect_rating = await _contextEHS.health_effect_rating.FindAsync(id);
            if (health_effect_rating == null)
                return NotFound();

            return View(health_effect_rating);
        }

        // POST: health_effect_rating/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,value,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] health_effect_rating health_effect_rating)
        {
            if (id != health_effect_rating.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    health_effect_rating.modified_user = employee.onpremisessamaccountname;
                    health_effect_rating.modified_user_fullname = employee.displayname;
                    health_effect_rating.modified_user_email = employee.mail;
                    health_effect_rating.modified_date = DateTime.Now;
                    _contextEHS.Update(health_effect_rating);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!health_effect_ratingExists(health_effect_rating.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(health_effect_rating);
        }

        // GET: health_effect_rating/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var health_effect_rating = await _contextEHS.health_effect_rating.FirstOrDefaultAsync(m => m.id == id);
            if (health_effect_rating == null)
                return NotFound();

            return View(health_effect_rating);
        }

        // POST: health_effect_rating/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var health_effect_rating = await _contextEHS.health_effect_rating.FindAsync(id);
            if (health_effect_rating == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            health_effect_rating.deleted_user = _username;
            health_effect_rating.deleted_user_fullname = employee.displayname;
            health_effect_rating.deleted_user_email = employee.mail;
            health_effect_rating.deleted_date = DateTime.Now;
            _contextEHS.Update(health_effect_rating);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool health_effect_ratingExists(int id)
        {
            return _contextEHS.health_effect_rating.Any(e => e.id == id);
        }
    }
}
