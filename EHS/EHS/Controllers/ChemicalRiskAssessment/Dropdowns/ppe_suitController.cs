using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class ppe_suitController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public ppe_suitController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: ppe_suit
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.ppe_suit.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: ppe_suit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_suit = await _contextEHS.ppe_suit.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_suit == null)
                return NotFound();

            return View(ppe_suit);
        }

        // GET: ppe_suit/Create
        public async Task<IActionResult> Create()
        {
            ppe_suit ppe_suit = new ppe_suit();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_suit.created_user = employee.onpremisessamaccountname;
            ppe_suit.created_user_fullname = employee.displayname;
            ppe_suit.created_user_email = employee.mail;
            ppe_suit.created_date = DateTime.Now;

            return View(ppe_suit);
        }

        // POST: ppe_suit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ppe_suit ppe_suit)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(ppe_suit);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_suit);
        }

        // GET: ppe_suit/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_suit = await _contextEHS.ppe_suit.FindAsync(id);
            if (ppe_suit == null)
                return NotFound();

            return View(ppe_suit);
        }

        // POST: ppe_suit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ppe_suit ppe_suit)
        {
            if (id != ppe_suit.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    ppe_suit.modified_user = employee.onpremisessamaccountname;
                    ppe_suit.modified_user_fullname = employee.displayname;
                    ppe_suit.modified_user_email = employee.mail;
                    ppe_suit.modified_date = DateTime.Now;
                    _contextEHS.Update(ppe_suit);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ppe_suitExists(ppe_suit.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ppe_suit);
        }

        // GET: ppe_suit/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ppe_suit = await _contextEHS.ppe_suit.FirstOrDefaultAsync(m => m.id == id);
            if (ppe_suit == null)
                return NotFound();

            return View(ppe_suit);
        }

        // POST: ppe_suit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ppe_suit = await _contextEHS.ppe_suit.FindAsync(id);
            if (ppe_suit == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            ppe_suit.deleted_user = _username;
            ppe_suit.deleted_user_fullname = employee.displayname;
            ppe_suit.deleted_user_email = employee.mail;
            ppe_suit.deleted_date = DateTime.Now;
            _contextEHS.Update(ppe_suit);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ppe_suitExists(int id)
        {
            return _contextEHS.ppe_suit.Any(e => e.id == id);
        }
    }
}
