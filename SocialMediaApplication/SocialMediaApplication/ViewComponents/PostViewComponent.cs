using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Data;
using System.Threading.Tasks;
namespace SocialMediaApplication.ViewComponents
{
    public class PostViewComponent :ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = ApplicationData.Posts;
            return View(posts);
        }
    }
}
