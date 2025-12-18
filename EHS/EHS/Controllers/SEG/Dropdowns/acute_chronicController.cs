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
    public class acute_chronicController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public acute_chronicController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: acute_chronic
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.acute_chronic.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: acute_chronic/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var acute_chronic = await _contextEHS.acute_chronic.FirstOrDefaultAsync(m => m.id == id);
            if (acute_chronic == null)
                return NotFound();

            return View(acute_chronic);
        }

        // GET: acute_chronic/Create
        public async Task<IActionResult> Create()
        {
            acute_chronic acute_chronic = new acute_chronic();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            acute_chronic.created_user = employee.onpremisessamaccountname;
            acute_chronic.created_user_fullname = employee.displayname;
            acute_chronic.created_user_email = employee.mail;
            acute_chronic.created_date = DateTime.Now;
            return View(acute_chronic);
        }

        // POST: acute_chronic/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] acute_chronic acute_chronic)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(acute_chronic);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(acute_chronic);
        }

        // GET: acute_chronic/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var acute_chronic = await _contextEHS.acute_chronic.FindAsync(id);
            if (acute_chronic == null)
                return NotFound();

            return View(acute_chronic);
        }

        // POST: acute_chronic/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] acute_chronic acute_chronic)
        {
            if (id != acute_chronic.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    acute_chronic.modified_user = employee.onpremisessamaccountname;
                    acute_chronic.modified_user_fullname = employee.displayname;
                    acute_chronic.modified_user_email = employee.mail;
                    acute_chronic.modified_date = DateTime.Now;
                    _contextEHS.Update(acute_chronic);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!acute_chronicExists(acute_chronic.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(acute_chronic);
        }

        // GET: acute_chronic/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var acute_chronic = await _contextEHS.acute_chronic.FirstOrDefaultAsync(m => m.id == id);
            if (acute_chronic == null)
                return NotFound();

            return View(acute_chronic);
        }

        // POST: acute_chronic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acute_chronic = await _contextEHS.acute_chronic.FindAsync(id);
            if (acute_chronic == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            acute_chronic.deleted_user = _username;
            acute_chronic.deleted_user_fullname = employee.displayname;
            acute_chronic.deleted_user_email = employee.mail;
            acute_chronic.deleted_date = DateTime.Now;
            _contextEHS.Update(acute_chronic);
            //_contextEHS.acute_chronic.Remove(acute_chronic);
            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool acute_chronicExists(int id)
        {
            return _contextEHS.acute_chronic.Any(e => e.id == id);
        }
    }
}
