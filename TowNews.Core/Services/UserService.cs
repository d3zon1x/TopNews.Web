using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;
using TopNews.Core.Entities.User;

namespace TopNews.Core.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
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
        public async Task<ServiceResponse> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
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
                Message = "Error to change password"
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
                    Message = "Something went wrong :( ."
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

            var roles = await _userManager.GetRolesAsync(user);
            var mappedUser = _mapper.Map<AppUser, UpdateUserDTO>(user);
            mappedUser.Role = roles[0];

            return new ServiceResponse
            {
                Success = true,
                Message = "User succesfully.",
                Payload = mappedUser
            };
        }
    }
}
