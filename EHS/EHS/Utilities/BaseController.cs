using EHS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EHS.Utilities
{
    public class BaseController : Controller
    {
        private readonly EHSContext _contextEHS;
        private readonly MOCContext _contextMOC;


        private string? userName { get; set; }
        private string? userDisplayName { get; set; }
        private bool? isAuthorized { get; set; }
        private bool? isAdmin { get; set; }

        public BaseController(EHSContext contextEHS, MOCContext contextMOC)
        {
            _contextMOC = contextMOC;
            _contextEHS = contextEHS;
        }

        public BaseController()
        {
            var blah = "";
        }
        public bool _isAdmin
        {
            get
            {
                if (isAdmin == null)
                {
                    var found = true; //_context.Administrators
                    //    .Where(m => m.Username == userName)    // User Name logged in matches one in the database
                    //    .Any();

                    if (found)
                    {
                        isAdmin = true;
                        return true;
                    }
                    else
                    {
                        isAdmin = false;
                        return false;
                    }
                }
                else
                    if (isAdmin == true)
                    return true;
                else
                    return false;
            }
        }

        //public string _username =>
        //    User?.Identity?.Name != null
        //        ? User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1)
        //        : Environment.UserName;

        public string _username
        {
            get
            {
                if (userName == null)
                {
                    userName = User.Identity.Name != null ? User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1) : Environment.UserName;
                    return userName;
                }
                else
                    return userName;
            }
        }

        public string _userDisplayName
        {
            get
            {
                if (userDisplayName == null)
                {
                    userDisplayName = _contextMOC.__mst_employee
                        .Where(m => m.onpremisessamaccountname == _username)
                        .Where(m => m.accountenabled == true)
                        .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                        .Select(m => m.displayname)
                        .FirstOrDefault();
                }
                return String.IsNullOrWhiteSpace(userDisplayName) ? userName : userDisplayName;
            }
        }



        public List<SelectListItem> getUserList(string username = null)
        {
            // Create Dropdown List of Users...
            var userList = _contextMOC.__mst_employee
                .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                .Where(m => m.accountenabled == true)
                .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                .OrderBy(m => m.displayname)
                .ThenBy(m => m.onpremisessamaccountname)
                .ToList();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (var user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.onpremisessamaccountname, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };
                if (user.onpremisessamaccountname == username)
                    item.Selected = true;
                users.Add(item);
            }
            return users;
        }

        public List<SelectListItem> getUserEmailList(List<string> emailLists = null)
        {
            // Create Dropdown List of Users...
            var userList = _contextMOC.__mst_employee
                .Where(m => !String.IsNullOrWhiteSpace(m.onpremisessamaccountname))
                .Where(m => m.accountenabled == true)
                .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                .Where(m => !String.IsNullOrWhiteSpace(m.manager) || !String.IsNullOrWhiteSpace(m.jobtitle))
                .OrderBy(m => m.displayname)
                .ThenBy(m => m.onpremisessamaccountname)
                .ToList();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (var user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.mail, Text = user.displayname + " (" + user.onpremisessamaccountname + ")" };
                if (emailLists != null)
                    if (emailLists.Contains(user.mail))
                        item.Selected = true;
                users.Add(item);
            }
            return users;
        }

        public void getDropdownSelectionLists()
        {
            ViewBag.Employees = getUserList();
            ViewBag.Locations = getLocations();
            ViewBag.ExposureTypes = getExposureTypes();
            ViewBag.Agents = getAgents();
            ViewBag.SegRoles = getSegRoles();
            ViewBag.Tasks = getTasks();
            ViewBag.OccupationalExposureLimits = getOccupationalExposureLimits();
            ViewBag.AcuteChronic = getAcuteChronic();
            ViewBag.RouteOfEntry = getRouteOfEntry();
            ViewBag.FrequencyOfTask = getFrequencyOfTask();
            ViewBag.MonitoringDataRequired = getMonitoringDataRequired();
            ViewBag.ControlsRecommended = getControlsRecommended();
            ViewBag.YesNo = getYesNo();
            ViewBag.AssessmentMethodsUsed = getAssessmentMethodsUsed();
            ViewBag.NumberOfWorkers = getNumberOfWorkers();
            ViewBag.HasAgentBeenChanged = getHasAgentBeenChanged();
            ViewBag.ExposureRatings = getExposureRatings();
            ViewBag.HealthEffectRatings = getHealthEffectRatings();
        }

        public List<SelectListItem> getLocations(string? locationIn = null)
        {
            // Create Dropdown List of Users...
            var locationList = _contextEHS.location
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> locations = new List<SelectListItem>();
            foreach (var location in locationList)
            {
                SelectListItem item = new SelectListItem { Value = location.description, Text = location.description };
                if (location.description == locationIn)
                    item.Selected = true;
                locations.Add(item);
            }
            return locations;
        }

        public List<SelectListItem> getExposureTypes(string? exposureTypeIn = null)
        {
            // Create Dropdown List of Users...
            var exposureTypeList = _contextEHS.exposure_type
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> exposureTypes = new List<SelectListItem>();
            foreach (var exposureType in exposureTypeList)
            {
                SelectListItem item = new SelectListItem { Value = exposureType.description, Text = exposureType.description };
                if (exposureType.description == exposureTypeIn)
                    item.Selected = true;
                exposureTypes.Add(item);
            }
            return exposureTypes;
        }

        public List<SelectListItem> getAgents(string? agentIn = null)
        {
            // Create all agents setup (should no longer be chemicals in here, they are in a seperate table)...
            var agentList = _contextEHS.agent
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();

            // Get list of chemicals and add them to the agent list also....
            var chemicalList = _contextEHS.ih_chemical.OrderBy(m=>m.PreferredName).ToList();

            List<SelectListItem> agents = new List<SelectListItem>();
            foreach (var agent in agentList)
            {
                SelectListItem item = new SelectListItem { Value = agent.description, Text = agent.description };
                if (agent.description == agentIn)
                    item.Selected = true;
                agents.Add(item);
            }
            foreach (var agent in chemicalList)
            {
                SelectListItem item = new SelectListItem { Value = agent.PreferredName, Text = agent.PreferredName };
                if (agent.PreferredName == agentIn)
                    item.Selected = true;
                agents.Add(item);
            }
            return agents;
        }

        public List<SelectListItem> getSegRoles(string? segRoleIn = null)
        {
            // Create Dropdown List of Users...
            var segRoleList = _contextEHS.seg_role
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> segRoles = new List<SelectListItem>();
            foreach (var segRole in segRoleList)
            {
                SelectListItem item = new SelectListItem { Value = segRole.description, Text = segRole.description };
                if (segRole.description == segRoleIn)
                    item.Selected = true;
                segRoles.Add(item);
            }
            return segRoles;
        }

        public List<SelectListItem> getTasks(string? taskIn = null)
        {
            // Create Dropdown List of Users...
            var taskList = _contextEHS.task
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var task in taskList)
            {
                SelectListItem item = new SelectListItem { Value = task.description, Text = task.description };
                if (task.description == taskIn)
                    item.Selected = true;
                tasks.Add(item);
            }
            return tasks;
        }

        public List<SelectListItem> getOccupationalExposureLimits(string? occupationalExposureLimitIn = null)
        {
            // Create Dropdown List of Users...
            var occupationalExposureLimitList = _contextEHS.occupational_exposure_limit
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> occupationalExposureLimits = new List<SelectListItem>();
            foreach (var occupationalExposureLimit in occupationalExposureLimitList)
            {
                SelectListItem item = new SelectListItem { Value = occupationalExposureLimit.description, Text = occupationalExposureLimit.description };
                if (occupationalExposureLimit.description == occupationalExposureLimitIn)
                    item.Selected = true;
                occupationalExposureLimits.Add(item);
            }
            return occupationalExposureLimits;
        }

        public List<SelectListItem> getAcuteChronic(string? acuteChronicIn = null)
        {
            // Create Dropdown List of Users...
            var acuteChronicList = _contextEHS.acute_chronic
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> acuteChronics = new List<SelectListItem>();
            foreach (var acuteChronic in acuteChronicList)
            {
                SelectListItem item = new SelectListItem { Value = acuteChronic.description, Text = acuteChronic.description };
                if (acuteChronic.description == acuteChronicIn)
                    item.Selected = true;
                acuteChronics.Add(item);
            }
            return acuteChronics;
        }

        public List<SelectListItem> getRouteOfEntry(string? routeOfEntryIn = null)
        {
            // Create Dropdown List of Users...
            var routeOfEntryList = _contextEHS.route_of_entry
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> routeOfEntrys = new List<SelectListItem>();
            foreach (var routeOfEntry in routeOfEntryList)
            {
                SelectListItem item = new SelectListItem { Value = routeOfEntry.description, Text = routeOfEntry.description };
                if (routeOfEntry.description == routeOfEntryIn)
                    item.Selected = true;
                routeOfEntrys.Add(item);
            }
            return routeOfEntrys;
        }

        public List<SelectListItem> getFrequencyOfTask(string? frequencyOfTaskIn = null)
        {
            // Create Dropdown List of Users...
            var frequencyOfTaskList = _contextEHS.frequency_of_task
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> frequencyOfTasks = new List<SelectListItem>();
            foreach (var frequencyOfTask in frequencyOfTaskList)
            {
                SelectListItem item = new SelectListItem { Value = frequencyOfTask.description, Text = frequencyOfTask.description };
                if (frequencyOfTask.description == frequencyOfTaskIn)
                    item.Selected = true;
                frequencyOfTasks.Add(item);
            }
            return frequencyOfTasks;
        }

        public List<SelectListItem> getMonitoringDataRequired(string? monitoringDataRequiredIn = null)
        {
            // Create Dropdown List of Users...
            var monitoringDataRequiredList = _contextEHS.monitoring_data_required
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> monitoringDataRequireds = new List<SelectListItem>();
            foreach (var monitoringDataRequired in monitoringDataRequiredList)
            {
                SelectListItem item = new SelectListItem { Value = monitoringDataRequired.description, Text = monitoringDataRequired.description };
                if (monitoringDataRequired.description == monitoringDataRequiredIn)
                    item.Selected = true;
                monitoringDataRequireds.Add(item);
            }
            return monitoringDataRequireds;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public List<SelectListItem> getControlsRecommended(string? controlsRecommendedIn = null)
        {
            // Create Dropdown List of Users...
            var controlsRecommendedList = _contextEHS.controls_recommended
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> controlsRecommendeds = new List<SelectListItem>();
            foreach (var controlsRecommended in controlsRecommendedList)
            {
                SelectListItem item = new SelectListItem { Value = controlsRecommended.description, Text = controlsRecommended.description };
                if (controlsRecommended.description == controlsRecommendedIn)
                    item.Selected = true;
                controlsRecommendeds.Add(item);
            }
            return controlsRecommendeds;
        }

        public List<SelectListItem> getYesNo(string? yesNoIn = null)
        {
            // Create Dropdown List of Users...
            var yesNoList = _contextEHS.yes_no
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> yesNos = new List<SelectListItem>();
            foreach (var yesNo in yesNoList)
            {
                SelectListItem item = new SelectListItem { Value = yesNo.description, Text = yesNo.description };
                if (yesNo.description == yesNoIn)
                    item.Selected = true;
                yesNos.Add(item);
            }
            return yesNos;
        }

        public List<SelectListItem> getAssessmentMethodsUsed(string? assessmentMethodsUsedIn = null)
        {
            // Create Dropdown List of Users...
            var assessmentMethodUsedList = _contextEHS.assessment_methods_used
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> assessmentMethodUseds = new List<SelectListItem>();
            foreach (var assessmentMethodUsed in assessmentMethodUsedList)
            {
                SelectListItem item = new SelectListItem { Value = assessmentMethodUsed.description, Text = assessmentMethodUsed.description };
                if (assessmentMethodUsed.description == assessmentMethodsUsedIn)
                    item.Selected = true;
                assessmentMethodUseds.Add(item);
            }
            return assessmentMethodUseds;
        }

        public List<SelectListItem> getNumberOfWorkers(string? numberOfWorkersIn = null)
        {
            // Create Dropdown List of Users...
            var numberOfWorkersList = _contextEHS.number_of_workers
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> numberOfWorkerss = new List<SelectListItem>();
            foreach (var numberOfWorkers in numberOfWorkersList)
            {
                SelectListItem item = new SelectListItem { Value = numberOfWorkers.description, Text = numberOfWorkers.description };
                if (numberOfWorkers.description == numberOfWorkersIn)
                    item.Selected = true;
                numberOfWorkerss.Add(item);
            }
            return numberOfWorkerss;
        }

        public List<SelectListItem> getHasAgentBeenChanged(string? agentBeenChangedIn = null)
        {
            // Create Dropdown List of Users...
            var hasAgentBeenChangedList = _contextEHS.has_agent_been_changed
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> hasAgentBeenChangeds = new List<SelectListItem>();
            foreach (var hasAgentBeenChanged in hasAgentBeenChangedList)
            {
                SelectListItem item = new SelectListItem { Value = hasAgentBeenChanged.description, Text = hasAgentBeenChanged.description };
                if (hasAgentBeenChanged.description == agentBeenChangedIn)
                    item.Selected = true;
                hasAgentBeenChangeds.Add(item);
            }
            return hasAgentBeenChangeds;
        }

        public List<SelectListItem> getExposureRatings(string? exposureRatingIn = null)
        {
            // Create Dropdown List of Users...
            var exposureRatingList = _contextEHS.exposure_rating
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> exposureRatings = new List<SelectListItem>();
            foreach (var exposureRating in exposureRatingList)
            {
                SelectListItem item = new SelectListItem { Value = exposureRating.value.ToString(), Text = exposureRating.value.ToString() + " - " + exposureRating.description };
                if (exposureRating.value.ToString() == exposureRatingIn)
                    item.Selected = true;
                exposureRatings.Add(item);
            }
            return exposureRatings;
        }

        public List<SelectListItem> getHealthEffectRatings(string? healthEffectRatingIn = null)
        {
            // Create Dropdown List of Users...
            var healthEffectRatingList = _contextEHS.health_effect_rating
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> healthEffectRatings = new List<SelectListItem>();
            foreach (var healthEffectRating in healthEffectRatingList)
            {
                SelectListItem item = new SelectListItem { Value = healthEffectRating.value.ToString(), Text = healthEffectRating.value.ToString() + " - " + healthEffectRating.description };
                if (healthEffectRating.value.ToString() == healthEffectRatingIn)
                    item.Selected = true;
                healthEffectRatings.Add(item);
            }
            return healthEffectRatings;
        }

    }
}
