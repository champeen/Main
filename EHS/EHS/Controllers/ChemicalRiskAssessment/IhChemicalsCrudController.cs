using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHS.Data;
using EHS.Models.IH;

namespace EHS.Controllers.ChemicalRiskAssessment
{
    // Lightweight row used by Index to avoid loading entire navigation collections
    public class IhChemicalListRow
    {
        public int Id { get; set; }
        public string CasNumber { get; set; } = string.Empty;
        public string? PreferredName { get; set; }
        public int PropertyCount { get; set; }
        public int SynonymCount { get; set; }
        public int HazardCount { get; set; }
        public int OelCount { get; set; }
        public int SamplingCount { get; set; }
    }

    public class IhChemicalsCrudController : Controller
    {
        private readonly EHSContext _context;

        public IhChemicalsCrudController(EHSContext context)
        {
            _context = context;
        }

        // GET: IhChemicalsCrud
        public async Task<IActionResult> Index()
        {
            // Project to counts in SQL (LEFT JOIN/COUNT) — null-safe and efficient
            var rows = await _context.ih_chemical
                .Select(c => new IhChemicalListRow
                {
                    Id = c.Id,
                    CasNumber = c.CasNumber,
                    PreferredName = c.PreferredName,

                    // Exclude all "PPE (NPG):*" except keep exactly "PPE (NPG): Flat"
                    PropertyCount = c.Properties.Count(p =>
                        !(EF.Functions.ILike(p.Key, "PPE (NPG):%") && !EF.Functions.ILike(p.Key, "PPE (NPG): Flat"))
                    ),

                    SynonymCount = c.Synonyms.Count(),
                    HazardCount = c.Hazards.Count(),
                    OelCount = c.OELs.Count(),
                    SamplingCount = c.SamplingMethods.Count()
                })
                .OrderBy(r => r.CasNumber)
                .ToListAsync();

            return View(rows);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Base entity (lightweight)
            var chemical = await _context.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chemical == null)
                return NotFound();

            // Load related collections separately (parallel async pattern)
            chemical.Properties = await _context.ih_chemical_property
                .AsNoTracking()
                .Where(p => p.IhChemicalId == id)
                .OrderBy(p => p.Key)
                .ToListAsync();

            chemical.Synonyms = await _context.ih_chemical_synonym
                .AsNoTracking()
                .Where(s => s.IhChemicalId == id)
                .OrderBy(s => s.Synonym)
                .ToListAsync();

            chemical.Hazards = await _context.ih_chemical_hazard
                .AsNoTracking()
                .Where(h => h.IhChemicalId == id)
                .OrderBy(h => h.Source)
                .ToListAsync();

            chemical.OELs = await _context.ih_chemical_oel
                .AsNoTracking()
                .Where(o => o.IhChemicalId == id)
                .OrderBy(o => o.Source)
                .ToListAsync();

            chemical.SamplingMethods = await _context.ih_chemical_sampling_method
                .AsNoTracking()
                .Where(m => m.IhChemicalId == id)
                .OrderBy(m => m.Source)
                .ToListAsync();

            return View(chemical);
        }



        // GET: IhChemicalsCrud/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IhChemicalsCrud/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CasNumber,PubChemCid,PreferredName")] IhChemical ihChemical)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ihChemical);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ihChemical);
        }

        // GET: IhChemicalsCrud/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ihChemical = await _context.ih_chemical.FindAsync(id);
            if (ihChemical == null) return NotFound();

            return View(ihChemical);
        }

        // POST: IhChemicalsCrud/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CasNumber,PreferredName")] IhChemical form)
        {
            if (id != form.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                // Re-hydrate immutable fields for redisplay while keeping typed PreferredName
                var current = await _context.ih_chemical.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (current == null) return NotFound();
                current.PreferredName = form.PreferredName; // preserve user input
                return View(current);
            }

            var entity = await _context.ih_chemical.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) return NotFound();

            entity.PreferredName = string.IsNullOrWhiteSpace(form.PreferredName)
                ? null
                : form.PreferredName.Trim();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ih_chemical.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id });
        }


        // GET: IhChemicalsCrud/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            if (id == null)
                return NotFound();

            // Base entity (lightweight)
            var chemical = await _context.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chemical == null)
                return NotFound();

            // Load related collections separately (parallel async pattern)
            chemical.Properties = await _context.ih_chemical_property
                .AsNoTracking()
                .Where(p => p.IhChemicalId == id)
                .OrderBy(p => p.Key)
                .ToListAsync();

            chemical.Synonyms = await _context.ih_chemical_synonym
                .AsNoTracking()
                .Where(s => s.IhChemicalId == id)
                .OrderBy(s => s.Synonym)
                .ToListAsync();

            chemical.Hazards = await _context.ih_chemical_hazard
                .AsNoTracking()
                .Where(h => h.IhChemicalId == id)
                .OrderBy(h => h.Source)
                .ToListAsync();

            chemical.OELs = await _context.ih_chemical_oel
                .AsNoTracking()
                .Where(o => o.IhChemicalId == id)
                .OrderBy(o => o.Source)
                .ToListAsync();

            chemical.SamplingMethods = await _context.ih_chemical_sampling_method
                .AsNoTracking()
                .Where(m => m.IhChemicalId == id)
                .OrderBy(m => m.Source)
                .ToListAsync();

            return View(chemical);
        }

        // POST: IhChemicalsCrud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ihChemical = await _context.ih_chemical.FindAsync(id);
            if (ihChemical != null)
            {
                _context.ih_chemical.Remove(ihChemical);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
