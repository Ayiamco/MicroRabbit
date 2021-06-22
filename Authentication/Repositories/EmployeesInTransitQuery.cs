using Authentication.Entities;
using Authentication.Infrastructure;
using Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Repositories
{
    public class EmployeeInTransitQuery : GenericQueries<EmployeeInTransit, Guid>, IEmployeeInTransitQuery
    {
        public EmployeeInTransitQuery(ApplicationDbContext context) : base(context)
        {

        }
    }
}
