using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TowNews.WebUI.Models;

namespace TowNews.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public  IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View("Error");
            }
        }
    }
}