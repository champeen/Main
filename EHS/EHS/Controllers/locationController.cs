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
    public class locationController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public locationController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.location.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _contextEHS.location.FirstOrDefaultAsync(m => m.id == id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // GET: Locations/Create
        public async Task<IActionResult> Create()
        {
            location location = new location();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            location.created_user = employee.onpremisessamaccountname;
            location.created_user_fullname = employee.displayname;
            location.created_user_email = employee.mail;            
            location.created_date = DateTime.Now;
            return View(location);
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] location location)
        {

            if (ModelState.IsValid)
            {
                _contextEHS.Add(location);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _contextEHS.location.FindAsync(id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] location location)
        {
            if (id != location.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    location.modified_user = employee.onpremisessamaccountname;
                    location.modified_user_fullname = employee.displayname;
                    location.modified_user_email = employee.mail;
                    location.modified_date = DateTime.Now;
                    _contextEHS.Update(location);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _contextEHS.location.FirstOrDefaultAsync(m => m.id == id);
            if (location == null)
                return NotFound();

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _contextEHS.location.FindAsync(id);
            if (location == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            location.deleted_user = _username;
            location.deleted_user_fullname = employee.displayname;
            location.deleted_user_email = employee.mail;
            location.deleted_date = DateTime.Now;
            _contextEHS.Update(location);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _contextEHS.location.Any(e => e.id == id);
        }
    }
}
