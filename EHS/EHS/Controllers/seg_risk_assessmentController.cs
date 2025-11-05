using EHS.Data;
using EHS.Models;
using EHS.Models.Dropdowns;
using EHS.Utilities;
using EHS.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EHS.Controllers
{
    public class seg_risk_assessmentController : BaseController
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;

        public seg_risk_assessmentController(EHSContext contextEHS, MOCContext contextMOC) : base(contextEHS, contextMOC)
        {
            _contextEHS = contextEHS;
            _contextMOC = contextMOC;
        }

        // GET: seg_risk_assessments
        public async Task<IActionResult> Index()
        {
            return View(await _contextEHS.seg_risk_assessment.Where(m => m.deleted_date == null).OrderBy(m => m.id).ToListAsync());
        }

        public async Task<IActionResult> Attachments(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessment = await _contextEHS.seg_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessment == null)
                return NotFound();

            SegViewModel segViewModel = new SegViewModel();
            segViewModel.seg_risk_assessment = seg_risk_assessment;
            segViewModel.Username = _username;

            // GET ALL ATTACHMENTS
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessment.id.ToString()));

            // TEST
            bool found = Directory.Exists(Initialization.AttachmentDirectory_IH_SEG);

            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessment.id.ToString())))
                path.Create();

            // Using GetFiles() method to get list of all the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<ViewModels.Attachment> attachments = new List<ViewModels.Attachment>();
            foreach (FileInfo i in Files)
            {
                ViewModels.Attachment attachment = new ViewModels.Attachment
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
            segViewModel.attachments = attachments.OrderBy(m => m.Name).ToList();

            segViewModel.IsAdmin = _isAdmin;
            segViewModel.Username = _username;

            return View(segViewModel);
        }

        // GET: seg_risk_assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessment = await _contextEHS.seg_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (seg_risk_assessment == null)
                return NotFound();

            SegViewModel segViewModel = new SegViewModel();
            segViewModel.seg_risk_assessment = seg_risk_assessment;
            segViewModel.Username = _username;

            // GET ALL ATTACHMENTS
            // Get the directory
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessment.id.ToString()));

            // TEST
            bool found = Directory.Exists(Initialization.AttachmentDirectory_IH_SEG);

            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg_risk_assessment.id.ToString())))
                path.Create();

            // Using GetFiles() method to get list of all the files present in the Train directory
            FileInfo[] Files = path.GetFiles();

            // Display the file names
            List<ViewModels.Attachment> attachments = new List<ViewModels.Attachment>();
            foreach (FileInfo i in Files)
            {
                ViewModels.Attachment attachment = new ViewModels.Attachment
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
            segViewModel.attachments = attachments.OrderBy(m => m.Name).ToList();

            segViewModel.IsAdmin = _isAdmin;
            segViewModel.Username = _username;

            return View(segViewModel);
        }

        // GET: seg_risk_assessments/Create
        public async Task<IActionResult> Create()
        {
            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                throw new Exception(_username == null ? "username is null" : _username);
            //return RedirectToAction(nameof(Index));

            seg_risk_assessment seg = new seg_risk_assessment();
            seg.created_user = employee.onpremisessamaccountname;
            seg.created_user_fullname = employee.displayname;
            seg.created_user_email = employee.mail;
            seg.created_date = DateTime.Now;

            SegViewModel segViewModel = new SegViewModel();
            segViewModel.seg_risk_assessment = seg;
            segViewModel.Username = _username;

            getDropdownSelectionLists();
            return View(segViewModel);
        }

        // POST: seg_risk_assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment_username,created_user")]*/ SegViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                seg_risk_assessment segRec = new seg_risk_assessment();
                segRec = viewModel.seg_risk_assessment;
                segRec.person_performing_assessment_displayname = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == viewModel.seg_risk_assessment.person_performing_assessment_username).Select(m => m.displayname).FirstOrDefaultAsync();
                segRec.duration_of_task = viewModel.DurationOfTask ?? TimeSpan.Zero;
                segRec.exposure_rating_description = await _contextEHS.exposure_rating.Where(m => m.value == segRec.exposure_rating).Select(m => m.description).FirstOrDefaultAsync();
                segRec.health_effect_rating_description = await _contextEHS.health_effect_rating.Where(m => m.value == segRec.health_effect_rating).Select(m => m.description).FirstOrDefaultAsync();
                segRec.created_date = DateTime.Now;
                _contextEHS.Add(segRec);
                await _contextEHS.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            getDropdownSelectionLists();
            return View(viewModel);
        }

        // GET: seg_risk_assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var seg_risk_assessments = await _contextEHS.seg_risk_assessment.FindAsync(id);
            if (seg_risk_assessments == null)
                return NotFound();

            SegViewModel segViewModel = new SegViewModel();
            segViewModel.seg_risk_assessment = seg_risk_assessments;
            segViewModel.DurationDays = seg_risk_assessments.duration_of_task.Days;
            segViewModel.DurationHours = seg_risk_assessments.duration_of_task.Hours;
            segViewModel.DurationMinutes = seg_risk_assessments.duration_of_task.Minutes;
            segViewModel.DurationSeconds = seg_risk_assessments.duration_of_task.Seconds;
            segViewModel.Username = _username;

            getDropdownSelectionLists();
            ViewBag.Agents = GetAgentByExposureTypeList(seg_risk_assessments.exposure_type, seg_risk_assessments.agent);

            return View(segViewModel);
        }

        // POST: seg_risk_assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("id,location,exposure_type,agent,seg_role,task,oel,acute_chronic,route_of_entry,frequency_of_task,duration_of_task,monitoring_data_required,controls_recommended,exposure_levels_acceptable,date_conducted,assessment_methods_used,seg_number_of_workers,has_agent_been_changed,person_performing_assessment_username,created_user,created_user_fullname,created_user_email,created_date,deleted_user,deleted_user_fullname,deleted_user_email,deleted_date")]*/ SegViewModel viewModel)
        {
            if (id != viewModel.seg_risk_assessment.id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
                    if (employee == null)
                        return RedirectToAction(nameof(Index));

                    viewModel.seg_risk_assessment.modified_user = _username;
                    viewModel.seg_risk_assessment.modified_user_fullname = employee.displayname;
                    viewModel.seg_risk_assessment.modified_user_email = employee.mail;
                    viewModel.seg_risk_assessment.modified_date = DateTime.Now;
                    viewModel.seg_risk_assessment.person_performing_assessment_displayname = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == viewModel.seg_risk_assessment.person_performing_assessment_username).Select(m => m.displayname).FirstOrDefaultAsync();
                    viewModel.seg_risk_assessment.duration_of_task = viewModel.DurationOfTask ?? TimeSpan.Zero;
                    viewModel.seg_risk_assessment.exposure_rating_description = await _contextEHS.exposure_rating.Where(m => m.value == viewModel.seg_risk_assessment.exposure_rating).Select(m => m.description).FirstOrDefaultAsync();
                    viewModel.seg_risk_assessment.health_effect_rating_description = await _contextEHS.health_effect_rating.Where(m => m.value == viewModel.seg_risk_assessment.health_effect_rating).Select(m => m.description).FirstOrDefaultAsync();

                    _contextEHS.Update(viewModel.seg_risk_assessment);
                    await _contextEHS.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!seg_risk_assessmentsExists(viewModel.seg_risk_assessment.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            getDropdownSelectionLists();
            ViewBag.Agents = GetAgentByExposureTypeList(viewModel.seg_risk_assessment.exposure_type, viewModel.seg_risk_assessment.agent);
            return View(viewModel);
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
            var seg_risk_assessment = await _contextEHS.seg_risk_assessment.FindAsync(id);
            if (seg_risk_assessment == null)
                return NotFound();

            __mst_employee employee = await _contextMOC.__mst_employee.Where(m => m.onpremisessamaccountname == _username).FirstOrDefaultAsync();
            if (employee == null)
                return RedirectToAction(nameof(Index));

            seg_risk_assessment.deleted_user = _username;
            seg_risk_assessment.deleted_user_fullname = employee.displayname;
            seg_risk_assessment.deleted_user_email = employee.mail;
            seg_risk_assessment.deleted_date = DateTime.Now;
            _contextEHS.Update(seg_risk_assessment);
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

            var seg = await _contextEHS.seg_risk_assessment.FirstOrDefaultAsync(m => m.id == id);
            if (seg == null)
                return RedirectToAction("Index");

            //// make sure the file being uploaded is an allowable file extension type....
            //var extensionType = Path.GetExtension(fileAttachment.FileName);
            //var found = _contextEHS.AllowedAttachmentExtensions
            //    .Where(m => m.ExtensionName == extensionType)
            //    .Any();

            //if (!found)
            //    return RedirectToAction("Details", new { id = id, tab = "AttachmentsPtn", fileAttachmentError = "File extension type '" + extensionType + "' not allowed. Contact PTN Admin to add, or change document to allowable type." });

            // attachment storage file path should already exist, but just make sure....
            DirectoryInfo path = new DirectoryInfo(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg.id.ToString()));
            if (!Directory.Exists(Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg.id.ToString())))
                path.Create();

            string filePath = Path.Combine(Initialization.AttachmentDirectory_IH_SEG, seg.id.ToString(), fileAttachment.FileName);

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
            return RedirectToAction("Attachments", new { id = id });
        }

        public async Task<IActionResult> DownloadFile(int id, string sourcePath, string fileName)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourcePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }

        public async Task<IActionResult> DeleteFile(int id, string sourcePath, string fileName)
        {
            System.IO.File.Delete(sourcePath);
            return RedirectToAction("Attachments", new { id = id });
        }

        [HttpGet]
        public JsonResult GetAgentByExposureType(string exposureType)
        {
            if (exposureType == "Chemical")
            {
                var agents = _contextEHS.ih_chemical
                    .Select(s => new {description = s.PreferredName})
                    .OrderBy(m => m.description)
                    .ToList();
                return Json(agents);
            }
            else
            {
                var agents = _contextEHS.agent
                    .Where(s => s.exposure_type == exposureType && s.deleted_date == null && s.display == true)
                    .Select(s => new { s.description })
                    .OrderBy(s => s.description)
                    .ToList();
                return Json(agents);
            }            
        }

        public List<SelectListItem> GetAgentByExposureTypeList(string exposureType, string agentIn)
        {
            List<SelectListItem> agents = new List<SelectListItem>();

            if (exposureType == "Chemical")
            {
                // Get list of chemicals and add them to the agent list also....
                var chemicalList = _contextEHS.ih_chemical.OrderBy(m => m.PreferredName).ToList();

                foreach (var agent in chemicalList)
                {
                    SelectListItem item = new SelectListItem { Value = agent.PreferredName, Text = agent.PreferredName };
                    if (agent.PreferredName == agentIn)
                        item.Selected = true;
                    agents.Add(item);
                }
            }
            else
            {
                // Create all agents setup (should no longer be chemicals in here, they are in a seperate table)...
                var agentList = _contextEHS.agent
                    .Where(m => m.deleted_date == null && m.display == true)
                    .OrderBy(m => m.sort_order)
                    .ThenBy(m => m.description)
                    .ToList();
                
                foreach (var agent in agentList)
                {
                    SelectListItem item = new SelectListItem { Value = agent.description, Text = agent.description };
                    if (agent.description == agentIn)
                        item.Selected = true;
                    agents.Add(item);
                }
            }
            return agents;
        }

        public static void WriteToEventLog(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            string source = "EHSApp";
            string logName = "Application";

            try
            {
                if (EventLog.SourceExists(source))
                {
                    EventLog.WriteEntry(source, message, type);
                }
                else
                {
                    // Optionally write a fallback somewhere
                }
            }
            catch (Exception ex)
            {
                // Optional: write to a fallback text file
                System.IO.File.AppendAllText("C:\\Logs\\ehs_fallback_log.txt", $"[{DateTime.Now:u}] ERROR: {ex.Message}\nOriginal: {message}\n");
            }
        }

    }
}
