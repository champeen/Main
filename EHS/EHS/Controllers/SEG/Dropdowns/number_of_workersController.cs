using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns.SEG;
using EHS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers.SEG.Dropdowns
{
    public class number_of_workersController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public number_of_workersController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: number_of_workers
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.number_of_workers.Where(m => m.deleted_date == null).OrderBy(m=>m.sort_order).ThenBy(m=>m.description).ToListAsync());
        }

        // GET: number_of_workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var number_of_workers = await _contextEHS.number_of_workers.FirstOrDefaultAsync(m => m.id == id);
            if (number_of_workers == null)
                return NotFound();

            return View(number_of_workers);
        }

        // GET: number_of_workers/Create
        public async Task<IActionResult> Create()
        {
            number_of_workers number_of_workers = new number_of_workers();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            number_of_workers.created_user = employee.onpremisessamaccountname;
            number_of_workers.created_user_fullname = employee.displayname;
            number_of_workers.created_user_email = employee.mail;
            number_of_workers.created_date = DateTime.Now;
            return View(number_of_workers);
        }

        // POST: number_of_workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] number_of_workers number_of_workers)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(number_of_workers);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(number_of_workers);
        }

        // GET: number_of_workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var number_of_workers = await _contextEHS.number_of_workers.FindAsync(id);
            if (number_of_workers == null)
                return NotFound();

            return View(number_of_workers);
        }

        // POST: number_of_workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] number_of_workers number_of_workers)
        {
            if (id != number_of_workers.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    number_of_workers.modified_user = employee.onpremisessamaccountname;
                    number_of_workers.modified_user_fullname = employee.displayname;
                    number_of_workers.modified_user_email = employee.mail;
                    number_of_workers.modified_date = DateTime.Now;
                    _contextEHS.Update(number_of_workers);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!number_of_workersExists(number_of_workers.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(number_of_workers);
        }

        // GET: number_of_workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var number_of_workers = await _contextEHS.number_of_workers.FirstOrDefaultAsync(m => m.id == id);
            if (number_of_workers == null)
                return NotFound();

            return View(number_of_workers);
        }

        // POST: number_of_workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var number_of_workers = await _contextEHS.number_of_workers.FindAsync(id);
            if (number_of_workers == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            number_of_workers.deleted_user = _username;
            number_of_workers.deleted_user_fullname = employee.displayname;
            number_of_workers.deleted_user_email = employee.mail;
            number_of_workers.deleted_date = DateTime.Now;
            _contextEHS.Update(number_of_workers);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool number_of_workersExists(int id)
        {
            return _contextEHS.number_of_workers.Any(e => e.id == id);
        }
    }
}
