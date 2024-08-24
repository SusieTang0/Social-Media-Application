using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Diagnostics;
using System.Linq;


namespace SocialMediaApplication.Controllers
{
    public class HomeController : Controller
    {

        private static User _currentUser = new User
        {
            Id = "currentUser",
            Name = "John Doe",
            Followers = new List<User>(),
            Following = new List<User>()
        };

        private static List<User> _users = new List<User>
        {
            new User { Id = "user1", Name = "Alice" },
            new User { Id = "user2", Name = "Bob" },
            new User { Id = "user3", Name = "Charlie" }
        };


        public IActionResult Follow()
        {
            var model = new FollowViewModel
            {
                CurrentUser = _currentUser,
                Users = _users
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ToggleFollow(string userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                if (_currentUser.Following.Contains(user))
                {
                    _currentUser.Following.Remove(user);
                    user.Followers.Remove(_currentUser);
                }
                else
                {
                    _currentUser.Following.Add(user);
                    user.Followers.Add(_currentUser);
                }
            }

            return Content("Success");
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
    }
}
