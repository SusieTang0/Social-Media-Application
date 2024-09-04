using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;

namespace SocialMediaApplication.ViewComponents
{            
    public class LikeViewComponent : ViewComponent
    {
        private readonly FirebaseService2 _firebaseService;

         public LikeViewComponent(FirebaseService2 firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string postId)
        {
            var likes = await _firebaseService.ShowPostLikes(postId);
            var likesCount = await _firebaseService.ShowPostLikesCount(postId);
            var model = new LikeViewModel
            {
                Likes = likes,
                LikesCount = likesCount
            };
            return View(model);
        }

        public class LikeViewModel
        {
            public List<Like> Likes {get;set;}
            public int LikesCount {get;set;}
        }
    }
}