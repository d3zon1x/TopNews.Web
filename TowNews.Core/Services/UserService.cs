using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;
using TopNews.Core.Entities.User;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TopNews.Core.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(RoleManager<IdentityRole> roleManager, IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _roleManager = roleManager;
        }
        public async Task<ServiceResponse> SingOutUserAsync()
        {
            await _signInManager.SignOutAsync();
            return new ServiceResponse
            {
                Success = true,
                Message = "Singed out successfully"
            };
        }
        public async Task<ServiceResponse> ChangePasswordAsync(string id, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "Password has changed successfully"
                    };
                }
                List<IdentityError> errors = result.Errors.ToList();
                string error = "";
                foreach (var err in errors)
                {
                    error = error + err.Description.ToList();
                }
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Errors.",
                    Payload = error
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Error. User null"
            };

        }
        public async Task<ServiceResponse> ChangeUserAsync(UpdateUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "null user."
                };
            }


            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {

                return new ServiceResponse
                {
                    Success = true,
                    Message = "Password has changed successfully"
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Errors to update user"
            };
        }


        public async Task<ServiceResponse> GetAllAsync()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();

            List<UsersDTO> mappedUsers = users.Select(u => _mapper.Map<AppUser, UsersDTO>(u)).ToList();

            foreach (var user in users)
            {
                var index = users.IndexOf(user);
                mappedUsers[index].Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }

            return new ServiceResponse
            {
                Success = true,
                Message = "All users loaded",
                Payload = mappedUsers
            };
        }

        public async Task<ServiceResponse> LoginUserAsync(UserLoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Login or password incorrect!"
                };
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.password, model.rememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, model.rememberMe);
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "Logged in successfully!"
                    };
                }
                if (result.IsNotAllowed)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Confirm your email please."
                    };
                }
                if (result.IsLockedOut)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "User is locked."
                    };
                }
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Something went wrong during login :( ."
                };
            }
        }
        public async Task<ServiceResponse> GetUserByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "user or password incorrect!"
                };
            }

            var mappedUser = _mapper.Map<AppUser, UpdateUserDTO>(user);

            return new ServiceResponse
            {
                Success = true,
                Message = "User succesfully.",
                Payload = mappedUser
            };
        }
        public async Task<ServiceResponse> GetRawUserByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "user or password incorrect!"
                };
            }
            return new ServiceResponse
            {
                Success = true,
                Message = "User succesfully.",
                Payload = user
            };
        }
        public async Task<ServiceResponse> AddNewUserAsync(CreateUserDTO user)
        {
            if (user != null)
            {
                //AppUser mappedUser = User.Select(u => _mapper.Map<AppUser, UsersDTO>(u)).ToList();
                AppUser mappedUser = _mapper.Map<CreateUserDTO, AppUser>(user);
                var result = await _userManager.CreateAsync(mappedUser, user.Password);
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(mappedUser, user.Role).Wait();

                    //  Email sender
                    //await _emailService.SendEmail(user.Email, "Welcome", "Welcome to our site");
                    await SendConfirmationEmailAsync(mappedUser);

                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "New user succesfully added.",
                        Payload = user
                    };
                }
                else
                {
                    List<IdentityError> errors = result.Errors.ToList();
                    string error = "";
                    foreach (var err in errors)
                    {
                        error = error + err.Description.ToList();
                    }
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Errors.",
                        Payload = error
                    };
                }
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Something went wrong during adding user :( ."
            };

        }
        public async Task SendConfirmationEmailAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["HostSetting:URL"]}/Dashboard/ConfirmEmail?userId={user.Id}&token={validEmailToken}";
            string emailBody = $"<h1>Confirm your email please.</h1><a href='{url}'>Confirm now</a>";
            await _emailService.SendEmail(user.Email, "TopNews Email confirmation", emailBody);
        }
        public async Task<ServiceResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "unknown user"
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "User`s email confirmed succesfully"
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "User`s email not confirmed",
                Payload = result.Errors.Select(e=>e.Description)
            };
        }

        public async Task<ServiceResponse> DeleteUserAsync(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "unknown user"
                };
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "User deleted succesfully!"
                };
            }
            else
            {
                return new ServiceResponse {
                    Success = false,
                    Message = "something went wrong during deleting user",
                    Payload = result.Errors .Select(e=>e.Description)
                };
            }
        }
        public async Task<ServiceResponse> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "unknown user"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["HostSetting:URL"]}/Dashboard/ResetPassword?email={email}&token={validEmailToken}";
            string emailBody = $"<h1>Follow the following instructions to reset password.</h1><a href='{url}'>Reset password</a>";
            await _emailService.SendEmail(email, "TopNews Password reset", emailBody);
            return new ServiceResponse
            {
                Success = true,
                Message = "email sent"
            };
        }
        public async Task<ServiceResponse> ResetPasswordAsync(ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "unknown user"
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);

            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Password reseted succesfully!"
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Password didn`t reset :( ." 
            };
        }
        public async Task<List<IdentityRole>> GetAllRoles()
        {
            List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }
        public async Task<ServiceResponse> EditUser(EditUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (user.Email != model.Email)
            {
                user.EmailConfirmed = false;
                user.Email = model.Email;
                user.UserName = model.Email;
                await SendConfirmationEmailAsync(user);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                return new ServiceResponse
                {
                    Message = "User successfully updated.",
                    Success = true
                };
            }

            List<IdentityError> errorList = result.Errors.ToList();
            string errors = "";

            foreach (var error in errorList)
            {
                errors = errors + error.Description.ToString();
            }
            return new ServiceResponse
            {
                Message = errors,
                Success = false
            };
        }
        public async Task<ServiceResponse> GetUserForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "unknown user"
                };
            }

            EditUserDTO mappedUser = _mapper.Map<AppUser, EditUserDTO>(user);

            return new ServiceResponse
            {
                Success = true,
                Message = "User mapped.",
                Payload = mappedUser
            };
        }
    }
}
