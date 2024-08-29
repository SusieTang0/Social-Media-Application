using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Data;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Diagnostics.Metrics;

namespace SocialMediaApplication.Controllers
{
    public class UserPageController:Controller
    {
      
     
        public IActionResult Index()
        {
            var userId = 1;
            var posts = GetPostlists(userId,2,);
            ViewBag.Users = ApplicationData.Users;
            ViewBag.User = ApplicationData.Users.FirstOrDefault(x => x.Id == userId);
            return View(posts);
        }

        public IActionResult GetComments(int postId)
        {
            var comments = ApplicationData.Comments
                                   .Where(c => c.PostId == postId)
                                   .OrderBy(c => c.CreatedTime) // Optional: Order by timestamp
                                   .ToList();

            return PartialView("_CommentsPartial", comments);
        }

        public User GetUserFromList(int userId)
        {
             var user =  ApplicationData.Users.FirstOrDefault(x => x.Id == userId);
             return user;
        }


        public PostList GetPostlists(int id,int numberToShow,string listName){

            var thePosts = new PostList();
            thePosts.MyPosts = new List<Post>();
            thePosts.MyFollowedPosts = new List<Post>();
  
            
            if (listName.Equals("My Posts"))
            {

            }

                    
            
            
            
            return thePosts;
        }

        public List<Post> FindPostList(int id, int numberToShow, string listName)
        {
            var posts = new List<Post>();
            var count = 0;

           

            if (ApplicationData.Posts != null)
            {
                for (int i = ApplicationData.Posts.Count - 1; i > 0; i--)
                {
                    var post = ApplicationData.Posts[i];
                    if (listName.Equals("My Posts"))
                    {
                        if (post.AuthorId == id)
                        {
                            posts.Add(post);
                            count++;
                        }
                    }
                    else
                    {
                        if (post.AuthorId == id)
                        {
                            posts.Add(post);
                            count++;
                        }
                    }
                    if(count == numberToShow)
                    {
                        break;
                    }
                    
                }
            }
            return posts;
        }

        public List<User> GetFollow(int id,string type)
        {
            var follows = new List<int>();
            if (ApplicationData.Follows != null)
            {
                foreach (var follow in ApplicationData.Follows)
                {
                    if (follow.FollowedId.Equals(id))
                    {

                    }
                    if (follow.FollowerId == id)
                    {
                        follows.Add(follow.FollowedId);
                    }
                }
            }
        }


    }
}
