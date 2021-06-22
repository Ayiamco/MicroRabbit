using Authentication.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    public interface IJWTManager
    {
        public string GetToken(JWTDto model);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(JWTDto model);
    }
}
