using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;

namespace SocialMediaApplication.Controllers
{
    [Route("OtherPage")]
    public class OtherPagesController : Controller
    {
        private readonly PostService _postService;

        public OtherPagesController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            string userId = HttpContext.Session.GetString("userId");
            ViewBag.IsOwner = false;
            ViewBag.OtherFollows = await _postService.GetFollowedIdsSetAsync(id);
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.Owner = await _postService.GetUserProfileAsync(id);
            ViewBag.User = await _postService.GetUserProfileAsync(id);
            var posts = await _postService.GetPostlistsAsync(userId);
            return View(posts);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("following")]
        public async Task<IActionResult> Following(string ownerId, string userId)
        {
            await _postService.AddFollowAsync(ownerId, userId);
            return RedirectToAction("Index", new { id = ownerId });
        }

        [HttpPost("unfollowing")]
        public async Task<IActionResult> Unfollowing(string ownerId, string userId)
        {
            await _postService.DeleteFollowingAsync(ownerId, userId);
            return RedirectToAction("Index", new { id = ownerId });
        }

       
    }

}
