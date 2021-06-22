using Authentication.Entities;
using Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Repositories
{
    public class IdentityQuery : IIdentityQuery
    {
        private ApplicationDbContext _context;
        public IdentityQuery(ApplicationDbContext _context)
        {
            this._context = _context;
        }


        public string GetUserRole(ApplicationUser user)
        {
            var userRoles = _context.UserRoles.Where(x => x.UserId == user.Id)
                 .Join(_context.Roles, x => x.RoleId, y => y.Id, (x, y) => y.Name)
                 .AsQueryable();
            return string.Join(",", userRoles);
        }

        public IEnumerable<string> GetLaundryEmployeesEmail(Guid laundryId)
        {
            var employeeEmails = _context.Users.Where(x => x.LaundryId == laundryId).AsQueryable()?
                .ToList().Select(x => x.UserName);
            return employeeEmails;

        }


    }
}
