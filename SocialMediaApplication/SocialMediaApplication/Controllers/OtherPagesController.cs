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
          
            ViewBag.IsOwner = false;
            ViewBag.OtherFollows = await _postService.GetFollowedIdsSetAsync(id);
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.Owner = await _postService.GetUserProfileAsync(id);
   
            var posts = await _postService.GetPostlistsAsync(id);
         
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
        public async Task<IActionResult> Following(string followOwnerId, string followUserId)
        {
            if (string.IsNullOrEmpty(followOwnerId) || string.IsNullOrEmpty(followUserId))
            {
                // Log details or add additional debugging here
                throw new ArgumentException("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.AddFollowAsync(followOwnerId, followUserId);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it accordingly
                throw new ApplicationException("Follow error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost("unfollowing")]
        public async Task<IActionResult> Unfollowing(string unfollowOwnerId, string unfollowUserId)
        {
            if (string.IsNullOrEmpty(unfollowOwnerId) || string.IsNullOrEmpty(unfollowUserId))
            {
                // Log details or add additional debugging here
                return BadRequest("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.DeleteFollowAsync(unfollowOwnerId, unfollowUserId);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest(ex.Message);
            }

            return RedirectToAction("Index");
        }


    }

}
