using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class hazard_codesController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public hazard_codesController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: hazard_codes
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.hazard_codes.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m=>m.code).ThenBy(m => m.description).ToListAsync());
        }

        // GET: hazard_codes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hazard_codes = await _contextEHS.hazard_codes.FirstOrDefaultAsync(m => m.id == id);
            if (hazard_codes == null)
                return NotFound();

            return View(hazard_codes);
        }

        // GET: hazard_codes/Create
        public async Task<IActionResult> Create()
        {
            hazard_codes hazard_codes = new hazard_codes();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            hazard_codes.created_user = employee.onpremisessamaccountname;
            hazard_codes.created_user_fullname = employee.displayname;
            hazard_codes.created_user_email = employee.mail;
            hazard_codes.created_date = DateTime.Now;

            return View(hazard_codes);
        }

        // POST: hazard_codes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(hazard_codes hazard_codes)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(hazard_codes);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hazard_codes);
        }

        // GET: hazard_codes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hazard_codes = await _contextEHS.hazard_codes.FindAsync(id);
            if (hazard_codes == null)
                return NotFound();

            return View(hazard_codes);
        }

        // POST: hazard_codes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, hazard_codes hazard_codes)
        {
            if (id != hazard_codes.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    hazard_codes.modified_user = employee.onpremisessamaccountname;
                    hazard_codes.modified_user_fullname = employee.displayname;
                    hazard_codes.modified_user_email = employee.mail;
                    hazard_codes.modified_date = DateTime.Now;
                    _contextEHS.Update(hazard_codes);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!hazard_codesExists(hazard_codes.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hazard_codes);
        }

        // GET: hazard_codes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hazard_codes = await _contextEHS.hazard_codes.FirstOrDefaultAsync(m => m.id == id);
            if (hazard_codes == null)
                return NotFound();

            return View(hazard_codes);
        }

        // POST: hazard_codes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hazard_codes = await _contextEHS.hazard_codes.FindAsync(id);
            if (hazard_codes == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            hazard_codes.deleted_user = _username;
            hazard_codes.deleted_user_fullname = employee.displayname;
            hazard_codes.deleted_user_email = employee.mail;
            hazard_codes.deleted_date = DateTime.Now;
            _contextEHS.Update(hazard_codes);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool hazard_codesExists(int id)
        {
            return _contextEHS.hazard_codes.Any(e => e.id == id);
        }
    }
}
