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

namespace EHS.Controllers.ChemicalRiskAssessment
{
    public class chemical_compositionController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public chemical_compositionController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: chemical_composition
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.chemical_composition.Where(m => m.deleted_date == null).OrderBy(m => m.chemical_name).ToListAsync());
        }

        // GET: chemical_composition/Details/5
        public async Task<IActionResult> Details(int? chemicalId)
        {
            if (chemicalId == null)
                return NotFound();

            var chemical_composition = await _contextEHS.chemical_composition.FirstOrDefaultAsync(m => m.id == chemicalId);
            if (chemical_composition == null)
                return NotFound();

            return View(chemical_composition);
        }

        // GET: chemical_composition/Create
        public async Task<IActionResult> Create(int assessmentId)
        {
            chemical_composition chemical_composition = new chemical_composition();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            chemical_composition.chemical_risk_assessment_id = assessmentId;
            chemical_composition.created_user = employee.onpremisessamaccountname;
            chemical_composition.created_user_fullname = employee.displayname;
            chemical_composition.created_user_email = employee.mail;
            chemical_composition.created_date = DateTime.Now;

            ViewBag.Agents = GetAgentByExposureTypeList("Chemical", null);

            return View(chemical_composition);
        }

        // POST: chemical_composition/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int assessmentId, chemical_composition chemical_composition)
        {
            // User never types chemical_name; we compute it.
            // So ignore the [Required] error for chemical_name from model binding.
            ModelState.Remove(nameof(chemical_composition.chemical_name));

            // Lookup IH chemical by CAS
            var ihChem = await _contextEHS.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CasNumber == chemical_composition.cas_number);

            if (ihChem == null)
            {
                // Validation: CAS chosen but not found in IH chemicals table
                ModelState.AddModelError(nameof(chemical_composition.cas_number),
                    "Selected chemical was not found in IH Chemicals.");
            }
            else
            {
                // Populate the required name field before saving
                chemical_composition.chemical_name = ihChem.PreferredName ?? ihChem.CasNumber;
            }

            if (ModelState.IsValid)
            {
                _contextEHS.Add(chemical_composition);
                await _contextEHS.SaveChangesAsync();

                return RedirectToAction(
                    "Details",
                    "chemical_risk_assessment",
                    new { id = chemical_composition.chemical_risk_assessment_id }
                );
            }

            // Redisplay with dropdown repopulated
            ViewBag.Agents = GetAgentByExposureTypeList("Chemical", null);
            return View(chemical_composition);
        }


        // GET: chemical_composition/Edit/5
        public async Task<IActionResult> Edit(int? chemicalId)
        {
            if (chemicalId == null)
                return NotFound();

            var chemical_composition = await _contextEHS.chemical_composition.FindAsync(chemicalId);
            if (chemical_composition == null)
                return NotFound();

            ViewData["chemical_risk_assessment_id"] = new SelectList(_contextEHS.chemical_risk_assessment, "id", "chemical", chemical_composition.chemical_risk_assessment_id);

            ViewBag.Agents = GetAgentByExposureTypeList("Chemical", null);

            return View(chemical_composition);
        }

        // POST: chemical_composition/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, chemical_composition chemical_composition)
        {
            if (id != chemical_composition.id)
                return NotFound();

            // We compute chemical_name, so ignore its [Required] error from binding
            ModelState.Remove(nameof(chemical_composition.chemical_name));

            // Lookup IH chemical by CAS
            var ihChem = await _contextEHS.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CasNumber == chemical_composition.cas_number);

            if (ihChem == null)
            {
                ModelState.AddModelError(nameof(chemical_composition.cas_number),
                    "Selected chemical was not found in IH Chemicals.");
            }
            else
            {
                // Populate required name before saving
                chemical_composition.chemical_name = ihChem.PreferredName ?? ihChem.CasNumber;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee
                        .Where(m => m.onpremisessamaccountname == _username)
                        .FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    chemical_composition.modified_user = employee.onpremisessamaccountname;
                    chemical_composition.modified_user_fullname = employee.displayname;
                    chemical_composition.modified_user_email = employee.mail;
                    chemical_composition.modified_date = DateTime.Now;

                    _contextEHS.Update(chemical_composition);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!chemical_compositionExists(chemical_composition.id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Details",
                    "chemical_risk_assessment",
                    new { id = chemical_composition.chemical_risk_assessment_id });
            }

            // Rebuild dropdown and redisplay form
            ViewBag.Agents = GetAgentByExposureTypeList("Chemical", null);
            return View(chemical_composition);
        }


        // GET: chemical_composition/Delete/5
        public async Task<IActionResult> Delete(int? chemicalId)
        {
            if (chemicalId == null)
                return NotFound();

            var chemical_composition = await _contextEHS.chemical_composition.Include(c => c.assessment).FirstOrDefaultAsync(m => m.id == chemicalId);
            if (chemical_composition == null)
                return NotFound();

            return View(chemical_composition);
        }

        // POST: chemical_composition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemical_composition = await _contextEHS.chemical_composition.FindAsync(id);
            if (chemical_composition == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            chemical_composition.deleted_user = _username;
            chemical_composition.deleted_user_fullname = employee.displayname;
            chemical_composition.deleted_user_email = employee.mail;
            chemical_composition.deleted_date = DateTime.Now;
            _contextEHS.Update(chemical_composition);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction("Details", "chemical_risk_assessment", new { id = chemical_composition.chemical_risk_assessment_id });
        }

        private bool chemical_compositionExists(int id)
        {
            return _contextEHS.chemical_composition.Any(e => e.id == id);
        }
    }
}
