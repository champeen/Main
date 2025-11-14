using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class ppe_respiratoryController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public ppe_respiratoryController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: ppe_respiratory
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.ppe_respiratory.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: ppe_respiratory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_respiratory = await _contextEHS.ppe_respiratory.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_respiratory == null)
                return NotFound();

            return View(ppe_respiratory);
        }

        // GET: ppe_respiratory/Create
        public async Task<IActionResult> Create()
        {
            ppe_respiratory ppe_respiratory = new ppe_respiratory();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_respiratory.created_user = employee.onpremisessamaccountname;
            ppe_respiratory.created_user_fullname = employee.displayname;
            ppe_respiratory.created_user_email = employee.mail;
            ppe_respiratory.created_date = DateTime.Now;

            return View(ppe_respiratory);
        }

        // POST: ppe_respiratory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ppe_respiratory ppe_respiratory)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(ppe_respiratory);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_respiratory);
        }

        // GET: ppe_respiratory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_respiratory = await _contextEHS.ppe_respiratory.FindAsync(id);
            if (ppe_respiratory == null)
                return NotFound();

            return View(ppe_respiratory);
        }

        // POST: ppe_respiratory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ppe_respiratory ppe_respiratory)
        {
            if (id != ppe_respiratory.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    ppe_respiratory.modified_user = employee.onpremisessamaccountname;
                    ppe_respiratory.modified_user_fullname = employee.displayname;
                    ppe_respiratory.modified_user_email = employee.mail;
                    ppe_respiratory.modified_date = DateTime.Now;
                    _contextEHS.Update(ppe_respiratory);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ppe_respiratoryExists(ppe_respiratory.id))
                         return NotFound();
                     else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_respiratory);
        }

        // GET: ppe_respiratory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_respiratory = await _contextEHS.ppe_respiratory.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_respiratory == null)
                return NotFound();

            return View(ppe_respiratory);
        }

        // POST: ppe_respiratory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ppe_respiratory = await _contextEHS.ppe_respiratory.FindAsync(id);
            if (ppe_respiratory == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_respiratory.deleted_user = _username;
            ppe_respiratory.deleted_user_fullname = employee.displayname;
            ppe_respiratory.deleted_user_email = employee.mail;
            ppe_respiratory.deleted_date = DateTime.Now;
            _contextEHS.Update(ppe_respiratory);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ppe_respiratoryExists(int id)
        {
            return _contextEHS.ppe_respiratory.Any(e => e.id == id);
        }
    }
}
