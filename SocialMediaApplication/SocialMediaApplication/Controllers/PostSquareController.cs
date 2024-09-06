using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;

namespace SocialMediaApplication.Controllers
{
    public class PostSquareController : Controller
    {
        private readonly PostService _postService;
        private readonly FirebaseService2 _firebaseService;

        public PostSquareController(PostService postService, FirebaseService2 firebaseService)
        {
            _postService = postService;
            _firebaseService = firebaseService;
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
            ViewBag.Page = "PostSquare";
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


       
        [HttpPost("square-following")]
        public async Task<IActionResult> Following(string ownerId, string userId)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(userId))
            {
                // Log details or add additional debugging here
                throw new ArgumentException("Owner ID and User ID cannot be null or empty.");
            }

            try
            {
                await _postService.AddFollowAsync(ownerId, userId);
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

            return RedirectToAction("Index", new { id = ownerId });
        }

        [HttpPost("square-unfollowing")]
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
       
        [HttpPost]
        public async Task<IActionResult> LikePost(string postId)
        {
            string userId = HttpContext.Session.GetString("userId");
            var user = await _firebaseService.GetUserProfileAsync(userId);
            string userName = user.Name;
            await _firebaseService.LikePost(postId, userId, userName);
            string page = ViewBag.page;

            return RedirectToAction("Index", "UserPage");
        }

        [HttpPost]
        public async Task<IActionResult> UnlikePost(string postId)
        {
            string userId = HttpContext.Session.GetString("userId");
            await _firebaseService.UnlikePost(postId, userId);
            string page = ViewBag.page;

            return RedirectToAction("Index", "UserPage");
        }

        [HttpPost]
        public async Task<IActionResult> LikeComment(string postId, string commentId)
        {
            string userId = HttpContext.Session.GetString("userId");
            var user = await _firebaseService.GetUserProfileAsync(userId);
            string userName = user.Name;
            await _firebaseService.LikeComment(postId, commentId, userId, userName);
            string page = ViewBag.page;

            return RedirectToAction("Index", "UserPage");
        }

        [HttpPost]
        public async Task<IActionResult> UnlikeComment(string postId, string commentId)
        {
            string userId = HttpContext.Session.GetString("userId");
            await _firebaseService.UnlikeComment(postId, commentId, userId);
            string page = ViewBag.page;

            return RedirectToAction("Index", "UserPage");
        }
    }
}
