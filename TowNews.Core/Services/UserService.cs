﻿using AutoMapper;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var result = await _userManager.CreateAsync(mappedUser);
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(mappedUser, user.Role).Wait();
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
                Message = "Something went wrong :( ."
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
                    Message = "something went wrong",
                    Payload = result.Errors 
                };
            }
        }
    }
}
