// displaying follower/following lists on the UI.
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialMediaApplication.Services.FollowService;
using System;

namespace SocialMediaApplication.ViewComponents
{
    public class FollowerViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public FollowerViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid userId) 
        {
            var followers = await _userService.GetFollowersAsync(userId);
            if (followers == null)
                {
                    followers = new List<UserFollow>(); // Ensure the list is never null
                }
                return View(followers);                 
        }
    }
}
