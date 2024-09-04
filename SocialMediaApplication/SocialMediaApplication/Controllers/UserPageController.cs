﻿using Microsoft.AspNetCore.Mvc;
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

            if (!string.IsNullOrEmpty(userId))
            {
                await _postService.AddPost(userId, content);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePost(string postId, string userId, string content)
        {
            
            Post post = new SocialMediaApplication.Models.Post
            {
                Id = postId,
                Content = content,
                AuthorId = userId,
                CreatedTime = DateTime.Now,
            };
            await _postService.SavePostAsync(postId, post);
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

        public async Task<List<Post>> FindPostListAsync(string id, int numberToShow)
        {
            var posts = new List<Post>();

            var thePosts = await _postService.GetPostsAsync();

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
        }


        public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
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
    }
}

