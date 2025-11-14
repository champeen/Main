using EHS.Data;
using EHS.Models;
using EHS.Models.IH;
using EHS.Utilities;
using EHS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHS.Controllers.ChemicalRiskAssessment
{
    public class chemical_risk_assessmentController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public chemical_risk_assessmentController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: chemical_risk_assessment
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.chemical_risk_assessment.Where(m => m.deleted_date == null).OrderBy(m => m.id).ToListAsync());
        }

        // GET: chemical_risk_assessment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (chemical_risk_assessment == null)
                return NotFound();

            return View(chemical_risk_assessment);
        }

        // GET: chemical_risk_assessment/Create
        public async Task<IActionResult> Create()
        {
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                throw new Exception(_username == null ? "username is null" : _username);

            chemical_risk_assessment cra = new chemical_risk_assessment();
            cra.created_user = employee.onpremisessamaccountname;
            cra.created_user_fullname = employee.displayname;
            cra.created_user_email = employee.mail;
            cra.created_date = DateTime.Now;

            ChemicalRiskAssessmentViewModel vm = new ChemicalRiskAssessmentViewModel();
            vm.chemical_risk_assessment = cra;
            vm.Username = _username;

            get_CRA_DropdownSelectionLists();
            return View(vm);
        }

        // POST: chemical_risk_assessment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(chemical_risk_assessment chemical_risk_assessment)
        {
            if (ModelState.IsValid)
            {
                _contextEHS.Add(chemical_risk_assessment);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chemical_risk_assessment);
        }

        // GET: chemical_risk_assessment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FindAsync(id);
            if (chemical_risk_assessment == null)
                return NotFound();

            return View(chemical_risk_assessment);
        }

        // POST: chemical_risk_assessment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, chemical_risk_assessment chemical_risk_assessment)
        {
            if (id != chemical_risk_assessment.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contextEHS.Update(chemical_risk_assessment);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!chemical_risk_assessmentExists(chemical_risk_assessment.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chemical_risk_assessment);
        }

        // GET: chemical_risk_assessment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (chemical_risk_assessment == null)
                return NotFound();

            return View(chemical_risk_assessment);
        }

        // POST: chemical_risk_assessment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FindAsync(id);
            if (chemical_risk_assessment != null)
                _contextEHS.chemical_risk_assessment.Remove(chemical_risk_assessment);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool chemical_risk_assessmentExists(int id)
        {
            return _contextEHS.chemical_risk_assessment.Any(e => e.id == id);
        }

        public async Task<IActionResult> Attachments(int? id)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (chemical_risk_assessment == null)
                return NotFound();

            ChemicalRiskAssessmentViewModel vm = new ChemicalRiskAssessmentViewModel();
            vm.chemical_risk_assessment = chemical_risk_assessment;
            vm.Username = _username;

            // GET ALL ATTACHMENTS
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, chemical_risk_assessment.id.ToString()));

            // TEST
            bool found = Directory.Exists(Initialization.AttachmentDirectory_IH_CRA);

            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, chemical_risk_assessment.id.ToString())))
                path.Create();

            // Using GetFiles() method to get list of all the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<Attachment> attachments = new List<Attachment>();
            foreach (FileInfo i in Files)
            {
                Attachment attachment = new Attachment
                {
                    Directory = i.DirectoryName,
                    Name = i.Name,
                    Extension = i.Extension,
                    FullPath = i.FullName,
                    CreatedDate = i.CreationTimeUtc.Date,
                    Size = Convert.ToInt32(i.Length)
                };
                attachments.Add(attachment);

                //var blah = i.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            vm.attachments = attachments.OrderBy(m => m.Name).ToList();

            vm.IsAdmin = _isAdmin;
            vm.Username = _username;

            return View(vm);
        }
    }
}
