using Management_of_Change.Controllers;
using Management_of_Change.Data;
using Management_of_Change.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public BaseController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        public BaseController(ILogger<AdminController> logger)
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

    }
}
