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
    public class controls_recommendedController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public controls_recommendedController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: controls_recommended
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.controls_recommended.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: controls_recommended/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var controls_recommended = await _contextEHS.controls_recommended.FirstOrDefaultAsync(m => m.id == id);
            if (controls_recommended == null)
                return NotFound();

            return View(controls_recommended);
        }

        // GET: controls_recommended/Create
        public async Task<IActionResult> Create()
        {
            controls_recommended controls_recommended = new controls_recommended();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            controls_recommended.created_user = employee.onpremisessamaccountname;
            controls_recommended.created_user_fullname = employee.displayname;
            controls_recommended.created_user_email = employee.mail;
            controls_recommended.created_date = DateTime.Now;
            return View(controls_recommended);
        }

        // POST: controls_recommended/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] controls_recommended controls_recommended)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(controls_recommended);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(controls_recommended);
        }

        // GET: controls_recommended/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var controls_recommended = await _contextEHS.controls_recommended.FindAsync(id);
            if (controls_recommended == null)
                return NotFound();

            return View(controls_recommended);
        }

        // POST: controls_recommended/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] controls_recommended controls_recommended)
        {
            if (id != controls_recommended.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    controls_recommended.modified_user = employee.onpremisessamaccountname;
                    controls_recommended.modified_user_fullname = employee.displayname;
                    controls_recommended.modified_user_email = employee.mail;
                    controls_recommended.modified_date = DateTime.Now;
                    _contextEHS.Update(controls_recommended);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!controls_recommendedExists(controls_recommended.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(controls_recommended);
        }

        // GET: controls_recommended/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var controls_recommended = await _contextEHS.controls_recommended.FirstOrDefaultAsync(m => m.id == id);
            if (controls_recommended == null)
                return NotFound();

            return View(controls_recommended);
        }

        // POST: controls_recommended/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var controls_recommended = await _contextEHS.controls_recommended.FindAsync(id);
            if (controls_recommended == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            controls_recommended.deleted_user = _username;
            controls_recommended.deleted_user_fullname = employee.displayname;
            controls_recommended.deleted_user_email = employee.mail;
            controls_recommended.deleted_date = DateTime.Now;
            _contextEHS.Update(controls_recommended);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool controls_recommendedExists(int id)
        {
            return _contextEHS.controls_recommended.Any(e => e.id == id);
        }
    }
}
