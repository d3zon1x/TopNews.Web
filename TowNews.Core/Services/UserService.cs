using Microsoft.AspNetCore.Identity;
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

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                SignInResult result = await _signInManager.PasswordSignInAsync(user, model.password, model.rememberMe, lockoutOnFailure: true);
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
    }
}
