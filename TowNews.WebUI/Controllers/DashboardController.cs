using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOS.User;
using TopNews.Core.Validation.User;

namespace TopNews.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]// GET
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]// POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginDTO model)
        {
            var validator = new UserLoginValidation();
            var validationResult = validator.Validate(model);
            if (validationResult.IsValid)
            {
                return View(model);
            }
            ViewBag.AuthError = validationResult.Errors[0];
            return View(model);
        }
    }
}
