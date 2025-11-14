using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class physical_stateController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public physical_stateController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: physical_state
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.physical_state.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: physical_state/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var physical_state = await _contextEHS.physical_state.FirstOrDefaultAsync(m => m.id == id);
            if (physical_state == null)
                return NotFound();

            return View(physical_state);
        }

        // GET: physical_state/Create
        public async Task<IActionResult> Create()
        {
            physical_state physical_state = new physical_state();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            physical_state.created_user = employee.onpremisessamaccountname;
            physical_state.created_user_fullname = employee.displayname;
            physical_state.created_user_email = employee.mail;
            physical_state.created_date = DateTime.Now;

            return View(physical_state);
        }

        // POST: physical_state/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(physical_state physical_state)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(physical_state);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(physical_state);
        }

        // GET: physical_state/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var physical_state = await _contextEHS.physical_state.FindAsync(id);
            if (physical_state == null)
                return NotFound();

            return View(physical_state);
        }

        // POST: physical_state/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, physical_state physical_state)
        {
            if (id != physical_state.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    physical_state.modified_user = employee.onpremisessamaccountname;
                    physical_state.modified_user_fullname = employee.displayname;
                    physical_state.modified_user_email = employee.mail;
                    physical_state.modified_date = DateTime.Now;
                    _contextEHS.Update(physical_state);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!physical_stateExists(physical_state.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(physical_state);
        }

        // GET: physical_state/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                 return NotFound();

            var physical_state = await _contextEHS.physical_state.FirstOrDefaultAsync(m => m.id == id);
            if (physical_state == null)
                return NotFound();

            return View(physical_state);
        }

        // POST: physical_state/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physical_state = await _contextEHS.physical_state.FindAsync(id);
            if (physical_state == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            physical_state.deleted_user = _username;
            physical_state.deleted_user_fullname = employee.displayname;
            physical_state.deleted_user_email = employee.mail;
            physical_state.deleted_date = DateTime.Now;
            _contextEHS.Update(physical_state);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool physical_stateExists(int id)
        {
            return _contextEHS.physical_state.Any(e => e.id == id);
        }
    }
}
