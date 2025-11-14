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
    public class agentController : BaseController 
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public agentController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: agents
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.agent.Where(m=>m.deleted_date == null).OrderBy(m=>m.exposure_type).ThenBy(m=>m.description).ToListAsync());
        }

        // GET: agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var agents = await _contextEHS.agent.FirstOrDefaultAsync(m => m.id == id);
            if (agents == null)
                return NotFound();

            return View(agents);
        }

        // GET: agents/Create
        public async Task<IActionResult> Create()
        {
            agent agent = new agent();
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            agent.created_user = employee.onpremisessamaccountname;
            agent.created_user_fullname = employee.displayname;
            agent.created_user_email = employee.mail;
            agent.created_date = DateTime.Now;

            ViewBag.ExposureTypes = getExposureTypes();

            return View(agent);
        }

        // POST: agents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,exposure_type,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] agent agents)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(agents);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ExposureTypes = getExposureTypes();
            return View(agents);
        }

        // GET: agents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var agents = await _contextEHS.agent.FindAsync(id);
            if (agents == null)
                return NotFound();

            ViewBag.ExposureTypes = getExposureTypes();

            return View(agents);
        }

        // POST: agents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,exposure_type,description,sort_order,display,created_user,created_user_fullname,created_user_email,created_date")] agent agents)
        {
            if (id != agents.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    agents.modified_user = employee.onpremisessamaccountname;
                    agents.modified_user_fullname = employee.displayname;
                    agents.modified_user_email = employee.mail;
                    agents.modified_date = DateTime.Now;
                    _contextEHS.Update(agents);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!agentsExists(agents.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExposureTypes = getExposureTypes();
            return View(agents);
        }

        // GET: agents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var agents = await _contextEHS.agent.FirstOrDefaultAsync(m => m.id == id);
            if (agents == null)
                return NotFound();

            return View(agents);
        }

        // POST: agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agents = await _contextEHS.agent.FindAsync(id);
            if (agents == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            agents.deleted_user = _username;
            agents.deleted_user_fullname = employee.displayname;
            agents.deleted_user_email = employee.mail;
            agents.deleted_date = DateTime.Now;
            _contextEHS.Update(agents);
            //_contextEHS.acute_chronic.Remove(acute_chronic);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool agentsExists(int id)
        {
            return _contextEHS.agent.Any(e => e.id == id);
        }
    }
}
