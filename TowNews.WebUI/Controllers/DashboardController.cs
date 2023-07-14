using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOS.User;
using TopNews.Core.Entities.User;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;

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
    }
}
