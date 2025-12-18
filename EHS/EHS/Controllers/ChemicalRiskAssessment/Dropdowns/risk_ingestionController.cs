using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class risk_ingestionController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public risk_ingestionController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: risk_ingestion
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.risk_ingestion.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: risk_ingestion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_ingestion = await _contextEHS.risk_ingestion.FirstOrDefaultAsync(m => m.id == id);
            if (risk_ingestion == null)
                return NotFound();

            return View(risk_ingestion);
        }

        // GET: risk_ingestion/Create
        public async Task<IActionResult> Create()
        {
            risk_ingestion risk_ingestion = new risk_ingestion();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_ingestion.created_user = employee.onpremisessamaccountname;
            risk_ingestion.created_user_fullname = employee.displayname;
            risk_ingestion.created_user_email = employee.mail;
            risk_ingestion.created_date = DateTime.Now;

            return View(risk_ingestion);
        }

        // POST: risk_ingestion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(risk_ingestion risk_ingestion)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(risk_ingestion);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(risk_ingestion);
        }

        // GET: risk_ingestion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_ingestion = await _contextEHS.risk_ingestion.FindAsync(id);
            if (risk_ingestion == null)
                return NotFound();

            return View(risk_ingestion);
        }

        // POST: risk_ingestion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, risk_ingestion risk_ingestion)
        {
            if (id != risk_ingestion.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    risk_ingestion.modified_user = employee.onpremisessamaccountname;
                    risk_ingestion.modified_user_fullname = employee.displayname;
                    risk_ingestion.modified_user_email = employee.mail;
                    risk_ingestion.modified_date = DateTime.Now;
                    _contextEHS.Update(risk_ingestion);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!risk_ingestionExists(risk_ingestion.id))
                          return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(risk_ingestion);
        }

        // GET: risk_ingestion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_ingestion = await _contextEHS.risk_ingestion.FirstOrDefaultAsync(m => m.id == id);
            if (risk_ingestion == null)
                  return NotFound();

            return View(risk_ingestion);
        }

        // POST: risk_ingestion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var risk_ingestion = await _contextEHS.risk_ingestion.FindAsync(id);
            if (risk_ingestion == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_ingestion.deleted_user = _username;
            risk_ingestion.deleted_user_fullname = employee.displayname;
            risk_ingestion.deleted_user_email = employee.mail;
            risk_ingestion.deleted_date = DateTime.Now;
            _contextEHS.Update(risk_ingestion);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool risk_ingestionExists(int id)
        {
            return _contextEHS.risk_ingestion.Any(e => e.id == id);
        }
    }
}
