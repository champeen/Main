using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class risk_inhalationController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public risk_inhalationController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: risk_inhalation
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.risk_inhalation.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: risk_inhalation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_inhalation = await _contextEHS.risk_inhalation.FirstOrDefaultAsync(m => m.id == id);
            if (risk_inhalation == null)
                return NotFound();

            return View(risk_inhalation);
        }

        // GET: risk_inhalation/Create
        public async Task<IActionResult> Create()
        {
            risk_inhalation risk_inhalation = new risk_inhalation();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_inhalation.created_user = employee.onpremisessamaccountname;
            risk_inhalation.created_user_fullname = employee.displayname;
            risk_inhalation.created_user_email = employee.mail;
            risk_inhalation.created_date = DateTime.Now;

            return View(risk_inhalation);
        }

        // POST: risk_inhalation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(risk_inhalation risk_inhalation)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(risk_inhalation);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(risk_inhalation);
        }

        // GET: risk_inhalation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_inhalation = await _contextEHS.risk_inhalation.FindAsync(id);
            if (risk_inhalation == null)
                 return NotFound();

            return View(risk_inhalation);
        }

        // POST: risk_inhalation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, risk_inhalation risk_inhalation)
        {
            if (id != risk_inhalation.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    risk_inhalation.modified_user = employee.onpremisessamaccountname;
                    risk_inhalation.modified_user_fullname = employee.displayname;
                    risk_inhalation.modified_user_email = employee.mail;
                    risk_inhalation.modified_date = DateTime.Now;
                    _contextEHS.Update(risk_inhalation);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!risk_inhalationExists(risk_inhalation.id))
                         return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(risk_inhalation);
        }

        // GET: risk_inhalation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_inhalation = await _contextEHS.risk_inhalation.FirstOrDefaultAsync(m => m.id == id);
            if (risk_inhalation == null)
                return NotFound();

            return View(risk_inhalation);
        }

        // POST: risk_inhalation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var risk_inhalation = await _contextEHS.risk_inhalation.FindAsync(id);
            if (risk_inhalation == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_inhalation.deleted_user = _username;
            risk_inhalation.deleted_user_fullname = employee.displayname;
            risk_inhalation.deleted_user_email = employee.mail;
            risk_inhalation.deleted_date = DateTime.Now;
            _contextEHS.Update(risk_inhalation);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool risk_inhalationExists(int id)
        {
            return _contextEHS.risk_inhalation.Any(e => e.id == id);
        }
    }
}
