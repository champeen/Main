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
    }
}
