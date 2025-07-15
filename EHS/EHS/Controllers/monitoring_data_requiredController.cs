using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers
{
    public class monitoring_data_requiredController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public monitoring_data_requiredController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: monitoring_data_required
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.monitoring_data_required.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: monitoring_data_required/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var monitoring_data_required = await _contextEHS.monitoring_data_required.FirstOrDefaultAsync(m => m.id == id);
            if (monitoring_data_required == null)
                return NotFound();

            return View(monitoring_data_required);
        }

        // GET: monitoring_data_required/Create
        public async Task<IActionResult> Create()
        {
            monitoring_data_required monitoring_data_required = new monitoring_data_required();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            monitoring_data_required.created_user = employee.onpremisessamaccountname;
            monitoring_data_required.created_user_fullname = employee.displayname;
            monitoring_data_required.created_user_email = employee.mail;
            monitoring_data_required.created_date = DateTime.Now;
            return View(monitoring_data_required);
        }

        // POST: monitoring_data_required/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] monitoring_data_required monitoring_data_required)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(monitoring_data_required);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monitoring_data_required);
        }

        // GET: monitoring_data_required/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var monitoring_data_required = await _contextEHS.monitoring_data_required.FindAsync(id);
            if (monitoring_data_required == null)
                return NotFound();

            return View(monitoring_data_required);
        }

        // POST: monitoring_data_required/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] monitoring_data_required monitoring_data_required)
        {
            if (id != monitoring_data_required.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    monitoring_data_required.modified_user = employee.onpremisessamaccountname;
                    monitoring_data_required.modified_user_fullname = employee.displayname;
                    monitoring_data_required.modified_user_email = employee.mail;
                    monitoring_data_required.modified_date = DateTime.Now;
                    _contextEHS.Update(monitoring_data_required);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!monitoring_data_requiredExists(monitoring_data_required.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monitoring_data_required);
        }

        // GET: monitoring_data_required/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var monitoring_data_required = await _contextEHS.monitoring_data_required.FirstOrDefaultAsync(m => m.id == id);
            if (monitoring_data_required == null)
                return NotFound();

            return View(monitoring_data_required);
        }

        // POST: monitoring_data_required/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monitoring_data_required = await _contextEHS.monitoring_data_required.FindAsync(id);
            if (monitoring_data_required == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            monitoring_data_required.deleted_user = _username;
            monitoring_data_required.deleted_user_fullname = employee.displayname;
            monitoring_data_required.deleted_user_email = employee.mail;
            monitoring_data_required.deleted_date = DateTime.Now;
            _contextEHS.Update(monitoring_data_required);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool monitoring_data_requiredExists(int id)
        {
            return _contextEHS.monitoring_data_required.Any(e => e.id == id);
        }
    }
}
