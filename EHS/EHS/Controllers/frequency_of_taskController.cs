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
    public class frequency_of_taskController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public frequency_of_taskController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: frequency_of_task
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.frequency_of_task.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: frequency_of_task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var frequency_of_task = await _contextEHS.frequency_of_task.FirstOrDefaultAsync(m => m.id == id);
            if (frequency_of_task == null)
                return NotFound();

            return View(frequency_of_task);
        }

        // GET: frequency_of_task/Create
        public async Task<IActionResult> Create()
        {
            frequency_of_task frequency_of_task = new frequency_of_task();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            frequency_of_task.created_user = employee.onpremisessamaccountname;
            frequency_of_task.created_user_fullname = employee.displayname;
            frequency_of_task.created_user_email = employee.mail;
            frequency_of_task.created_date = DateTime.Now;
            return View(frequency_of_task);
        }

        // POST: frequency_of_task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] frequency_of_task frequency_of_task)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(frequency_of_task);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(frequency_of_task);
        }

        // GET: frequency_of_task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var frequency_of_task = await _contextEHS.frequency_of_task.FindAsync(id);
            if (frequency_of_task == null)
                return NotFound();

            return View(frequency_of_task);
        }

        // POST: frequency_of_task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] frequency_of_task frequency_of_task)
        {
            if (id != frequency_of_task.id)
                return NotFound();


            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    frequency_of_task.modified_user = employee.onpremisessamaccountname;
                    frequency_of_task.modified_user_fullname = employee.displayname;
                    frequency_of_task.modified_user_email = employee.mail;
                    frequency_of_task.modified_date = DateTime.Now;
                    _contextEHS.Update(frequency_of_task);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!frequency_of_taskExists(frequency_of_task.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(frequency_of_task);
        }

        // GET: frequency_of_task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var frequency_of_task = await _contextEHS.frequency_of_task.FirstOrDefaultAsync(m => m.id == id);
            if (frequency_of_task == null)
                return NotFound();

            return View(frequency_of_task);
        }

        // POST: frequency_of_task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var frequency_of_task = await _contextEHS.frequency_of_task.FindAsync(id);
            if (frequency_of_task == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            frequency_of_task.deleted_user = _username;
            frequency_of_task.deleted_user_fullname = employee.displayname;
            frequency_of_task.deleted_user_email = employee.mail;
            frequency_of_task.deleted_date = DateTime.Now;
            _contextEHS.Update(frequency_of_task);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool frequency_of_taskExists(int id)
        {
            return _contextEHS.frequency_of_task.Any(e => e.id == id);
        }
    }
}
