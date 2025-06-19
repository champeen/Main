using EHS.Data;
using EHS.Models;
using EHS.Utilities;
using EHS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHS.Controllers
{
    public class seg_risk_assessmentController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public seg_risk_assessmentController(EHSContext contextEHS, MOCContext contextMOC) : base (contextEHS, contextMOC) 
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: seg_risk_assessments
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.seg_risk_assessment.ToListAsync());
        }

        // GET: seg_risk_assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessment = await _contextEHS.seg_risk_assessment
                .FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessment == null)
                return NotFound();

            SegViewModel segViewModel = new SegViewModel();
            segViewModel.seg_risk_assessments = seg_risk_assessment;

            // get all attachments
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessment.id.ToString()));

            // TEST
//            bool found = Directory.Exists(Initialization.AttachmentDirectory_IH_SEG);

//            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessments.id.ToString())))
//                path.Create();

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
//            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<ViewModels.Attachment> attachments = new List<ViewModels.Attachment>();
//            foreach (FileInfo i in Files)
//            {
//                ViewModels.Attachment attachment = new ViewModels.Attachment
//                {
//                    Directory = i.DirectoryName,
//                    Name = i.Name,
//                    Extension = i.Extension,
//                    FullPath = i.FullName,
//                    CreatedDate = i.CreationTimeUtc.Date,
//                    Size = Convert.ToInt32(i.Length)
//                };
//                attachments.Add(attachment);

                //var blah = i.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
//            }
            segViewModel.attachments = attachments.OrderBy(m => m.Name).ToList();

            segViewModel.IsAdmin = _isAdmin;
            segViewModel.Username = _username;

            return View(segViewModel);
        }

        // GET: seg_risk_assessments/Create
        public async Task<IActionResult> Create()
        {
            seg_risk_assessment seg = new seg_risk_assessment();
            seg.created_user = _username;
            ViewBag.Employees = getUserList();
            ViewBag.Locations = getLocations();
            ViewBag.ExposureTypes = getExposureTypes();
            ViewBag.Agents = getAgents();
            ViewBag.SegRoles = getSegRoles();
            ViewBag.Tasks = getTasks();
            ViewBag.OccupationalExposureLimits = getOccupationalExposureLimits();
            ViewBag.AcuteChronic = getAcuteChronic();
            ViewBag.RouteOfEntry = getRouteOfEntry();
            return View(seg);
        }

        // POST: seg_risk_assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment_username,created_user")] seg_risk_assessment seg_risk_assessment)
        {     
            if (ModelState.IsValid)
            {
                seg_risk_assessment.person_performing_assessment_displayname = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == seg_risk_assessment.person_performing_assessment_username).Select(m => m.displayname).FirstOrDefaultAsync();
                seg_risk_assessment.created_date = DateTime.Now;
                _contextEHS.Add(seg_risk_assessment);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Employees = getUserList();
            ViewBag.Locations = getLocations();
            ViewBag.ExposureTypes = getExposureTypes();
            ViewBag.Agents = getAgents();
            ViewBag.SegRoles = getSegRoles();
            ViewBag.Tasks = getTasks();
            ViewBag.OccupationalExposureLimits = getOccupationalExposureLimits();
            ViewBag.AcuteChronic = getAcuteChronic();
            ViewBag.RouteOfEntry = getRouteOfEntry();
            return View(seg_risk_assessment);
        }

        // GET: seg_risk_assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessments = await _contextEHS.seg_risk_assessment.FindAsync(id);
            if (seg_risk_assessments == null)
                 return NotFound();

            ViewBag.Employees = getUserList();
            ViewBag.Locations = getLocations();
            ViewBag.ExposureTypes = getExposureTypes();
            ViewBag.Agents = getAgents();
            ViewBag.SegRoles = getSegRoles();
            ViewBag.Tasks = getTasks();
            ViewBag.OccupationalExposureLimits = getOccupationalExposureLimits();
            ViewBag.AcuteChronic = getAcuteChronic();
            ViewBag.RouteOfEntry = getRouteOfEntry();
            return View(seg_risk_assessments);
        }

        // POST: seg_risk_assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment_username,created_user,created_user_fullname,created_user_email,created_date,deleted_user,deleted_user_fullname,deleted_user_email,deleted_date")] seg_risk_assessment seg_risk_assessments)
        {
            if (id != seg_risk_assessments.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    seg_risk_assessments.person_performing_assessment_displayname = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == seg_risk_assessments.person_performing_assessment_username).Select(m => m.displayname).FirstOrDefaultAsync();
                    seg_risk_assessments.modified_user = _username;
                    seg_risk_assessments.modified_date = DateTime.Now;
                    _contextEHS.Update(seg_risk_assessments);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!seg_risk_assessmentsExists(seg_risk_assessments.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = getUserList();
            ViewBag.Locations = getLocations();
            ViewBag.ExposureTypes = getExposureTypes();
            ViewBag.Agents = getAgents();
            ViewBag.SegRoles = getSegRoles();
            ViewBag.Tasks = getTasks();
            ViewBag.OccupationalExposureLimits = getOccupationalExposureLimits();
            ViewBag.AcuteChronic = getAcuteChronic();
            ViewBag.RouteOfEntry = getRouteOfEntry();
            return View(seg_risk_assessments);
        }

        // GET: seg_risk_assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessments = await _contextEHS.seg_risk_assessment
                .FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessments == null)
                return NotFound();

            return View(seg_risk_assessments);
        }

        // POST: seg_risk_assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seg_risk_assessments = await _contextEHS.seg_risk_assessment.FindAsync(id);
            if (seg_risk_assessments != null)
                _contextEHS.seg_risk_assessment.Remove(seg_risk_assessments);

            await _contextEHS.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool seg_risk_assessmentsExists(int id)
        {
            return _contextEHS.seg_risk_assessment.Any(e => e.id == id);
        }

        [HttpPost]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> SaveFile(int id, IFormFile? fileAttachment)
        {
            if (id == null || _contextEHS.seg_risk_assessment == null)
                return NotFound();

            if (fileAttachment == null || fileAttachment.Length == 0)
                return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "No File Has Been Selected For Upload" });

            var ptn = await _contextEHS.seg_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (ptn == null)
                return RedirectToAction("Index");

            //// make sure the file being uploaded is an allowable file extension type....
            //var extensionType = Path.GetExtension(fileAttachment.FileName);
            //var found = _contextEHS.AllowedAttachmentExtensions
            //    .Where(m => m.ExtensionName == extensionType)
            //    .Any();

            //if (!found)
            //    return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            // attachment storage file path should already exist, but just make sure....
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, ptn.id.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, ptn.id.ToString())))
                path.Create();

            string filePath = Path.Combine(Initialization.AttachmentDirectory_IH_SEG, ptn.id.ToString(), fileAttachment.FileName);

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
            return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn" });
        }

        [HttpGet]
        public JsonResult GetAgentByExposureType(string exposureType)
        {
            var agents = _contextEHS.agent.Where(s => s.exposure_type == exposureType)
                                        .Select(s => new { s.description })
                                        .OrderBy(s=>s.description).ToList();
            return Json(agents);
        }

    }
}
