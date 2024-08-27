using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Data;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;

namespace SocialMediaApplication.Controllers
{
    public class UserPageController:Controller
    {
      
     
        public IActionResult Index()
        {
            var userId = 1;
            var posts = GetPostlists(userId,2);
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


        public PostList GetPostlists(int Id,int numberToShow){

            var thePosts = new PostList();
            thePosts.MyPosts = new List<Post>();
            thePosts.MyFollowdPosts = new List<Post>();
            var count1 = 0;
            var count2 = 0;
            var follows =new List<int>();
            if (ApplicationData.Follows != null)
            {
                foreach (var follow in ApplicationData.Follows)
                {
                    if (follow.FollowerId == Id)
                    {
                        follows.Add(follow.FollowedId);
                    }
                }
            }

            
            if (ApplicationData.Posts != null)
            {
                for (int i=ApplicationData.Posts.Count-1;i>0;i--)
                {
                    var post = ApplicationData.Posts[i];
                    if (count1 < numberToShow)
                    {
                        if (post.AuthorId == Id)
                        {
                            thePosts.MyPosts.Add(post);
                            count1++;
                        }
                    }

                    if (count2 < numberToShow)
                    {
                        if (follows.Contains(post.AuthorId))
                        {
                            thePosts.MyFollowdPosts.Add(post);
                            count2++;
                        }
                    }

                    if((count1 + count1)== (numberToShow *2))
                    {

                    }
                }
            }
            
            
            return thePosts;
        }



    }
}
