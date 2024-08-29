using Microsoft.AspNetCore.Mvc;

namespace SocialMediaApplication.Controllers
{
    public class UserPageController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }

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
    }
}
