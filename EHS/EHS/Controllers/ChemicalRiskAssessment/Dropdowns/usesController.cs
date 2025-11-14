using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class usesController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public usesController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: uses
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.use.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: uses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var use = await _contextEHS.use.FirstOrDefaultAsync(m => m.id == id);
            if (use == null)
                return NotFound();

            return View(use);
        }

        // GET: uses/Create
        public async Task<IActionResult> Create()
        {
            use use = new use();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            use.created_user = employee.onpremisessamaccountname;
            use.created_user_fullname = employee.displayname;
            use.created_user_email = employee.mail;
            use.created_date = DateTime.Now;

            return View(use);
        }

        // POST: uses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(use use)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(use);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(use);
        }

        // GET: uses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var use = await _contextEHS.use.FindAsync(id);
            if (use == null)
                return NotFound();

            return View(use);
        }

        // POST: uses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, use use)
        {
            if (id != use.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    use.modified_user = employee.onpremisessamaccountname;
                    use.modified_user_fullname = employee.displayname;
                    use.modified_user_email = employee.mail;
                    use.modified_date = DateTime.Now;
                    _contextEHS.Update(use);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!useExists(use.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(use);
        }

        // GET: uses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var use = await _contextEHS.use.FirstOrDefaultAsync(m => m.id == id);
            if (use == null)
                 return NotFound();

            return View(use);
        }

        // POST: uses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var use = await _contextEHS.use.FindAsync(id);
            if (use == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            use.deleted_user = _username;
            use.deleted_user_fullname = employee.displayname;
            use.deleted_user_email = employee.mail;
            use.deleted_date = DateTime.Now;
            _contextEHS.Update(use);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool useExists(int id)
        {
            return _contextEHS.use.Any(e => e.id == id);
        }
    }
}
