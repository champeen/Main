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
    public class route_of_entryController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public route_of_entryController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: route_of_entry
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.route_of_entry.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: route_of_entry/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var route_of_entry = await _contextEHS.route_of_entry.FirstOrDefaultAsync(m => m.id == id);
            if (route_of_entry == null)
                return NotFound();

            return View(route_of_entry);
        }

        // GET: route_of_entry/Create
        public async Task<IActionResult> Create()
        {
            route_of_entry route_of_entry = new route_of_entry();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            route_of_entry.created_user = employee.onpremisessamaccountname;
            route_of_entry.created_user_fullname = employee.displayname;
            route_of_entry.created_user_email = employee.mail;
            route_of_entry.created_date = DateTime.Now;
            return View(route_of_entry);
        }

        // POST: route_of_entry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] route_of_entry route_of_entry)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(route_of_entry);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(route_of_entry);
        }

        // GET: route_of_entry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var route_of_entry = await _contextEHS.route_of_entry.FindAsync(id);
            if (route_of_entry == null)
                return NotFound();

            return View(route_of_entry);
        }

        // POST: route_of_entry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] route_of_entry route_of_entry)
        {
            if (id != route_of_entry.id)
                 return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    route_of_entry.modified_user = employee.onpremisessamaccountname;
                    route_of_entry.modified_user_fullname = employee.displayname;
                    route_of_entry.modified_user_email = employee.mail;
                    route_of_entry.modified_date = DateTime.Now;
                    _contextEHS.Update(route_of_entry);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!route_of_entryExists(route_of_entry.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(route_of_entry);
        }

        // GET: route_of_entry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var route_of_entry = await _contextEHS.route_of_entry.FirstOrDefaultAsync(m => m.id == id);
            if (route_of_entry == null)
                return NotFound();

            return View(route_of_entry);
        }

        // POST: route_of_entry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var route_of_entry = await _contextEHS.route_of_entry.FindAsync(id);
            if (route_of_entry == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            route_of_entry.deleted_user = _username;
            route_of_entry.deleted_user_fullname = employee.displayname;
            route_of_entry.deleted_user_email = employee.mail;
            route_of_entry.deleted_date = DateTime.Now;
            _contextEHS.Update(route_of_entry);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool route_of_entryExists(int id)
        {
            return _contextEHS.route_of_entry.Any(e => e.id == id);
        }
    }
}
