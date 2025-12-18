using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EHS.Data;
using EHS.Utilities;
using EHS.Models;
using EHS.Models.Dropdowns.SEG;

namespace EHS.Controllers.SEG.Dropdowns
{
    public class seg_roleController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public seg_roleController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: seg_role
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.seg_role.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: seg_role/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_role = await _contextEHS.seg_role.FirstOrDefaultAsync(m => m.id == id);
            if (seg_role == null)
                return NotFound();

            return View(seg_role);
        }

        // GET: seg_role/Create
        public async Task<IActionResult> Create()
        {
            seg_role role = new seg_role();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            role.created_user = employee.onpremisessamaccountname;
            role.created_user_fullname = employee.displayname;
            role.created_user_email = employee.mail;
            role.created_date = DateTime.Now;
            return View(role);
        }

        // POST: seg_role/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] seg_role seg_role)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(seg_role);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seg_role);
        }

        // GET: seg_role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_role = await _contextEHS.seg_role.FindAsync(id);
            if (seg_role == null)
                return NotFound();

            return View(seg_role);
        }

        // POST: seg_role/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] seg_role seg_role)
        {
            if (id != seg_role.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    seg_role.modified_user = employee.onpremisessamaccountname;
                    seg_role.modified_user_fullname = employee.displayname;
                    seg_role.modified_user_email = employee.mail;
                    seg_role.modified_date = DateTime.Now;
                    _contextEHS.Update(seg_role);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!seg_roleExists(seg_role.id))
                        return NotFound();
                    else
                        throw;
                 }
                return RedirectToAction(nameof(Index));
            }
            return View(seg_role);
        }

        // GET: seg_role/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_role = await _contextEHS.seg_role.FirstOrDefaultAsync(m => m.id == id);
            if (seg_role == null)
                return NotFound();

            return View(seg_role);
        }

        // POST: seg_role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seg_role = await _contextEHS.seg_role.FindAsync(id);
            if (seg_role == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            seg_role.deleted_user = _username;
            seg_role.deleted_user_fullname = employee.displayname;
            seg_role.deleted_user_email = employee.mail;
            seg_role.deleted_date = DateTime.Now;
            _contextEHS.Update(seg_role);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool seg_roleExists(int id)
        {
            return _contextEHS.seg_role.Any(e => e.id == id);
        }
    }
}
