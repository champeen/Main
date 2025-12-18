using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class areasController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public areasController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: areas
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.area.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: areas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var area = await _contextEHS.area.FirstOrDefaultAsync(m => m.id == id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // GET: areas/Create
        public async Task<IActionResult> Create()
        {
            area area = new area();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            area.created_user = employee.onpremisessamaccountname;
            area.created_user_fullname = employee.displayname;
            area.created_user_email = employee.mail;
            area.created_date = DateTime.Now;

            return View(area);
        }

        // POST: areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(area area)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(area);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: areas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var area = await _contextEHS.area.FindAsync(id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // POST: areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, area area)
        {
            if (id != area.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    area.modified_user = employee.onpremisessamaccountname;
                    area.modified_user_fullname = employee.displayname;
                    area.modified_user_email = employee.mail;
                    area.modified_date = DateTime.Now;
                    _contextEHS.Update(area);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!areaExists(area.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: areas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var area = await _contextEHS.area.FirstOrDefaultAsync(m => m.id == id);
            if (area == null)
                return NotFound();

            return View(area);
        }

        // POST: areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _contextEHS.area.FindAsync(id);
            if (area == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            area.deleted_user = _username;
            area.deleted_user_fullname = employee.displayname;
            area.deleted_user_email = employee.mail;
            area.deleted_date = DateTime.Now;
            _contextEHS.Update(area);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool areaExists(int id)
        {
            return _contextEHS.area.Any(e => e.id == id);
        }
    }
}
