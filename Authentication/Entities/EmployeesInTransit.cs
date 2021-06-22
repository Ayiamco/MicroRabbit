using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Entities
{
    public class EmployeeInTransit
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid LaundryId { get; set; }
    }
}
