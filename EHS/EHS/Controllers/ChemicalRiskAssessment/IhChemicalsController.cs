using EHS.Data;
using EHS.Services.Chemicals;
using EHS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment
{
    public class IhChemicalsController : Controller
    {
        private readonly EHSContext _db;
        private readonly ChemicalIngestService _svc;

        public IhChemicalsController(EHSContext db, ChemicalIngestService svc)
        { _db = db; _svc = svc; }

        // GET /IhChemicals?cas=50-00-0   (optional "cas")
        [HttpGet]
        public async Task<IActionResult> Index(string? cas, CancellationToken ct = default)
        {
            var vm = new IhChemicalsIndexViewModel
            {
                Recent = await _svc.GetRecentAsync(25, ct)
            };

            if (string.IsNullOrWhiteSpace(cas))
                return View(vm);

            cas = cas.Trim();

            // 1) Try DB first so the page reflects exactly what was saved
            var dtoFromDb = await _svc.BuildDtoFromDbAsync(cas);
            if (dtoFromDb != null)
            {
                vm.Result = dtoFromDb;

                var existing = await _db.ih_chemical
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CasNumber == cas, ct);

                vm.ExistsAlready = existing != null;
                vm.ExistingId = existing?.Id;
                // vm.ExistingUpdatedAt = existing?.UpdatedAt; // if you add this col later

                return View(vm);
            }

            // 2) Fallback to live fetch if not in DB
            var (dto, unavailable) = await _svc.FetchByCasWithStatusAsync(cas, ct);
            vm.Result = dto;
            vm.UnavailableSources = unavailable?.Distinct().ToList();

            // Even if we fetched live, check whether the DB already has a record for this CAS
            var existingLive = await _db.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CasNumber == dto.CasNumber, ct);

            if (existingLive != null)
            {
                vm.ExistsAlready = true;
                vm.ExistingId = existingLive.Id;
                // vm.ExistingUpdatedAt = existingLive.UpdatedAt;
            }

            return View(vm);
        }

        // FETCH ONLY (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IhChemicalsIndexViewModel vm, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(vm.CasInput))
                ModelState.AddModelError("CasInput", "CAS number is required.");

            // Keep the “recent” list consistent by using the service here too
            vm.Recent = await _svc.GetRecentAsync(25, ct);

            if (!ModelState.IsValid) return View(vm);

            var cas = vm.CasInput.Trim();
            var (dto, unavailable) = await _svc.FetchByCasWithStatusAsync(cas, ct);

            vm.Result = dto;
            vm.UnavailableSources = unavailable?.Distinct().ToList();

            // Flag existence so the view can warn and the Save click will show the modal
            var existing = await _db.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CasNumber == dto.CasNumber, ct);

            vm.ExistsAlready = existing != null;
            vm.ExistingId = existing?.Id;
            // vm.ExistingUpdatedAt = existing?.UpdatedAt;

            return View(vm);
        }

        // SAVE (with overwrite guard)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string cas, bool OverwriteConfirmed = false, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cas))
            {
                TempData["Toast"] = "No CAS provided.";
                return RedirectToAction(nameof(Index));
            }

            cas = cas.Trim();

            // Does a record already exist?
            var existing = await _db.ih_chemical
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CasNumber == cas, ct);

            var existsAlready = existing != null;

            if (existsAlready && !OverwriteConfirmed)
            {
                // Build the page back with fetched preview + force the overwrite modal to open
                var vm = new IhChemicalsIndexViewModel
                {
                    Recent = await _svc.GetRecentAsync(25, ct),
                    OpenOverwriteModal = true,
                    ExistsAlready = true,
                    ExistingId = existing?.Id,
                    // ExistingUpdatedAt = existing?.UpdatedAt
                };

                var (dtoPreview, unavailable) = await _svc.FetchByCasWithStatusAsync(cas, ct);
                vm.Result = dtoPreview;
                vm.UnavailableSources = unavailable?.Distinct().ToList();

                // Optional: model-level message (shows if you render validation summary)
                ModelState.AddModelError(string.Empty, "A record for this CAS already exists. Confirm overwrite to continue.");

                return View("Index", vm);
            }

            // Proceed to fetch current data and upsert (overwrite if exists)
            var (dto, _) = await _svc.FetchByCasWithStatusAsync(cas, ct);

            // Overwrite or insert via your service
            await _svc.UpsertFromDtoAsync(dto, CancellationToken.None);

            TempData["Toast"] = existsAlready
                ? $"Updated existing: {dto.PreferredName ?? dto.CasNumber}"
                : $"Saved: {dto.PreferredName ?? dto.CasNumber}";

            // **NEW**: mark that we just saved so the view shows a green success banner instead of the yellow warning
            TempData["JustSaved"] = "true";

            // After save, land on GET /Index?cas=... so the page shows what's stored
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

            var vm = new IhChemicalsIndexViewModel
            {
                Result = dto,
                ExistsAlready = true,
                ExistingId = chem.Id
                // ExistingUpdatedAt = chem.UpdatedAt
            };

            return View("Index", vm);
        }
    }
}
