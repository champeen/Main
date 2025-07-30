using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EHS.Data;
using EHS.Models.Dropdowns;
using EHS.Utilities;
using EHS.Models;

namespace EHS.Controllers
{
    public class tasksController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public tasksController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: tasks
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.task.Where(m => m.deleted_date == null).OrderBy(m=>m.sort_order).ThenBy(m=>m.description).ToListAsync());
        }

        // GET: tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _contextEHS.task.FirstOrDefaultAsync(m => m.id == id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        // GET: tasks/Create
        public async Task<IActionResult> Create()
        {
            task task = new task();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            task.created_user = employee.onpremisessamaccountname;
            task.created_user_fullname = employee.displayname;
            task.created_user_email = employee.mail;
            task.created_date = DateTime.Now;
            return View(task);
        }

        // POST: tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,exposure_type,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] task task)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(task);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _contextEHS.task.FindAsync(id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,exposure_type,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] task task)
        {
            if (id != task.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    task.modified_user = employee.onpremisessamaccountname;
                    task.modified_user_fullname = employee.displayname;
                    task.modified_user_email = employee.mail;
                    task.modified_date = DateTime.Now;
                    _contextEHS.Update(task);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!taskExists(task.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _contextEHS.task.FirstOrDefaultAsync(m => m.id == id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _contextEHS.task.FindAsync(id);
            if (task == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            task.deleted_user = _username;
            task.deleted_user_fullname = employee.displayname;
            task.deleted_user_email = employee.mail;
            task.deleted_date = DateTime.Now;
            _contextEHS.Update(task);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool taskExists(int id)
        {
            return _contextEHS.task.Any(e => e.id == id);
        }
    }
}
