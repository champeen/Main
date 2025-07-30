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
    public class exposure_typeController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public exposure_typeController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: exposure_types
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.exposure_type.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: exposure_types/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_type = await _contextEHS.exposure_type.FirstOrDefaultAsync(m => m.id == id);

            if (exposure_type == null)
                return NotFound();

            return View(exposure_type);
        }

        // GET: exposure_types/Create
        public async Task<IActionResult> Create()
        {
            exposure_type et = new exposure_type();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            et.created_user = employee.onpremisessamaccountname;
            et.created_user_fullname = employee.displayname;
            et.created_user_email = employee.mail;
            et.created_date = DateTime.Now;
            return View(et);
        }

        // POST: exposure_types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] exposure_type exposure_type)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(exposure_type);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exposure_type);
        }

        // GET: exposure_types/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_type = await _contextEHS.exposure_type.FindAsync(id);
            if (exposure_type == null)
                return NotFound();

            return View(exposure_type);
        }

        // POST: exposure_types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] exposure_type exposure_type)
        {
            if (id != exposure_type.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    exposure_type.modified_user = employee.onpremisessamaccountname;
                    exposure_type.modified_user_fullname = employee.displayname;
                    exposure_type.modified_user_email = employee.mail;
                    exposure_type.modified_date = DateTime.Now;
                    _contextEHS.Update(exposure_type);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!exposure_typeExists(exposure_type.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exposure_type);
        }

        // GET: exposure_types/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exposure_type = await _contextEHS.exposure_type
                .FirstOrDefaultAsync(m => m.id == id);
            if (exposure_type == null)
                return NotFound();

            return View(exposure_type);
        }

        // POST: exposure_types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exposure_type = await _contextEHS.exposure_type.FindAsync(id);
            if (exposure_type == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            exposure_type.deleted_user = _username;
            exposure_type.deleted_user_fullname = employee.displayname;
            exposure_type.deleted_user_email = employee.mail;
            exposure_type.deleted_date = DateTime.Now;
            _contextEHS.Update(exposure_type);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool exposure_typeExists(int id)
        {
            return _contextEHS.exposure_type.Any(e => e.id == id);
        }
    }
}
