using Management_of_Change.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_of_Change.Utilities
{
    public class Initialize : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        public Initialize(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        public void GetUserInfo()
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.UserName = _username;
        }
    }
}
