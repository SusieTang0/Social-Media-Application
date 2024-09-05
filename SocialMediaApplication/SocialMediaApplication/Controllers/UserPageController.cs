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
        public async Task<IActionResult> Index(string Id)
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            if(Id == null || Id == userId)
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(userId);
                ViewBag.IsOwner = true;
            }
            else
            {
                ViewBag.Owner = await _postService.GetUserProfileAsync(Id);
                ViewBag.IsOwner = false;
            }
            
          
          
           
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.User = await _postService.GetUserProfileAsync(userId);
            var posts = await GetPostlistsAsync(userId);
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
                // Log the exception and handle errors
                // Logger.LogError(ex, "Error creating post");
                return StatusCode(500, "Internal server error. Please try again later.");
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

        public async Task<PostList> GetPostlistsAsync(string id)
        {
            var thePosts = new PostList
            {
                MyPosts = await _postService.FindPostListAsync(id, 5),
                MyFollowedPosts = new List<Post>(),//await _postService.FindFollowedPostsAsync(id, 5)
            };

            return thePosts;
        }

        /*public async Task<List<Post>> FindPostListAsync(string id, int numberToShow)
       

       

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Sign out the user from the authentication system
            await HttpContext.SignOutAsync();

            if (thePosts != null)
            {
                int count = 0;
                for (int i = thePosts.Count - 1; i >= 0; i--)
                {
                    var post = thePosts[i];

                    if (post.AuthorId == id)
                    {
                        posts.Add(post);
                        count++;
                    }

                    if (count >= numberToShow)
                    {
                        break;
                    }
                }
            }

            return posts;
        }*/
        public async Task<List<Post>> FindPostListAsync(string authorId, int numberToShow)
        {
            var posts = new List<Post>();

            var postsDict = await _postService.GetPostsAsync();

            if (postsDict != null)
            {
                var filteredPosts = postsDict
                    .Where(kvp => kvp.Value.AuthorId == authorId)
                    .Select(kvp => new Post
                    {
                        Id = kvp.Key, 
                        AuthorId = kvp.Value.AuthorId,
                        AuthorName = kvp.Value.AuthorName,
                        AuthorAvatar = kvp.Value.AuthorAvatar,
                        Content = kvp.Value.Content,
                        CreatedTime = kvp.Value.CreatedTime,
                        Comments = kvp.Value.Comments
                    })
                    .OrderByDescending(post => post.CreatedTime) 
                    .Take(numberToShow)  
                    .ToList();

                posts.AddRange(filteredPosts);
            }

            return posts;
        }


        /* public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
         {
             var posts = new List<Post>();
             var followedUsers = await _postService.GetFollowedIdsAsync(userId);
             var followedIds = new HashSet<string>(followedUsers.Select(f => f.FollowedId));
             var allPosts = await _postService.GetPostsAsync();

             if (allPosts != null && followedIds.Count > 0)
             {
                 int count = 0;
                 for (int i = allPosts.Count - 1; i >= 0; i--)
                 {
                     var post = allPosts[i];

                     if (followedIds.Contains(post.AuthorId))
                     {
                         posts.Add(post);
                         count++;
                     }

                     if (count >= numberToShow)
                     {
                         break;
                     }
                 }
             }

             return posts;
         }*/

        public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
        {
            var posts = new List<Post>();

            // Get followed users
            var followedUsers = await _postService.GetFollowedIdsAsync(userId);
            var followedIds = new HashSet<string>(followedUsers.Select(f => f.FollowedId));

            // Get all posts
            var allPostsDict = await _postService.GetPostsAsync();

            if (allPostsDict != null && followedIds.Count > 0)
            {
                // Convert dictionary to list of posts
                var allPosts = allPostsDict
                    .Select(kvp => new Post
                    {
                        Id = kvp.Key,
                        AuthorId = kvp.Value.AuthorId,
                        AuthorName = kvp.Value.AuthorName,
                        AuthorAvatar = kvp.Value.AuthorAvatar,
                        Content = kvp.Value.Content,
                        CreatedTime = kvp.Value.CreatedTime,
                        Comments = kvp.Value.Comments
                    })
                    .Where(post => followedIds.Contains(post.AuthorId))
                    .OrderByDescending(post => post.CreatedTime) // Sort posts by creation time
                    .Take(numberToShow) // Limit number of posts
                    .ToList();

                posts.AddRange(allPosts);
            }

            return posts;
        }
        /*public IActionResult GetComments(int postId)
       {
           var comments = ApplicationData.Comments
                                  .Where(c => c.PostId == postId)
                                  .OrderBy(c => c.CreatedTime) // Optional: Order by timestamp
                                  .ToList();

           return PartialView("_CommentsPartial", comments);
       }

      */
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
