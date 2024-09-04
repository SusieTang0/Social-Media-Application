using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(string Id)
        {
            string userId = HttpContext.Session.GetString("userId");
          
           
            ViewBag.User = await _postService.GetUserProfileAsync(Id);
            ViewBag.IsOwner = Id == userId;
          
            var posts = await GetPostlistsAsync(userId);
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.Owner = await _postService.GetUserProfileAsync(userId);

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

            User thisUser = await _postService.GetUserProfileAsync(userId);
            await _postService.AddPost(userId, content, thisUser.Name, thisUser.ProfilePictureUrl);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(string postId, string userId, string content)
        {
            var existingPost = await _postService.GetPostByPostIdAsync(postId);

            if (existingPost == null)
            {
                return NotFound();
            }

            var updatedPost = new Post
            {
                Id = postId,
                Content = content,
                AuthorId = existingPost.AuthorId,
                AuthorName = existingPost.AuthorName,
                AuthorAvatar = existingPost.AuthorAvatar,
                CreatedTime = DateTime.Now
            };
            await _postService.SavePostAsync(postId, updatedPost);

            return RedirectToAction("Index");
        }

        public async Task<PostList> GetPostlistsAsync(string id)
        {
            var thePosts = new PostList
            {
                MyPosts = await FindPostListAsync(id, 5),
                MyFollowedPosts = await FindFollowedPostsAsync(id, 5)
            };

            return thePosts;
        }

        public async Task<List<Post>> FindPostListAsync(string id, int numberToShow)
        {
            var posts = new List<Post>();
            var allPosts = await _postService.GetPostsAsync();

            if (allPosts != null)
            {
                posts = allPosts
                    .Where(post => post.AuthorId == id)
                    .OrderByDescending(post => post.CreatedTime)
                    .Take(numberToShow)
                    .ToList();
            }
            posts.Reverse();
            return posts;
        }

        public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
        {
            var posts = new List<Post>();
            var followedUsers = await _postService.GetFollowedIdsAsync(userId);
            var followedIds = new HashSet<string>(followedUsers.Select(f => f.FollowedId));
            var allPosts = await _postService.GetPostsAsync();

            if (allPosts != null && followedIds.Count > 0)
            {
                posts = allPosts
                    .Where(post => followedIds.Contains(post.AuthorId))
                    .OrderByDescending(post => post.CreatedTime)
                    .Take(numberToShow)
                    .ToList();
            }
            posts.Reverse();
            return posts;
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
