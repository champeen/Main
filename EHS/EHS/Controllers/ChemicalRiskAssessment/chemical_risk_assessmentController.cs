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
using System.Collections.Immutable;
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
            var hazardCodes = await _contextEHS.hazard_codes
                .AsNoTracking()
                //.Where(h => h.display)
                .OrderBy(h => h.sort_order)
                .ToListAsync();

            // id -> "CODE - Description"
            var hazardLookup = hazardCodes.ToDictionary(
                h => h.id,
                h => $"{h.code} - {h.description}"
            );

            // id -> risk_rating (1–4)
            var hazardRisk = hazardCodes.ToDictionary(
                h => h.id,
                h => h.risk_rating
            );

            ViewData["HazardLookup"] = hazardLookup;
            ViewData["HazardRisk"] = hazardRisk;

            // Main list: no Include, no tracking (read-only)
            var list = await _contextEHS.chemical_risk_assessment
                .Where(m => m.deleted_date == null)
                .OrderBy(m => m.id)
                .AsNoTracking()
                .ToListAsync();

            // Get just the IDs we care about
            var assessmentIds = list.Select(m => m.id).ToList();

            // Lightweight query: id + COUNT(*)
            var compositionCounts = await _contextEHS.chemical_composition
                .Where(c => assessmentIds.Contains(c.chemical_risk_assessment_id))
                .GroupBy(c => c.chemical_risk_assessment_id)
                .Select(g => new
                {
                    AssessmentId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.AssessmentId, x => x.Count);

            ViewBag.CompositionCounts = compositionCounts;

            return View(list);
        }


        // GET: chemical_risk_assessment/Details/5
        public async Task<IActionResult> Details(int? id, string fileAttachmentError = null)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (chemical_risk_assessment == null)
                return NotFound();

            chemical_risk_assessment.composition = await _contextEHS.chemical_composition.Where(m => m.chemical_risk_assessment_id == id && m.deleted_date == null).OrderBy(m => m.chemical_name).ToListAsync();

            ChemicalRiskAssessmentViewModel craViewModel = new ChemicalRiskAssessmentViewModel();
            craViewModel.chemical_risk_assessment = chemical_risk_assessment;
            craViewModel.Username = _username;
            craViewModel.FileAttachmentError = fileAttachmentError;

            // Load hazard codes once, reuse for text + risk rating
            var hazardCodes = await _contextEHS.hazard_codes
                .AsNoTracking()
                .OrderBy(h => h.sort_order)
                .ToListAsync();

            // id -> "CODE - Description"
            ViewBag.HazardLookup = hazardCodes.ToDictionary(
                h => h.id,
                h => $"{h.code} - {h.description}"
            );

            // id -> risk_rating (1–4)
            ViewBag.HazardRisk = hazardCodes.ToDictionary(
                h => h.id,
                h => h.risk_rating
            );

            // GET ALL ATTACHMENTS
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, chemical_risk_assessment.id.ToString()));

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
            craViewModel.attachments = attachments.OrderBy(m => m.Name).ToList();

            craViewModel.IsAdmin = _isAdmin;
            craViewModel.Username = _username;            

            return View(craViewModel);
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
            cra.created_date = DateTime.Today;
            cra.date_conducted = DateTime.Today;

            //ChemicalRiskAssessmentViewModel vm = new ChemicalRiskAssessmentViewModel();
            //vm.chemical_risk_assessment = cra;
            //vm.Username = _username;

            get_CRA_DropdownSelectionLists();
            return View(cra);
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

            get_CRA_DropdownSelectionLists();
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

            get_CRA_DropdownSelectionLists();

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
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    chemical_risk_assessment.modified_user = _username;
                    chemical_risk_assessment.modified_user_fullname = employee.displayname;
                    chemical_risk_assessment.modified_user_email = employee.mail;
                    chemical_risk_assessment.modified_date = DateTime.Now;
                    chemical_risk_assessment.person_performing_assessment_displayname = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == chemical_risk_assessment.person_performing_assessment_username).Select(m => m.displayname).FirstOrDefaultAsync();

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
            get_CRA_DropdownSelectionLists();
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

        public async Task<IActionResult> Attachments(int? id, string fileAttachmentError = null)
        {
            if (id == null)
                return NotFound();

            var chemical_risk_assessment = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (chemical_risk_assessment == null)
                return NotFound();

            ChemicalRiskAssessmentViewModel craViewModel = new ChemicalRiskAssessmentViewModel();
            craViewModel.chemical_risk_assessment = chemical_risk_assessment;
            craViewModel.Username = _username;
            craViewModel.IsAdmin = _isAdmin;
            craViewModel.FileAttachmentError = fileAttachmentError;

            // Get Hazard Codes
            var hazardCodes = await _contextEHS.hazard_codes
                .AsNoTracking()
                //.Where(h => h.display)
                .OrderBy(h => h.sort_order)
                .ToListAsync();

            // id -> "CODE - Description"
            var hazardLookup = hazardCodes.ToDictionary(
                h => h.id,
                h => $"{h.code} - {h.description}"
            );

            // id -> risk_rating (1–4)
            var hazardRisk = hazardCodes.ToDictionary(
                h => h.id,
                h => h.risk_rating
            );

            ViewData["HazardLookup"] = hazardLookup;
            ViewData["HazardRisk"] = hazardRisk;

            //// Main list: no Include, no tracking (read-only)
            //var list = await _contextEHS.chemical_risk_assessment
            //    .Where(m => m.deleted_date == null)
            //    .OrderBy(m => m.id)
            //    .AsNoTracking()
            //    .ToListAsync();

            // Get just the IDs we care about
            var assessmentIds = new List<int> { chemical_risk_assessment.id };

            // Lightweight query: id + COUNT(*)
            var compositionCounts = await _contextEHS.chemical_composition
                .Where(c => assessmentIds.Contains(c.chemical_risk_assessment_id))
                .GroupBy(c => c.chemical_risk_assessment_id)
                .Select(g => new
                {
                    AssessmentId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.AssessmentId, x => x.Count);

            ViewBag.CompositionCounts = compositionCounts;

            // GET ALL ATTACHMENTS
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, chemical_risk_assessment.id.ToString()));

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
            craViewModel.attachments = attachments.OrderBy(m => m.Name).ToList();

            return View(craViewModel);
        }

        [HttpPost]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SaveAttachment(int id, IFormFile? fileAttachment, string sourceView = null)
        {
            if (id == null || _contextEHS.chemical_risk_assessment == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                if (sourceView == "Attachments")
                    return RedirectToAction("Attachments", "chemical_risk_assessment",new { id, fileAttachmentError = "No File Has Been Selected For Upload" });
                else
                    return RedirectToAction("Details", "chemical_risk_assessment", new { id, fileAttachmentError = "No File Has Been Selected For Upload" });

            var cra = await _contextEHS.chemical_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (cra == null)
                return RedirectToAction("Index");

            //// make sure the file being uploaded is an allowable file extension type....
            //var extensionType = Path.GetExtension(fileAttachment.FileName);
            //var found = _contextEHS.AllowedAttachmentExtensions
            //    .Where(m => m.ExtensionName == extensionType)
            //    .Any();

            //if (!found)
            //    return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            // attachment storage file path should already exist, but just make sure....
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, cra.id.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_CRA, cra.id.ToString())))
                path.Create();

            string filePath = Path.Combine(Initialization.AttachmentDirectory_IH_CRA, cra.id.ToString(), fileAttachment.FileName);

            //// if file exists but not in draft mode, do NOT let user replace it.....
            //if (ptn.Status != "Draft" && System.IO.File.Exists(filePath))
            //{
            //    // return message to user saying they cannot overwrite a document after PTN has been approved.  You can only upload new documents.
            //    return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "Cannot Overwrite Existing Attachment.  Past Draft Mode. Must Create New Attachment." });
            //}
            //else
            //{
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileAttachment.CopyToAsync(fileStream);
            }
            //}
            if (sourceView == "Attachments")
                return RedirectToAction("Attachments", new { id});
            else
                return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> DownloadAttachment(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }

        public async Task<IActionResult> DeleteAttachment(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Attachments", new { id });
        }

    }
}
