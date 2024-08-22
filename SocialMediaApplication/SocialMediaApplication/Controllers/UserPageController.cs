using Microsoft.AspNetCore.Mvc;

namespace SocialMediaApplication.Controllers
{
    public class UserPageController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
