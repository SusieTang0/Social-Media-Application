using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.ViewComponents
{
    public class FollowingViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public FollowingViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var following = await _userService.GetFollowingAsync(userId);
            return View(following);
        }
    }
}
