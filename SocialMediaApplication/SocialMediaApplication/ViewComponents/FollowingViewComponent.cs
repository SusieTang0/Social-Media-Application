// displaying follower/following lists on the UI.
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services.FollowService;
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

        public async Task<IViewComponentResult> InvokeAsync(Guid userId)
        {
            var following = await _userService.GetFollowingAsync(userId);
            if (following == null)
                {
                    following = new List<UserFollow>(); // Ensure the list is never null
                }
                return View(following);
                        return View(following);
                    }
                }
}
