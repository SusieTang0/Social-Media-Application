<<<<<<< HEAD
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
=======
﻿using Microsoft.AspNetCore.Mvc;
>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApplication.Controllers
{
    public class UserPageController:Controller
    {
        private readonly PostService _postService;

        public UserPageController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            PostList posts = await GetPostlistsAsync(userId);
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.User = await _postService.GetUserProfileAsync(userId);
           
            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(string content)
        {
            string userId = HttpContext.Session.GetString("userId");
            User thisUser = await _postService.GetUserProfileAsync(userId);

            if (!string.IsNullOrEmpty(userId))
            {
                await _postService.AddPost(userId, content, thisUser.Name, thisUser.ProfilePictureUrl);
            }
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

            var updatedPost = new SocialMediaApplication.Models.Post
            {
                Id = postId,
                Content = content,
                AuthorId = existingPost.AuthorId,
                AuthorName = existingPost.AuthorName,
                AuthorAvatar = existingPost.AuthorAvatar,
                CreatedTime = DateTime.UtcNow
            };
            await _postService.SavePostAsync(postId, updatedPost);
            return RedirectToAction("Index");
        }


        public async Task<PostList> GetPostlistsAsync(string id)
        {
            var thePosts = new PostList
            {
                MyPosts = await FindPostListAsync(id, 2), 
                MyFollowedPosts = await FindFollowedPostsAsync(id, 2) 
            };
            Console.WriteLine(thePosts);
            return thePosts;
        }

<<<<<<< HEAD
       

       

        [HttpPost]
        public async Task<IActionResult> Logout()
=======
        /*public async Task<List<Post>> FindPostListAsync(string id, int numberToShow)
>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)
        {
            var posts = new List<Post>();

            var thePosts = await _postService.GetPostsAsync();

<<<<<<< HEAD
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
=======
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
>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)
    }
}

