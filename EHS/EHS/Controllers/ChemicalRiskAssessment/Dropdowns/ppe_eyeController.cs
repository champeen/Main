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
    public class ppe_eyeController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public ppe_eyeController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: ppe_eye
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.ppe_eye.ToListAsync());
        }

        // GET: ppe_eye/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_eye = await _contextEHS.ppe_eye.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).FirstOrDefaultAsync(m => m.id == id);
            if (ppe_eye == null)
                return NotFound();

            return View(ppe_eye);
        }

        // GET: ppe_eye/Create
        public async Task<IActionResult> Create()
        {
            ppe_eye ppe_eye = new ppe_eye();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_eye.created_user = employee.onpremisessamaccountname;
            ppe_eye.created_user_fullname = employee.displayname;
            ppe_eye.created_user_email = employee.mail;
            ppe_eye.created_date = DateTime.Now;

            return View(ppe_eye);
        }

        // POST: ppe_eye/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ppe_eye ppe_eye)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(ppe_eye);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_eye);
        }

        // GET: ppe_eye/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_eye = await _contextEHS.ppe_eye.FindAsync(id);
            if (ppe_eye == null)
                 return NotFound();

            return View(ppe_eye);
        }

        // POST: ppe_eye/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ppe_eye ppe_eye)
        {
            if (id != ppe_eye.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    ppe_eye.modified_user = employee.onpremisessamaccountname;
                    ppe_eye.modified_user_fullname = employee.displayname;
                    ppe_eye.modified_user_email = employee.mail;
                    ppe_eye.modified_date = DateTime.Now;
                    _contextEHS.Update(ppe_eye);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ppe_eyeExists(ppe_eye.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_eye);
        }

        // GET: ppe_eye/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_eye = await _contextEHS.ppe_eye.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_eye == null)
                return NotFound();

            return View(ppe_eye);
        }

        // POST: ppe_eye/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ppe_eye = await _contextEHS.ppe_eye.FindAsync(id);
            if (ppe_eye == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_eye.deleted_user = _username;
            ppe_eye.deleted_user_fullname = employee.displayname;
            ppe_eye.deleted_user_email = employee.mail;
            ppe_eye.deleted_date = DateTime.Now;
            _contextEHS.Update(ppe_eye);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ppe_eyeExists(int id)
        {
            return _contextEHS.ppe_eye.Any(e => e.id == id);
        }
    }
}
