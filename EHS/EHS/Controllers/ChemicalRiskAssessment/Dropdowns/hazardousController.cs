using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class hazardousController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public hazardousController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: hazardous
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.hazardous.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: hazardous/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hazardous = await _contextEHS.hazardous.FirstOrDefaultAsync(m => m.id == id);
            if (hazardous == null)
                return NotFound();

            return View(hazardous);
        }

        // GET: hazardous/Create
        public async Task<IActionResult> Create()
        {
            hazardous hazardous = new hazardous();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            hazardous.created_user = employee.onpremisessamaccountname;
            hazardous.created_user_fullname = employee.displayname;
            hazardous.created_user_email = employee.mail;
            hazardous.created_date = DateTime.Now;

            return View(hazardous);
        }

        // POST: hazardous/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(hazardous hazardous)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(hazardous);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hazardous);
        }

        // GET: hazardous/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hazardous = await _contextEHS.hazardous.FindAsync(id);
            if (hazardous == null)
                return NotFound();

            return View(hazardous);
        }

        // POST: hazardous/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, hazardous hazardous)
        {
            if (id != hazardous.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    hazardous.modified_user = employee.onpremisessamaccountname;
                    hazardous.modified_user_fullname = employee.displayname;
                    hazardous.modified_user_email = employee.mail;
                    hazardous.modified_date = DateTime.Now;
                    _contextEHS.Update(hazardous);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!hazardousExists(hazardous.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hazardous);
        }

        // GET: hazardous/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hazardous = await _contextEHS.hazardous.FirstOrDefaultAsync(m => m.id == id);
            if (hazardous == null)
                return NotFound();

            return View(hazardous);
        }

        // POST: hazardous/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hazardous = await _contextEHS.hazardous.FindAsync(id);
            if (hazardous == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            hazardous.deleted_user = _username;
            hazardous.deleted_user_fullname = employee.displayname;
            hazardous.deleted_user_email = employee.mail;
            hazardous.deleted_date = DateTime.Now;
            _contextEHS.Update(hazardous);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool hazardousExists(int id)
        {
            return _contextEHS.hazardous.Any(e => e.id == id);
        }
    }
}
