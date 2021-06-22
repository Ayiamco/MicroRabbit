using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Infrastructure
{
    public class AppConstants
    {
        public const string ClientBaseUrl = " ClientBaseUrl";
        public const string AuthSigningKey = "AuthSigningKey";


    }


    public class RoleNames
    {
        public const string Admin = "Admin";
        public const string Owner = "Owner";
        public const string Employee = "Employee";
    }
}
