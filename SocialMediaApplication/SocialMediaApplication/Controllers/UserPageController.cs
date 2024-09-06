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
    public class UserPageController : Controller
    {
        private readonly PostService _postService;

        public UserPageController(PostService postService)
        {
            _postService = postService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsOwner = true;
            ViewBag.Owner = await _postService.GetUserProfileAsync(userId);
            ViewBag.Follows = await _postService.GetFollowedIdsSetAsync(userId);
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.User = await _postService.GetUserProfileAsync(userId);

            var posts = await _postService.GetPostlistsAsync(userId);
            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(string content)
        {
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                // Handle empty content case
                ModelState.AddModelError(string.Empty, "Content cannot be empty.");
                return RedirectToAction("Index"); // Redirect to the appropriate action or return a view with error messages
            }

            User thisUser = await _postService.GetUserProfileAsync(userId);
            if (thisUser == null)
            {
                // Handle the case where the user profile cannot be retrieved
                return NotFound("User not found.");
            }

            

            try
            {
                await _postService.AddPost(content, userId, thisUser.Name, thisUser.ProfilePictureUrl);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error. Please try again later." + ex.Message);
            }

            return RedirectToAction("Index", new { Id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(string postId, string content)
        {
            string userId = HttpContext.Session.GetString("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(postId) && string.IsNullOrEmpty(content))
            {
                // Handle the case where postId is null or empty
                return BadRequest("Post ID cannot be null or empty.");
            }


            await _postService.SavePostAsync(postId, content);




            // Redirect to the index action with the correct userId
            return RedirectToAction("Index", new { Id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string id)
        {
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(id))
            {
                // Handle the error or return a view with an error message
                return BadRequest("Post ID cannot be null or empty.");
            }
            
            await _postService.DeletePostAsync(id);

            return RedirectToAction("Index", new { Id = userId });
        }

       
       

       

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Sign out the user from the authentication system
            await HttpContext.SignOutAsync();

            // Redirect to the Home/Index page
            return RedirectToAction("Index", "Home");
        }

        /* Uncomment and implement this if needed
        public IActionResult GetComments(int postId)
        {
            var comments = ApplicationData.Comments
                              .Where(c => c.PostId == postId)
                              .OrderBy(c => c.CreatedTime)
                              .ToList();

            return PartialView("_CommentsPartial", comments);
        }
        */
    }
}
