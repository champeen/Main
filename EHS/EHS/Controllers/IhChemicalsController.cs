using EHS.Data;
using EHS.Services.Chemicals;
using EHS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers
{
    public class IhChemicalsController : Controller
    {
        private readonly EHSContext _db;
        private readonly ChemicalIngestService _svc;
        public IhChemicalsController(EHSContext db, ChemicalIngestService svc)
        { _db = db; _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new IhChemicalsIndexViewModel
            {
                Recent = await _db.ih_chemical
                    .OrderByDescending(c => c.Id)
                    .Select(c => new IhChemHistoryRow
                    {
                        Id = c.Id,
                        CasNumber = c.CasNumber,
                        PreferredName = c.PreferredName,
                        PubChemCid = c.PubChemCid,
                        SynonymCount = c.Synonyms.Count,
                        PropertyCount = c.Properties.Count,
                        HazardCount = c.Hazards.Count,
                        OelCount = c.OELs.Count,
                        SamplingCount = c.SamplingMethods.Count
                    })
                    .Take(25)
                    .ToListAsync()
            };
            return View(vm);
        }

        // FETCH ONLY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IhChemicalsIndexViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.CasInput))
                ModelState.AddModelError("CasInput", "CAS number is required.");

            vm.Recent = await _db.ih_chemical
                .OrderByDescending(c => c.Id)
                .Select(c => new IhChemHistoryRow
                {
                    Id = c.Id,
                    CasNumber = c.CasNumber,
                    PreferredName = c.PreferredName,
                    PubChemCid = c.PubChemCid,
                    SynonymCount = c.Synonyms.Count,
                    PropertyCount = c.Properties.Count,
                    HazardCount = c.Hazards.Count,
                    OelCount = c.OELs.Count,
                    SamplingCount = c.SamplingMethods.Count
                })
                .Take(25)
                .ToListAsync();

            if (!ModelState.IsValid) return View(vm);

            var (dto, unavailable) = await _svc.FetchByCasWithStatusAsync(vm.CasInput.Trim());
            vm.Result = dto;
            vm.UnavailableSources = unavailable.Distinct().ToList();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? cas, CancellationToken ct = default)
        {
            var vm = new EHS.ViewModels.IhChemicalsIndexViewModel
            {
                Recent = await _svc.GetRecentAsync(25, ct)
            };

            if (!string.IsNullOrWhiteSpace(cas))
            {
                // 1) Try DB first (verifies what we saved)
                var dtoFromDb = await _svc.BuildDtoFromDbAsync(cas, ct);
                if (dtoFromDb != null)
                {
                    vm.Result = dtoFromDb;
                    // optional: vm.UnavailableSources = new List<string> { "Loaded from database." };
                    return View(vm);
                }

                // 2) Fallback: live fetch if not in DB yet
                var (dto, unavailable) = await _svc.FetchByCasWithStatusAsync(cas, ct);
                vm.Result = dto;
                vm.UnavailableSources = unavailable;
            }

            return View(vm);
        }

        // SAVE after reviewing fetched data
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Save(string cas)
        //{
        //    if (string.IsNullOrWhiteSpace(cas))
        //    {
        //        TempData["Toast"] = "No CAS provided to save.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var dto = await _svc.IngestByCasAsync(cas.Trim());
        //    // Look up saved record id for redirect
        //    var id = await _db.ih_chemical
        //        .Where(c => c.CasNumber == dto.CasNumber)
        //        .Select(c => c.Id)
        //        .FirstOrDefaultAsync();

        //    TempData["Toast"] = $"Saved data for CAS {dto.CasNumber}.";
        //    if (id == 0) return RedirectToAction(nameof(Index));
        //    return RedirectToAction(nameof(Details), new { id });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string cas, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cas))
            {
                TempData["Toast"] = "No CAS provided.";
                return RedirectToAction(nameof(Index));
            }

            // Use the same live fetch pipeline you already have
            var (dto, _) = await _svc.FetchByCasWithStatusAsync(cas, ct);

            // Persist everything you care about
            await _svc.UpsertFromDtoAsync(dto, ct);

            TempData["Toast"] = $"Saved: {dto.PreferredName ?? dto.CasNumber}";
            // After save, show what’s in the DB:
            return RedirectToAction(nameof(Index), new { cas = dto.CasNumber });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var chem = await _db.ih_chemical
                .Include(c => c.Synonyms)
                .Include(c => c.Properties)
                .Include(c => c.Hazards)
                .Include(c => c.OELs)
                .Include(c => c.SamplingMethods)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (chem == null) return NotFound();

            var dto = new ChemicalCoreDto(
                chem.CasNumber,
                chem.PreferredName,
                chem.PubChemCid,
                chem.Synonyms.Select(s => s.Synonym).ToArray(),
                chem.Properties.ToDictionary(p => p.Key, p => p.Value),
                chem.Hazards.ToList(),
                chem.OELs.ToList(),
                chem.SamplingMethods.ToList()
            );

            var vm = new IhChemicalsIndexViewModel { Result = dto };
            return View("Index", vm);
        }
    }
}