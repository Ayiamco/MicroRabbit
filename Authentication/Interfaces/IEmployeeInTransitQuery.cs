using Authentication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    
    public interface IEmployeeInTransitQuery : IGenericQueries<EmployeeInTransit, Guid>
    {
    }
}
