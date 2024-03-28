using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using PtnWaiver.Data;
using PtnWaiver.Models;
using PtnWaiver.ViewModels;

namespace PtnWaiver.Controllers
{
    public class BaseController : Controller
    {
        private readonly PtnWaiverContext _contextPtnWaiver;
        private readonly MocContext _contextMoc;

        private string? userName { get; set; }
        private string? userDisplayName { get; set; }
        private bool? isAuthorized { get; set; }
        private bool? isAdmin { get; set; }

        public BaseController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc, WebApplicationBuilder builder)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }
        public BaseController(PtnWaiverContext contextPtnWaiver, MocContext contextMoc)
        {
            _contextPtnWaiver = contextPtnWaiver;
            _contextMoc = contextMoc;
        }
        public BaseController()
        {

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

        public int _getDaysSince1900
        {
            get
            {
                DateTime beginDate = new DateTime(1900, 01, 01);
                DateTime today = DateTime.Now;
                TimeSpan ts = today - beginDate;
                int differenceInDays = ts.Days;
                int subtract38352 = differenceInDays - 38352;
                return subtract38352;
            }
        }
        public string _userDisplayName
        {
            get
            {
                if (userDisplayName == null)
                {
                    userDisplayName = _contextMoc.__mst_employee
                        .Where(m => m.onpremisessamaccountname == _username)
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
                    var found = _contextMoc.__mst_employee
                        .Where(m => m.onpremisessamaccountname == _username)    // User Name logged in matches one in the database
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
                    var found = _contextPtnWaiver.Administrators
                        .Where(m => m.Username == _username)    // User Name logged in matches one in the database
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
            var userList = _contextMoc.__mst_employee
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

        public List<SelectListItem> getPtnStatus()
        {
            var statusList = _contextPtnWaiver.PtnStatus.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> status = new List<SelectListItem>();
            foreach (var rec in statusList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Status, Text = rec.Description };
                status.Add(item);
            }
            return status;
        }

        public List<SelectListItem> getWaiverStatus()
        {
            var statusList = _contextPtnWaiver.WaiverStatus.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> status = new List<SelectListItem>();
            foreach (var rec in statusList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Status, Text = rec.Description };
                status.Add(item);
            }
            return status;
        }

        public List<SelectListItem> getPtns()
        {
            var statusList = _contextPtnWaiver.PTN.OrderBy(m => m.DocId).ToList();
            List<SelectListItem> status = new List<SelectListItem>();
            foreach (var rec in statusList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Id.ToString(), Text = rec.DocId };
                status.Add(item);
            }
            return status;
        }

        public List<SelectListItem> getStatusFilter(string value = null)
        {
            var statusList = _contextPtnWaiver.PtnStatus.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "All", Value = "All", Selected = (value == "All" ? true : false) });
            foreach (var rec in statusList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Status, Text = rec.Description, Selected = (value == rec.Status ? true : false) };
                status.Add(item);
            }
            return status;
        }

        public List<SelectListItem> getOriginatingGroups()
        {
            var originatingGroupList = _contextPtnWaiver.OriginatingGroup.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> originatingGroups = new List<SelectListItem>();
            foreach (var rec in originatingGroupList)
            {
                if (rec.BouleSizeRequired)
                    originatingGroups.Add(new SelectListItem { Value = rec.Code, Text = rec.Description + " (Boule Size is Required)" });
                else
                    originatingGroups.Add(new SelectListItem { Value = rec.Code, Text = rec.Description });
            }
            return originatingGroups;
        }
        public List<SelectListItem> getBouleSizes()
        {
            var bouleSizeList = _contextPtnWaiver.BouleSize.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> bouleSizes = new List<SelectListItem>();
            foreach (var rec in bouleSizeList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Code, Text = rec.Description };
                bouleSizes.Add(item);
            }
            return bouleSizes;
        }

        public List<SelectListItem> getSubjectTypes()
        {
            var subjectTypeList = _contextPtnWaiver.SubjectType.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> subjectTypes = new List<SelectListItem>();
            foreach (var rec in subjectTypeList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Code, Text = rec.Description };
                subjectTypes.Add(item);
            }
            return subjectTypes;
        }

        public List<SelectListItem> getGroupApprovers()
        {
            var groupList = _contextPtnWaiver.GroupApprovers.OrderBy(m => m.Order).ThenBy(m => m.Group).ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var rec in groupList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Group, Text = rec.Group + " - (" + rec.PrimaryApproverFullName + "/" + rec.SecondaryApproverFullName + ")" };
                groups.Add(item);
            }
            return groups;
        }
        public List<SelectListItem> getPorProjects()
        {
            var groupList = _contextPtnWaiver.PorProject.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var rec in groupList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Code, Text = rec.Description };
                groups.Add(item);
            }
            return groups;
        }
        public List<SelectListItem> getProductProcess()
        {
            var groupList = _contextPtnWaiver.ProductProcess.OrderBy(m => m.Order).ThenBy(m => m.Description).ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var rec in groupList)
            {
                SelectListItem item = new SelectListItem { Value = rec.Code, Text = rec.Description };
                groups.Add(item);
            }
            return groups;
        }

        //public List<SelectListItem> getChangeLevels()
        //{
        //    var changeLevelList = _context.ChangeLevel.OrderBy(m => m.Order).ThenBy(m => m.Level).ToList();
        //    List<SelectListItem> changeLevels = new List<SelectListItem>();
        //    foreach (var changeLevel in changeLevelList)
        //    {
        //        string text = changeLevel.Level + "\u00A0\u00A0\u00A0 : \u00A0\u00A0\u00A0" + changeLevel.Description;
        //        SelectListItem item = new SelectListItem { Value = changeLevel.Level, Text = text.Substring(0, text.Length > 200 ? 200 : text.Length) };
        //        changeLevels.Add(item);
        //    }
        //    return changeLevels;
        //}

        public EmailHistory AddEmailHistory(string? priority, string? subject, string? body, string? sentToDisplayName, string? sentToUsername, string? sentToEmail, int? ptnId, int? waiverId, int? taskId, string? type, string? status, DateTime createdDate, string? username)
        {
            EmailHistory emailHistory = new EmailHistory
            {
                Priority = priority,
                Subject = subject,
                Body = body,
                SentToDisplayName = sentToDisplayName,
                SentToUsername = sentToUsername,
                SentToEmail = sentToEmail,
                PtnId = ptnId,
                WaiverId = waiverId,
                TaskId = taskId,
                Type = type,
                Status = status,
                CreatedDate = createdDate,
                CreatedUser = username
            };
            _contextPtnWaiver.Add(emailHistory);
            _contextPtnWaiver.SaveChanges();
            return emailHistory;
            //_context.Add(emailHistory);
            //await _context.SaveChangesAsync();
        }

        public __mst_employee getUserInfo(string username = null)
        {
            if (username == null)
                return null;

            return _contextMoc.__mst_employee
            .Where(m => m.onpremisessamaccountname == username)
            .Where(m => m.accountenabled == true)
            .Where(m => !String.IsNullOrWhiteSpace(m.mail))
            //.Select(m => m.displayname)
            .FirstOrDefault();
        }

        public string getSerialNumberBasedOnYear(string year)
        {
            var lastSerialNumberForYear = _contextPtnWaiver.PTN.Where(m => m.OriginatorYear == year).Max(m => m.SerialNumber);
            Int32.TryParse(lastSerialNumberForYear, out int serialNumber);   // returns zero if null value, which is ok
            serialNumber += 1;
            return serialNumber.ToString("000");
        }

        public string getOriginatorInitials()
        {
            var userInfo = getUserInfo(_username);
            var firstInitial = userInfo.givenname == null || userInfo.givenname == "" ? "" : userInfo.givenname.Substring(0, 1);
            var lastInitial = userInfo.surname == null || userInfo.surname == "" ? "" : userInfo.surname.Substring(0, 1);
            return (firstInitial + lastInitial);
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class PreventDuplicateRequestAttribute : ActionFilterAttribute
        {
            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.HttpContext.Request.Form.ContainsKey("__RequestVerificationToken"))
                {
                    await context.HttpContext.Session.LoadAsync();

                    var currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
                    var lastToken = context.HttpContext.Session.GetString("LastProcessedToken");

                    if (lastToken == currentToken)
                    {
                        context.ModelState.AddModelError(string.Empty, "Looks like you accidentally submitted the same form twice.");
                    }
                    else
                    {
                        context.HttpContext.Session.SetString("LastProcessedToken", currentToken);
                        await context.HttpContext.Session.CommitAsync();
                    }
                }

                await next();
            }


        }
    }
}