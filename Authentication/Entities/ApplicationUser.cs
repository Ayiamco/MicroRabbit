using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int ProfileId { get; set; }
        public string RefreshToken { get; set; }
        public Guid LaundryId { get; set; }
    }
}
