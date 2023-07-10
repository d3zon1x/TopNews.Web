using Microsoft.AspNetCore.Mvc;

namespace TopNews.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
