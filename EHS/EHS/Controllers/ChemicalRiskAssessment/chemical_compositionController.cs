using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EHS.Data;
using EHS.Models;

namespace EHS.Controllers.ChemicalRiskAssessment
{
    public class chemical_compositionController : Controller
    {
        private readonly EHSContext _context;

        public chemical_compositionController(EHSContext context)
        {
            _context = context;
        }

        // GET: chemical_composition
        public async Task<IActionResult> Index()
        {
            var eHSContext = _context.chemical_composition.Include(c => c.assessment);
            return View(await eHSContext.ToListAsync());
        }

        // GET: chemical_composition/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical_composition = await _context.chemical_composition
                .Include(c => c.assessment)
                .FirstOrDefaultAsync(m => m.id == id);
            if (chemical_composition == null)
            {
                return NotFound();
            }

            return View(chemical_composition);
        }

        // GET: chemical_composition/Create
        public IActionResult Create()
        {
            ViewData["chemical_risk_assessment_id"] = new SelectList(_context.chemical_risk_assessment, "id", "chemical");
            return View();
        }

        // POST: chemical_composition/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,chemical_risk_assessment_id,cas_number,chemical_name,concentration_low,concentration_high,created_user,created_user_fullname,created_user_email,created_date,modified_user,modified_user_fullname,modified_user_email,modified_date,deleted_user,deleted_user_fullname,deleted_user_email,deleted_date")] chemical_composition chemical_composition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chemical_composition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["chemical_risk_assessment_id"] = new SelectList(_context.chemical_risk_assessment, "id", "chemical", chemical_composition.chemical_risk_assessment_id);
            return View(chemical_composition);
        }

        // GET: chemical_composition/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical_composition = await _context.chemical_composition.FindAsync(id);
            if (chemical_composition == null)
            {
                return NotFound();
            }
            ViewData["chemical_risk_assessment_id"] = new SelectList(_context.chemical_risk_assessment, "id", "chemical", chemical_composition.chemical_risk_assessment_id);
            return View(chemical_composition);
        }

        // POST: chemical_composition/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,chemical_risk_assessment_id,cas_number,chemical_name,concentration_low,concentration_high,created_user,created_user_fullname,created_user_email,created_date,modified_user,modified_user_fullname,modified_user_email,modified_date,deleted_user,deleted_user_fullname,deleted_user_email,deleted_date")] chemical_composition chemical_composition)
        {
            if (id != chemical_composition.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chemical_composition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!chemical_compositionExists(chemical_composition.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["chemical_risk_assessment_id"] = new SelectList(_context.chemical_risk_assessment, "id", "chemical", chemical_composition.chemical_risk_assessment_id);
            return View(chemical_composition);
        }

        // GET: chemical_composition/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical_composition = await _context.chemical_composition
                .Include(c => c.assessment)
                .FirstOrDefaultAsync(m => m.id == id);
            if (chemical_composition == null)
            {
                return NotFound();
            }

            return View(chemical_composition);
        }

        // POST: chemical_composition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemical_composition = await _context.chemical_composition.FindAsync(id);
            if (chemical_composition != null)
            {
                _context.chemical_composition.Remove(chemical_composition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool chemical_compositionExists(int id)
        {
            return _context.chemical_composition.Any(e => e.id == id);
        }
    }
}
