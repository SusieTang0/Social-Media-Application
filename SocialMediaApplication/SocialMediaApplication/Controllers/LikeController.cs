using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;

namespace SocialMediaApplication.Controllers
{

    public class LikeController : Controller
    {
        private readonly FirebaseService2 _firebaseService;

         public LikeController(FirebaseService2 firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(string postId){
            string userId = HttpContext.Session.GetString("userId");
            var user = await _firebaseService.GetUserProfileAsync(userId);
            string userName = user.Name;
            await _firebaseService.LikePost(postId, userId, userName);
            return RedirectToAction("Index", "UserPage");
        }

         [HttpPost]
        public async Task<IActionResult> UnlikePost(string postId){
            string userId = HttpContext.Session.GetString("userId");
            await _firebaseService.UnlikePost(postId, userId);
            return RedirectToAction("PostDetails",new {postId});
        }

        [HttpPost]
        public async Task<IActionResult> LikeComment(string postId, string commentId){
            string userId = HttpContext.Session.GetString("userId");
            var user = await _firebaseService.GetUserProfileAsync(userId);
            string userName = user.Name;
            await _firebaseService.LikeComment(postId, commentId, userId,userName);
            return RedirectToAction("PostDetails",new {postId});
        }

        [HttpPost]
        public async Task<IActionResult> UnlikeComment(string postId, string commentId){
            string userId = HttpContext.Session.GetString("userId");
            await _firebaseService.UnlikeComment(postId, commentId, userId);
            return RedirectToAction("PostDetails",new {postId});
        }
    }

}