using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Diagnostics;
using System.Linq;


namespace SocialMediaApplication.Controllers
{
    public class HomeController : Controller
    {


        private readonly ILogger<HomeController> _logger;

//         public HomeController(ILogger<HomeController> logger)
//         {
//             _logger = logger;
//         }

<<<<<<< HEAD
//         public IActionResult Index()
//         {
//             return View();
//         }
=======
        public IActionResult Index()
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Home", "Account");
            }
            return View();
        }
>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)

//         public IActionResult Privacy()
//         {
//             return View();
//         }

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
