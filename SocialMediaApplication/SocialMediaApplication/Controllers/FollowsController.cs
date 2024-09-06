using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace SocialMediaApplication.Controllers
{
    public class FollowController : Controller
    {
        private readonly PostService _postService;

        public FollowController(PostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string id, string type)
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null || id == userId)
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(userId);
                ViewBag.IsOwner = true;
            }
            else
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(id);
                ViewBag.IsOwner = false;
            }
            var Follows = new List<User>();
            if (type.Equals("followings"))
            {
                Follows = await _postService.GetFollowedsUserAsync(userId);
            }
            else if (type.Equals("followers"))
            {
                Follows = await _postService.GetFollowersUserAsync(userId);
            }

            ViewBag.Follows = await _postService.GetFollowedIdsSetAsync(userId);
            ViewBag.User = await _postService.GetUserProfileAsync(userId);

            return View(Follows);
        }

        public async Task<IActionResult> GoToFollowings(string id)
        {
            return View("Index", new { Id = id, type = "followings" });
        }

        public async Task<IActionResult> GoToFollowers(string id)
        {
            return View("Index", new { Id = id, type = "followers" });
        }


        [HttpPost("following")]
        public async Task<IActionResult> Following(string ownerId, string userId)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(userId))
            {
                // Log details or add additional debugging here
                return BadRequest("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.AddFollowAsync(ownerId, userId);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest(ex.Message);
            }

            return RedirectToAction("Index", new { id = ownerId });
        }


        [HttpPost("unfollowing")]
        public async Task<IActionResult> Unfollowing(string ownerId, string userId)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(userId))
            {
                // Log details or add additional debugging here
                return BadRequest("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.DeleteFollowAsync(ownerId, userId);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest(ex.Message);
            }

            return RedirectToAction("Index", new { id = ownerId });
        }
    }
}