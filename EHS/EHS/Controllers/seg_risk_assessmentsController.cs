using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EHS.Data;
using EHS.Models;
using EHS.Utilities;

namespace EHS.Controllers
{
    public class seg_risk_assessmentsController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public seg_risk_assessmentsController(EHSContext contextEHS, MOCContext contextMOC) : base (contextEHS, contextMOC) 
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: seg_risk_assessments
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.seg_risk_assessments.ToListAsync());
        }

        // GET: seg_risk_assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seg_risk_assessments = await _contextEHS.seg_risk_assessments
                .FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessments == null)
            {
                return NotFound();
            }

            return View(seg_risk_assessments);
        }

        // GET: seg_risk_assessments/Create
        public IActionResult Create()
        {
            seg_risk_assessments seg = new seg_risk_assessments();
            seg.created_user = _username;
            ViewBag.Employees = getUserList();
            return View(seg);
        }

        // POST: seg_risk_assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment,created_user")] seg_risk_assessments seg_risk_assessments)
        {     
            if (ModelState.IsValid)
            {
                seg_risk_assessments.created_date = DateTime.Now;
                _contextEHS.Add(seg_risk_assessments);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = getUserList();
            return View(seg_risk_assessments);
        }

        // GET: seg_risk_assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seg_risk_assessments = await _contextEHS.seg_risk_assessments.FindAsync(id);
            if (seg_risk_assessments == null)
            {
                return NotFound();
            }

            ViewBag.Employees = getUserList();
            return View(seg_risk_assessments);
        }

        // POST: seg_risk_assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment,created_user,created_user_fullname,created_user_email,created_date,deleted_user,deleted_user_fullname,deleted_user_email,deleted_date")] seg_risk_assessments seg_risk_assessments)
        {
            if (id != seg_risk_assessments.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    seg_risk_assessments.modified_user = _username;
                    seg_risk_assessments.modified_date = DateTime.Now;
                    _contextEHS.Update(seg_risk_assessments);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!seg_risk_assessmentsExists(seg_risk_assessments.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = getUserList();
            return View(seg_risk_assessments);
        }

        // GET: seg_risk_assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seg_risk_assessments = await _contextEHS.seg_risk_assessments
                .FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessments == null)
            {
                return NotFound();
            }

            return View(seg_risk_assessments);
        }

        // POST: seg_risk_assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seg_risk_assessments = await _contextEHS.seg_risk_assessments.FindAsync(id);
            if (seg_risk_assessments != null)
            {
                _contextEHS.seg_risk_assessments.Remove(seg_risk_assessments);
            }

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool seg_risk_assessmentsExists(int id)
        {
            return _contextEHS.seg_risk_assessments.Any(e => e.id == id);
        }
    }
}
