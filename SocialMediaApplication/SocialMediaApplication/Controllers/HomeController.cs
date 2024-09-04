using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Diagnostics;
using System.Linq;


namespace SocialMediaApplication.Controllers
{
    public class HomeController : Controller
    {


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            return View("Index", "UserPage");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SignUp()
        {
            return View("./Views/Account/Register.cshtml");
        }

        public IActionResult Login()
        {
            return View("./Views/Account/Login.cshtml");
        }


    }
}
