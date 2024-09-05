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

        public async Task<IActionResult> Index()
        {
            string userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
<<<<<<< HEAD
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
          
            var posts = await _postService.GetAllPostsAsync();
=======
            }*/

            //List<Post> posts = await _postService.GetPostsAsync();
            var postsWithIds = await _postService.GetPostsAsync();
            var posts = postsWithIds.Select(p => new Post
            {
                Id = p.Key,
                AuthorId = p.Value.AuthorId,
                AuthorName = p.Value.AuthorName,
                AuthorAvatar = p.Value.AuthorAvatar,
                Content = p.Value.Content,
                CreatedTime = p.Value.CreatedTime,
                Comments = p.Value.Comments
            }).ToList();

>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)
            ViewBag.Users = await _postService.GetUsersAsync();
            ViewBag.User = await _postService.GetUserProfileAsync(userId);

            return View(posts);
        }
    }
}
