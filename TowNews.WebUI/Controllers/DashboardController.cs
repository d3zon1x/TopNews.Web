using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(DeleteUserDTO modal)
        {
            var result = await _userService.DeleteUserAsync(modal.Id);
            if (result.Success)
            {
                return RedirectToAction(nameof(GetAll));
            }
            ViewBag.CreateUserError = result.Errors.Count() > 0 ? ((IdentityError)result.Errors.First()).Description : result.Message;
            return View();
        }
    }
}
