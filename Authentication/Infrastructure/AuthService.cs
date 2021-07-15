using Authentication.dtos;
using Authentication.Entities;
using Authentication.Interfaces;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Laundromat.SharedKernel.Core;
using Microsoft.Extensions.Logging;

namespace Authentication.Infrastructure
{
    public enum AppServiceResult
    {
        Succeeded, Failed, Unknown, Prohibited, TwoFactorEnabled
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly ILogger<AuthService> logger;
        private readonly IJWTManager _jwtmanager;
        private readonly IIdentityQuery _userRepo;
        private readonly IUnitOfWork _unitOFWork;
        private readonly IConfiguration _config;
        private readonly IEmployeeInTransitQuery _employeeInTransitRepo;
        private readonly IMapper mapper;
        private readonly IEmailExchange emailExchange;
        private readonly IMessageBrokerPublisher<NewLaundry> newLaundryMessagePublisher;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,   ILogger<AuthService> logger,
            IJWTManager jwtmanager,
            IIdentityQuery userRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IEmployeeInTransitQuery employeeInTransitRepo,
            IMapper mapper,
            IEmailExchange emailExchange,
            IMessageBrokerPublisher<NewLaundry> messagePublisher
            )
        {
            _userManager = userManager;
            _signManager = signInManager;
            this.logger = logger;
            _jwtmanager = jwtmanager;
            _userRepo = userRepo;
            _unitOFWork = unitOfWork;
            _config = config;
            _employeeInTransitRepo = employeeInTransitRepo;
            this.mapper = mapper;
            this.emailExchange = emailExchange;
            this.newLaundryMessagePublisher=  messagePublisher;
        }

        public async Task<ServiceResponse> CreateLaundry(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Username,
            };
            logger.LogError("this is me testing the error");

            //publish the create laundry message to the broker
            newLaundryMessagePublisher.PublishEvent(new NewLaundry { LaundryName = model.LaundryName,OwnerName= model.OwnerName });

            user.TwoFactorEnabled = true;
            var result = await _userManager.CreateAsync(user, model.Password); ;
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Owner);
                await SendConfirmationEmail(user);

                //TODO : add the laundry id to the response
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = ""
                };
            }

            return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new { errors = GetErrors(result.Errors) })
            };
        }

        public async Task<ServiceResponse> Authenticate(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username);
            var result = await _signManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.RequiresTwoFactor)
            {
                //TODO: send a request to the email service
                //await SendEmail2FAToken(user);

                return new ServiceResponse
                {
                    Result = AppServiceResult.TwoFactorEnabled,
                    Data = JsonConvert.SerializeObject(new
                    {
                        message = "Please check your mail for your authentication code"
                    })
                };
            }

            var response = new ServiceResponse { Result = AppServiceResult.Failed };
            if (user == null)
            {
                response.Data = JsonConvert.SerializeObject(new
                { errors = new { username = new string[] { "user does not exist" } } });
                return response;
            }

            else if (result.IsNotAllowed)
            {
                await SendConfirmationEmail(user);
                response.Data = JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    message = "check your mail for confirmation link",
                    errors = new { account = new string[] { "account is not confirmed" } }
                });
            }

            else if (user != null && !result.IsLockedOut && !result.IsNotAllowed)
            {
                response.Data = JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    errors = new { password = new string[] { "password is incorrect" } },
                    message = "password is incorrect"
                });
            }

            else
            {
                response.Result = AppServiceResult.Unknown;
                response.Data = JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    message = "some server error occured"
                });
            };

            return response;

        }

        public async Task<ServiceResponse> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new
                { errors = new { username = new string[] { "user does not exist" } } }),
            };
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return new ServiceResponse
            {
                Result = AppServiceResult.Succeeded
            };

            return new ServiceResponse
            {
                Result = AppServiceResult.Unknown,
                Data = JsonConvert.SerializeObject(new
                { message = "unknown error occured" }),
            }; ;
        }

        public async Task<ServiceResponse> Login2Factor(string code, string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var result = await _signManager.TwoFactorSignInAsync("Email", code, false, false);
            if (result.Succeeded)
            {

                var userRole = _userRepo.GetUserRole(user);
                var token = _jwtmanager.GetToken(new JWTDto { UserEmail = username, UserId = user.Id, UserRole = userRole });
               
                //TODO : call the laundromatmain service to get laundry details
                //var laundry = await laundryRepo.GetLaundryByUserId(new Guid(user.Id));
                await UpdateRefreshToken(user);

                //TODO: update the laundryname and laundry Id 
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            jwtToken = token,
                            refreshToken = user.RefreshToken,
                            id = user.Id,
                            profileId = user.ProfileId

                        }
                    }),
                };
            }

            return new ServiceResponse { Result = AppServiceResult.Unknown };
        }
        public async Task<ServiceResponse> RefreshJWtToken(JWTDto model)
        {
            try
            {
                var claim = _jwtmanager.GetPrincipalFromExpiredToken(model);
                var user = await _userManager.FindByEmailAsync(claim.Identity.Name);
                if (user.RefreshToken != model.RefreshToken) return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        errors = new
                        {
                            refreshToken = new string[] { "refresh token has being changed, go to login" }
                        },

                    })
                };

                model.UserEmail = user.Email;
                model.UserId = user.Id;
                model.UserRole = RoleNames.Owner;
                var newJwt = _jwtmanager.GetToken(model);
                await UpdateRefreshToken(user);

                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            jwtToken = newJwt,
                            refreshToken = user.RefreshToken
                        }
                    })
                };
            }
            catch (SecurityTokenException)
            {
                return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        errors = new
                        {
                            jwtToken = new string[] { "Invalid jwt token" }
                        },

                    })
                };
            }
            catch
            {
                return new ServiceResponse
                {
                    Result = AppServiceResult.Unknown,
                    Data = JsonConvert.SerializeObject(new
                    {
                        errors = new
                        {
                            serverError = new string[] { "Unknown server error occured" }
                        },

                    })
                };
            }
        }

        public async Task SendResetPasswordLink(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var passwordresetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = _config[AppConstants.ClientBaseUrl] + $"account/confirm-password-reset/?passwordToken={passwordresetToken},username={user.Email}";
            
            //TODO: call mail service to send password rest link
            //await _mailService.SendEmailAsync(new Message(new List<string> { username }, "Password Reset",
            //    $"<h3>Hi<h3>, <p>Please click <a href={url}>here</a>. Link expires in 10 mins"),
            //    IsHTML: true);
        }

        public async Task<ServiceResponse> ResetPassword(ConfirmPasswordResetDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new
                {
                    errors = new
                    {
                        username = new string[] { "User does not exist" }
                    },
                })
            };

            var resetPassResult = await _userManager.ResetPasswordAsync(user, model.PasswordToken, model.Password);
            if (!resetPassResult.Succeeded)
            {
                return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new { errors = GetErrors(resetPassResult.Errors) })
                };
            }

            return new ServiceResponse
            {
                Result = AppServiceResult.Succeeded,
                Data = JsonConvert.SerializeObject(new { message = "Password change was successful." })
            };
        }

        public async Task<ServiceResponse> AddEmployee(NewEmployeeDto model)
        {
            var empInTransit = _employeeInTransitRepo.Find(x => x.Username == model.Username)
                .AsQueryable().SingleOrDefault();

            if (empInTransit == null || empInTransit.Id != model.Id) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new
                {
                    errors = new
                    {
                        Username = new string[] { "user has not being added by laundry owner" }
                    }

                })
            };

            var user = mapper.Map<ApplicationUser>(model);
            user.LaundryId = empInTransit.LaundryId;
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Employee);
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded
                };
            }


            return new ServiceResponse() { Result = AppServiceResult.Failed };
        }

        private async Task UpdateRefreshToken(ApplicationUser user)
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                user.RefreshToken = Convert.ToBase64String(randomNumber);
                await _unitOFWork.SaveAsync();
            }
        }

        private async Task SendEmail2FAToken(ApplicationUser user)
        {
            //if (user.Profile == null) user = _userRepo.GetUserWithNavProps(user.Id);
            //var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            //var message = new Message(new List<string> { user.Email }, $"{user.Laundry.Name} email confirmation",
            //   @$"
            //        <h4>Hi {user.Profile.Name},</h4>
            //        <p>please use {token} as your laundry login OTP, thanks.
            //        </p>
                    
            //    ");

            //await _mailService.SendEmailAsync(message, IsHTML: true);
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_config[AppConstants.ClientBaseUrl]}confirmEmail" +
                $"?confirmationToken={confirmEmailToken},id={user.Id}";

            //TODO : Get user laundry and add the name to the mail being sent.
            var message = new EmailMessage
            {
                To = new List<string> { user.Email },
                Subject = "Registration email confirmation",
                Content =
                @$"
                    <h4>Hi,</h4>
                    <p>please click on this <a href={confirmationLink}>link</a> to confirm your email and complete your laundry 
                        registration, thanks.
                    </p>   
                "
            };
                
            emailExchange.PublishEmail(message);
        }
        private Dictionary<string, string[]> GetErrors(IEnumerable<IdentityError> errors)
        {
            var obj = new Dictionary<string, string[]>();
            foreach (var error in errors)
            {
                if (obj.ContainsKey(error.Code)) obj[error.Code] = (string[])obj[error.Code].Append(error.Description);
                else obj.Add(error.Code, new string[] { error.Description });
            }
            return obj;
        }
    }
}
