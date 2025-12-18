using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.ChemicalRiskAssessment;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers.ChemicalRiskAssessment.Dropdowns
{
    public class risk_eye_contactController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public risk_eye_contactController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: risk_eye_contact
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.risk_eye_contact.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: risk_eye_contact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_eye_contact = await _contextEHS.risk_eye_contact.FirstOrDefaultAsync(m => m.id == id);
            if (risk_eye_contact == null)
                return NotFound();

            return View(risk_eye_contact);
        }

        // GET: risk_eye_contact/Create
        public async Task<IActionResult> Create()
        {
            risk_eye_contact risk_eye_contact = new risk_eye_contact();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_eye_contact.created_user = employee.onpremisessamaccountname;
            risk_eye_contact.created_user_fullname = employee.displayname;
            risk_eye_contact.created_user_email = employee.mail;
            risk_eye_contact.created_date = DateTime.Now;

            return View(risk_eye_contact);
        }

        // POST: risk_eye_contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(risk_eye_contact risk_eye_contact)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(risk_eye_contact);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(risk_eye_contact);
        }

        // GET: risk_eye_contact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                 return NotFound();

            var risk_eye_contact = await _contextEHS.risk_eye_contact.FindAsync(id);
            if (risk_eye_contact == null)
                return NotFound();

            return View(risk_eye_contact);
        }

        // POST: risk_eye_contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, risk_eye_contact risk_eye_contact)
        {
            if (id != risk_eye_contact.id)
                  return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    risk_eye_contact.modified_user = employee.onpremisessamaccountname;
                    risk_eye_contact.modified_user_fullname = employee.displayname;
                    risk_eye_contact.modified_user_email = employee.mail;
                    risk_eye_contact.modified_date = DateTime.Now;
                    _contextEHS.Update(risk_eye_contact);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!risk_eye_contactExists(risk_eye_contact.id))
                         return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(risk_eye_contact);
        }

        // GET: risk_eye_contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var risk_eye_contact = await _contextEHS.risk_eye_contact.FirstOrDefaultAsync(m => m.id == id);
            if (risk_eye_contact == null)
                return NotFound();

            return View(risk_eye_contact);
        }

        // POST: risk_eye_contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var risk_eye_contact = await _contextEHS.risk_eye_contact.FindAsync(id);
            if (risk_eye_contact == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            risk_eye_contact.deleted_user = _username;
            risk_eye_contact.deleted_user_fullname = employee.displayname;
            risk_eye_contact.deleted_user_email = employee.mail;
            risk_eye_contact.deleted_date = DateTime.Now;
            _contextEHS.Update(risk_eye_contact);
            await _contextEHS.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool risk_eye_contactExists(int id)
        {
            return _contextEHS.risk_eye_contact.Any(e => e.id == id);
        }
    }
}
