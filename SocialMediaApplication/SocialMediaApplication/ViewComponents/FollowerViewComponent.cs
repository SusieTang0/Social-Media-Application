using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.ViewComponents
{
    public class FollowerViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public FollowerViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var followers = await _userService.GetFollowersAsync(userId);
            return View(followers);
        }
    }
}