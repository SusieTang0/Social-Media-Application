using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;

namespace SocialMediaApplication.Controllers
{
    public class PostSquareController : Controller
    {
        private readonly PostService _postService;

        public PostSquareController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index(string Id)
        {
            string userId = HttpContext.Session.GetString("userId");
            ViewBag.UserId = userId;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if (Id == null)
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(userId);
                ViewBag.IsOwner = true;
            }
            else
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(Id);
                ViewBag.IsOwner = false;
            }

            var postsWithIds = await _postService.GetPostsAsync();
            var posts = postsWithIds.Select(p => new Post
            {
                Id = p.Key,
                AuthorId = p.Value.AuthorId,
                AuthorName = p.Value.AuthorName,
                AuthorAvatar = p.Value.AuthorAvatar,
                Content = p.Value.Content,
                CreatedTime = p.Value.CreatedTime,
                Comments = p.Value.Comments,
                Likes = p.Value.Likes
            }).ToList();

            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.User = await _postService.GetUserProfileAsync(userId);
            return View(posts);
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


        [HttpPost("following")]
        public async Task Following(string ownerId, string userId)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(userId))
            {
                // Log details or add additional debugging here
                throw new ApplicationException("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.AddFollowAsync(ownerId, userId);
            }
            catch (ArgumentException ex)
            {
                // Log the exception or handle it accordingly
                throw new ApplicationException("Follow error." + ex.Message);
            }

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
                await _postService.DeleteFollowingAsync(ownerId, userId);
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
