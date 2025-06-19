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

        public List<SelectListItem> getLocations(string locationIn = null)
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

        public List<SelectListItem> getExposureTypes(string exposureTypeIn = null)
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

        public List<SelectListItem> getAgents(string agentIn = null)
        {
            // Create Dropdown List of Users...
            var agentList = _contextEHS.agent
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> agents = new List<SelectListItem>();
            foreach (var agent in agentList)
            {
                SelectListItem item = new SelectListItem { Value = agent.description, Text = agent.description };
                if (agent.description == agentIn)
                    item.Selected = true;
                agents.Add(item);
            }
            return agents;
        }

        public List<SelectListItem> getSegRoles(string segRoleIn = null)
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

        public List<SelectListItem> getTasks(string taskIn = null)
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

        public List<SelectListItem> getOccupationalExposureLimits(string taskIn = null)
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
                if (occupationalExposureLimit.description == taskIn)
                    item.Selected = true;
                occupationalExposureLimits.Add(item);
            }
            return occupationalExposureLimits;
        }

        public List<SelectListItem> getAcuteChronic(string taskIn = null)
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
                if (acuteChronic.description == taskIn)
                    item.Selected = true;
                acuteChronics.Add(item);
            }
            return acuteChronics;
        }

        public List<SelectListItem> getRouteOfEntry(string taskIn = null)
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
                if (routeOfEntry.description == taskIn)
                    item.Selected = true;
                routeOfEntrys.Add(item);
            }
            return routeOfEntrys;
        }

        public List<SelectListItem> getFrequencyOfTask(string taskIn = null)
        {
            // Create Dropdown List of Users...
            var frequencyOfTaskList = _contextEHS.route_of_entry
                .Where(m => m.deleted_date == null && m.display == true)
                .OrderBy(m => m.sort_order)
                .ThenBy(m => m.description)
                .ToList();
            List<SelectListItem> frequencyOfTasks = new List<SelectListItem>();
            foreach (var frequencyOfTask in frequencyOfTaskList)
            {
                SelectListItem item = new SelectListItem { Value = frequencyOfTask.description, Text = frequencyOfTask.description };
                if (frequencyOfTask.description == taskIn)
                    item.Selected = true;
                frequencyOfTasks.Add(item);
            }
            return frequencyOfTasks;
        }

    }
}
