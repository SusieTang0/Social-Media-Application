using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services;
namespace SocialMediaApplication.ViewComponents
{
 public class PostViewComponent :ViewComponent
    {
        private readonly PostService _postService;

        public PostViewComponent(PostService postService)
        {
            _postService = postService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
        var posts = _postService.GetPostsAsync();
            return View(posts);
        }
    }
}

