﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopNews.Core.DTOS.User;
using TopNews.Core.Entities.User;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;
using TopNews.WebUI.Models.ViewModels;

namespace TopNews.WebUI.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserService _userService;
        public DashboardController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]// GET
        public IActionResult Login()
        {
            var user =  HttpContext.User.Identity.IsAuthenticated;
            if (user)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [AllowAnonymous]// POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            var validator = new UserLoginValidation();
            var validationResult = validator.Validate(model);
            if (validationResult.IsValid)
            {
                ServiceResponse result = await _userService.LoginUserAsync(model);
                if(result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.AuthError = result.Message;
                return View(model);
            }
            ViewBag.AuthError = validationResult.Errors[0];
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.SingOutUserAsync();
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> GetAll()
        {
            var result  = await _userService.GetAllAsync();
            return View(result.Payload);
        }
        public async Task<IActionResult> Profile(string Id)
        {
            var result = await _userService.GetUserByIdAsync(Id);
            if (result.Success)
            {
                UpdateProfileVM profile = new UpdateProfileVM()
                {
                    UserInfo = (UpdateUserDTO)result.Payload
                };
                return View(profile);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UpdatePasswordDTO pass)
        {
            var validator = new UpdatePasswordVilidation();
            var validationReuslt = await validator.ValidateAsync(pass);
            if (validationReuslt.IsValid)
            {
                var result = await _userService.ChangePasswordAsync(pass.Id, pass.OldPassword, pass.NewPassword);
                if (result.Success)
                {
                    return View(result.Payload); 
                }

                ViewBag.UpdatePasswordError = result.Payload;
                return View();
            }
            ViewBag.UpdatePasswordError = validationReuslt.Errors[0];
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO user)
        {
            var validator = new UpdateUserValidation();
            var validationReuslt = await validator.ValidateAsync(user);
            if (validationReuslt.IsValid)
            {
                //var result = await _userService.ChangePasswordAsync(pass.Id, pass.OldPassword, pass.NewPassword);
                //if (result.Success)
                //{
                //    return View(result.Payload);
                //}

                //ViewBag.UpdatePasswordError = result.Payload;
                //return View();
            }
            ViewBag.UpdatePasswordError = validationReuslt.Errors[0];

            var result = user.FirstName;

            return View();

        }
        public async Task<IActionResult> AddNew()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNew(CreateUserDTO model)
        {
            var validator = new CreateUserValidation();
            var validationReuslt = await validator.ValidateAsync(model);
            if (validationReuslt.IsValid)
            {
                var result = await _userService.AddNewUserAsync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(GetAll));
                }
                else
                {
                    ViewBag.CreateUserError = result.Payload;
                    return View();
                }
            }
            ViewBag.CreateUserValidationError = validationReuslt.Errors[0];
            return View();
        }
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return View(result.Payload);
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Success)
            {
                return RedirectToAction(nameof(GetAll));
            }
            ViewBag.CreateUserError = result.Errors.Count() > 0 ? ((IdentityError)result.Errors.First()).Description : result.Message;
            return RedirectToAction(nameof(GetAll));
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _userService.ConfirmEmailAsync(userId, token);
            return RedirectToAction(nameof(Login));
        }
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userService.ForgotPasswordAsync(email);
            if (result.Success)
            {
                ViewBag.AuthError = "We send you email to reset the password.";
                return View(nameof(Login));
            }
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.AuthError = "New password must be equel to confirmed password";
                return View();
            }
            var result = await _userService.ResetPasswordAsync(model);
            ViewBag.AuthError = result.Message;
            return View(nameof(Login));
        }
        private async Task LoadRoles()
        {
            var result = await _userService.GetAllRoles();
            /*@ViewBag.RoleList = new SelectList((System.Collections.IEnumerable)result, nameof(IdentityRole.Id),nameof(IdentityRole.Name));*/
            @ViewBag.RoleList = result;
        }
        public async Task<IActionResult> Edit(string id)
        {
            await LoadRoles();
            var user = await _userService.GetUserForEditAsync(id);
            if (user.Success)
            {
                return View(user.Payload);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserDTO model)
        {
            await LoadRoles();
            return View();
        }
    }
}
