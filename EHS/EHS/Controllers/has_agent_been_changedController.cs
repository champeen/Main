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
    public class has_agent_been_changedController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public has_agent_been_changedController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: has_agent_been_changed
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.has_agent_been_changed.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: has_agent_been_changed/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var has_agent_been_changed = await _contextEHS.has_agent_been_changed.FirstOrDefaultAsync(m => m.id == id);
            if (has_agent_been_changed == null)
                return NotFound();

            return View(has_agent_been_changed);
        }

        // GET: has_agent_been_changed/Create
        public async Task<IActionResult> Create()
        {
            has_agent_been_changed has_agent_been_changed = new has_agent_been_changed();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            has_agent_been_changed.created_user = employee.onpremisessamaccountname;
            has_agent_been_changed.created_user_fullname = employee.displayname;
            has_agent_been_changed.created_user_email = employee.mail;
            has_agent_been_changed.created_date = DateTime.Now;
            return View(has_agent_been_changed);
        }

        // POST: has_agent_been_changed/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] has_agent_been_changed has_agent_been_changed)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(has_agent_been_changed);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(has_agent_been_changed);
        }

        // GET: has_agent_been_changed/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var has_agent_been_changed = await _contextEHS.has_agent_been_changed.FindAsync(id);
            if (has_agent_been_changed == null)
                return NotFound();

            return View(has_agent_been_changed);
        }

        // POST: has_agent_been_changed/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] has_agent_been_changed has_agent_been_changed)
        {
            if (id != has_agent_been_changed.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    has_agent_been_changed.modified_user = employee.onpremisessamaccountname;
                    has_agent_been_changed.modified_user_fullname = employee.displayname;
                    has_agent_been_changed.modified_user_email = employee.mail;
                    has_agent_been_changed.modified_date = DateTime.Now;
                    _contextEHS.Update(has_agent_been_changed);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!has_agent_been_changedExists(has_agent_been_changed.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(has_agent_been_changed);
        }

        // GET: has_agent_been_changed/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var has_agent_been_changed = await _contextEHS.has_agent_been_changed.FirstOrDefaultAsync(m => m.id == id);
            if (has_agent_been_changed == null)
                return NotFound();

            return View(has_agent_been_changed);
        }

        // POST: has_agent_been_changed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var has_agent_been_changed = await _contextEHS.has_agent_been_changed.FindAsync(id);
            if (has_agent_been_changed == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            has_agent_been_changed.deleted_user = _username;
            has_agent_been_changed.deleted_user_fullname = employee.displayname;
            has_agent_been_changed.deleted_user_email = employee.mail;
            has_agent_been_changed.deleted_date = DateTime.Now;
            _contextEHS.Update(has_agent_been_changed);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool has_agent_been_changedExists(int id)
        {
            return _contextEHS.has_agent_been_changed.Any(e => e.id == id);
        }
    }
}
