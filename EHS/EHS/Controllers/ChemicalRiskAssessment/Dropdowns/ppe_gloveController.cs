using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class ppe_gloveController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public ppe_gloveController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: ppe_glove
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.ppe_glove.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: ppe_glove/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_glove = await _contextEHS.ppe_glove.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_glove == null)
                return NotFound();

            return View(ppe_glove);
        }

        // GET: ppe_glove/Create
        public async Task<IActionResult> Create()
        {
            ppe_glove ppe_glove = new ppe_glove();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_glove.created_user = employee.onpremisessamaccountname;
            ppe_glove.created_user_fullname = employee.displayname;
            ppe_glove.created_user_email = employee.mail;
            ppe_glove.created_date = DateTime.Now;

            return View(ppe_glove);
        }

        // POST: ppe_glove/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ppe_glove ppe_glove)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(ppe_glove);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_glove);
        }

        // GET: ppe_glove/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_glove = await _contextEHS.ppe_glove.FindAsync(id);
            if (ppe_glove == null)
                return NotFound();

            return View(ppe_glove);
        }

        // POST: ppe_glove/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ppe_glove ppe_glove)
        {
            if (id != ppe_glove.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    ppe_glove.modified_user = employee.onpremisessamaccountname;
                    ppe_glove.modified_user_fullname = employee.displayname;
                    ppe_glove.modified_user_email = employee.mail;
                    ppe_glove.modified_date = DateTime.Now;
                    _contextEHS.Update(ppe_glove);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ppe_gloveExists(ppe_glove.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_glove);
        }

        // GET: ppe_glove/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_glove = await _contextEHS.ppe_glove.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_glove == null)
                return NotFound();

            return View(ppe_glove);
        }

        // POST: ppe_glove/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ppe_glove = await _contextEHS.ppe_glove.FindAsync(id);
            if (ppe_glove == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_glove.deleted_user = _username;
            ppe_glove.deleted_user_fullname = employee.displayname;
            ppe_glove.deleted_user_email = employee.mail;
            ppe_glove.deleted_date = DateTime.Now;
            _contextEHS.Update(ppe_glove);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ppe_gloveExists(int id)
        {
            return _contextEHS.ppe_glove.Any(e => e.id == id);
        }
    }
}
