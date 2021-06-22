using Authentication.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse> Authenticate(LoginDto model);
        Task<ServiceResponse> CreateLaundry(RegisterDto model);
        Task<ServiceResponse> RefreshJWtToken(JWTDto model);
        Task SendResetPasswordLink(string username);
        Task<ServiceResponse> ResetPassword(ConfirmPasswordResetDto model);
        Task<ServiceResponse> AddEmployee(NewEmployeeDto model);
        Task<ServiceResponse> Login2Factor(string code, string username);
        Task<ServiceResponse> ConfirmEmail(string token, string userId);
    }
}
