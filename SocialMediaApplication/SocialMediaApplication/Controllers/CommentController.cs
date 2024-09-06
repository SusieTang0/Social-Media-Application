using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;
using Firebase.Auth;

namespace SocialMediaApplication.Controllers
{            
    public class CommentController : Controller
    {
        private readonly FirebaseService2 _firebaseService;

        public CommentController(FirebaseService2 firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, string content)
        {
            string authorId = HttpContext.Session.GetString("userId");
            var author = await _firebaseService.GetUserProfileAsync(authorId);
            string authorName = author.Name;
            string authorAvatar = author.ProfilePictureUrl;
            await _firebaseService.AddComment(postId, authorId, authorName, authorAvatar, content);
           
            return RedirectToAction("Index", "UserPage", new { Id = authorId });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(string postId, string commentId, string content)
        {
            string authorId = HttpContext.Session.GetString("userId");
            await _firebaseService.EditComment(postId, commentId, content);
            string page = ViewBag.page;
            
            return RedirectToAction("Index", "UserPage", new { Id = authorId });
        }

        public async Task<IActionResult> DeleteComment(string postId, string commentId)
        {
            string authorId = HttpContext.Session.GetString("userId");
            await _firebaseService.DeleteComment(postId, commentId);
            string page = ViewBag.page;
           
            return RedirectToAction("Index", "UserPage", new { Id = authorId });
        }

        public async Task<IActionResult> GetComments(string postId)
        {
            var comments = await _firebaseService.GetComments(postId);
            string page = ViewBag.page;
            
            return PartialView("_CommentsPartial", comments);
        }
    }

}