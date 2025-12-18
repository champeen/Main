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
    public class yes_noController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public yes_noController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: yes_no
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.yes_no.Where(m => m.deleted_date == null).OrderBy(m => m.sort_order).ThenBy(m => m.description).ToListAsync());
        }

        // GET: yes_no/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var yes_no = await _contextEHS.yes_no.FirstOrDefaultAsync(m => m.id == id);
            if (yes_no == null)
                return NotFound();

            return View(yes_no);
        }

        // GET: yes_no/Create
        public async Task<IActionResult> Create()
        {
            yes_no yes_no = new yes_no();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            yes_no.created_user = employee.onpremisessamaccountname;
            yes_no.created_user_fullname = employee.displayname;
            yes_no.created_user_email = employee.mail;
            yes_no.created_date = DateTime.Now;
            return View(yes_no);
        }

        // POST: yes_no/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] yes_no yes_no)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(yes_no);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yes_no);
        }

        // GET: yes_no/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var yes_no = await _contextEHS.yes_no.FindAsync(id);
            if (yes_no == null)
                return NotFound();

            return View(yes_no);
        }

        // POST: yes_no/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] yes_no yes_no)
        {
            if (id != yes_no.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    yes_no.modified_user = employee.onpremisessamaccountname;
                    yes_no.modified_user_fullname = employee.displayname;
                    yes_no.modified_user_email = employee.mail;
                    yes_no.modified_date = DateTime.Now;
                    _contextEHS.Update(yes_no);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!yes_noExists(yes_no.id))
                         return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(yes_no);
        }

        // GET: yes_no/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var yes_no = await _contextEHS.yes_no.FirstOrDefaultAsync(m => m.id == id);
            if (yes_no == null)
                return NotFound();

            return View(yes_no);
        }

        // POST: yes_no/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yes_no = await _contextEHS.yes_no.FindAsync(id);
            if (yes_no == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            yes_no.deleted_user = _username;
            yes_no.deleted_user_fullname = employee.displayname;
            yes_no.deleted_user_email = employee.mail;
            yes_no.deleted_date = DateTime.Now;
            _contextEHS.Update(yes_no);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool yes_noExists(int id)
        {
            return _contextEHS.yes_no.Any(e => e.id == id);
        }
    }
}
