using Management_of_Change.Controllers;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Management_of_Change.Utilities
{
    public class BaseController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        private string? userName { get; set; }
        private string? userDisplayName { get; set; }
        private bool? isAuthorized { get; set; }
        private bool? isAdmin { get; set; }

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
                    userDisplayName = _context.__mst_employee
                        .Where(m => m.onpremisessamaccountname == userName)
                        .Where(m => m.accountenabled == true)
                        .Where(m => !String.IsNullOrWhiteSpace(m.mail))
                        .Select(m => m.displayname)
                        .FirstOrDefault();
                }
                return String.IsNullOrWhiteSpace(userDisplayName) ? userName : userDisplayName;
            }
        }

        public bool _isAuthorized
        {
            get
            {
                if (isAuthorized == null)
                {
                    var found = _context.__mst_employee
                        .Where(m => m.onpremisessamaccountname == userName)    // User Name logged in matches one in the database
                        .Where(m => m.accountenabled == true)                   // Account is enabled
                        .Where(m => !String.IsNullOrWhiteSpace(m.mail))         // There is an email address
                        .Any();

                    if (found)
                    {
                        isAuthorized = true;
                        return true;
                    }

                    else
                    {
                        isAuthorized = false;
                        return false;
                    }
                }
                else
                    if (isAuthorized == true)
                        return true;
                    else
                        return false;
            }
        }

        public bool _isAdmin
        {
            get
            {
                if (isAdmin == null)
                {
                    var found = _context.Administrators
                        .Where(m => m.Username == userName)    // User Name logged in matches one in the database
                        .Any();

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

        //public BaseController()
        //{

        //}
        public BaseController(Management_of_ChangeContext context, WebApplicationBuilder builder)
        {
            _context = context;
        }

        public BaseController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        public BaseController(ILogger<AdminController> logger)
        {
            
        }

        public BaseController()
        {

        }

        public ErrorViewModel CheckAuthorization()
        {
            if (String.IsNullOrWhiteSpace(_username))
                return new ErrorViewModel { Action = "Error", Controller = "Home", ErrorMessage = "Invalid Username: " + _username + ". Contact MoC Admin." };

            if (!_isAuthorized)
                return new ErrorViewModel { Action = "Unauthorized", Controller = "Home", ErrorMessage = "User " + _username + " Unauthorized - Not Setup as Active Employee. Contact MoC Admin." };

            return null;
        }

        public List<SelectListItem> getUserList(string username = null)
        {
            // Create Dropdown List of Users...
            var userList = _context.__mst_employee
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
        
        public List<SelectListItem> getChangeTypes()
        {
            var changeTypeList = _context.ChangeType.OrderBy(m => m.Order).ThenBy(m => m.Type).ToList();
            List<SelectListItem> changeTypes = new List<SelectListItem>();
            foreach (var changeType in changeTypeList)
            {
                string text = changeType.Type + "\u00A0\u00A0\u00A0 : \u00A0\u00A0\u00A0" + changeType.Description;
                SelectListItem item = new SelectListItem { Value = changeType.Type, Text = text.Substring(0, text.Length > 200 ? 200 : text.Length) };
                changeTypes.Add(item);
            }
            return changeTypes;
        }

        public List<SelectListItem> getChangeLevels()
        {
            var changeLevelList = _context.ChangeLevel.OrderBy(m => m.Order).ThenBy(m => m.Level).ToList();
            List<SelectListItem> changeLevels = new List<SelectListItem>();
            foreach (var changeLevel in changeLevelList)
            {
                string text = changeLevel.Level + "\u00A0\u00A0\u00A0 : \u00A0\u00A0\u00A0" + changeLevel.Description;
                SelectListItem item = new SelectListItem { Value = changeLevel.Level, Text = text.Substring(0, text.Length > 200 ? 200 : text.Length) };
                changeLevels.Add(item);
            }
            return changeLevels;
        }

        public List<SelectListItem> getPtnNumbers()
        {
            var ptnList = _context.PTN.Where(m => m.DeletedDate == null && m.Enabled == true).OrderBy(m => m.Order).ThenBy(m => m.Name).ToList();
            List<SelectListItem> ptns = new List<SelectListItem>();
            foreach (var request in ptnList)
            {
                SelectListItem item = new SelectListItem { Value = request.Name, Text = request.Name + " : " + request.Description };
                ptns.Add(item);
            }
            return ptns;
        }


    }
}
