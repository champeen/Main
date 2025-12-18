using EHS.Data;
using EHS.Models;
using EHS.Models;
using EHS.Models.Dropdowns;
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
    public class assessment_methods_usedController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public assessment_methods_usedController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: assessment_methods_used
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.assessment_methods_used.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: assessment_methods_used/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var assessment_methods_used = await _contextEHS.assessment_methods_used
                .FirstOrDefaultAsync(m => m.id == id);
            if (assessment_methods_used == null)
                return NotFound();

            return View(assessment_methods_used);
        }

        // GET: assessment_methods_used/Create
        public async Task<IActionResult> Create()
        {
            assessment_methods_used assessment_methods_used = new assessment_methods_used();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            assessment_methods_used.created_user = employee.onpremisessamaccountname;
            assessment_methods_used.created_user_fullname = employee.displayname;
            assessment_methods_used.created_user_email = employee.mail;
            assessment_methods_used.created_date = DateTime.Now;
            return View(assessment_methods_used);
        }

        // POST: assessment_methods_used/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] assessment_methods_used assessment_methods_used)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(assessment_methods_used);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assessment_methods_used);
        }

        // GET: assessment_methods_used/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var assessment_methods_used = await _contextEHS.assessment_methods_used.FindAsync(id);
            if (assessment_methods_used == null)
                return NotFound();

            return View(assessment_methods_used);
        }

        // POST: assessment_methods_used/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] assessment_methods_used assessment_methods_used)
        {
            if (id != assessment_methods_used.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    assessment_methods_used.modified_user = employee.onpremisessamaccountname;
                    assessment_methods_used.modified_user_fullname = employee.displayname;
                    assessment_methods_used.modified_user_email = employee.mail;
                    assessment_methods_used.modified_date = DateTime.Now;
                    _contextEHS.Update(assessment_methods_used);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!assessment_methods_usedExists(assessment_methods_used.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(assessment_methods_used);
        }

        // GET: assessment_methods_used/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var assessment_methods_used = await _contextEHS.assessment_methods_used.FirstOrDefaultAsync(m => m.id == id);
            if (assessment_methods_used == null)
                return NotFound();

            return View(assessment_methods_used);
        }

        // POST: assessment_methods_used/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment_methods_used = await _contextEHS.assessment_methods_used.FindAsync(id);
            if (assessment_methods_used == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            assessment_methods_used.deleted_user = _username;
            assessment_methods_used.deleted_user_fullname = employee.displayname;
            assessment_methods_used.deleted_user_email = employee.mail;
            assessment_methods_used.deleted_date = DateTime.Now;
            _contextEHS.Update(assessment_methods_used);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool assessment_methods_usedExists(int id)
        {
            return _contextEHS.assessment_methods_used.Any(e => e.id == id);
        }
    }
}
