using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;

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
            await _firebaseService.AddComment(postId, authorId, authorName, content);
            return RedirectToAction("Index", "UserPage");
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(string postId, string commentId, string content)
        {
            await _firebaseService.EditComment(postId, commentId, content);
            return RedirectToAction("Index", "UserPage");
        }

        public async Task<IActionResult> DeleteComment(string postId, string commentId)
        {
            await _firebaseService.DeleteComment(postId, commentId);
            return RedirectToAction("Index", "UserPage");
        }

        public async Task<IActionResult> GetComments(string postId)
        {
            var comments = await _firebaseService.GetComments(postId);
            return PartialView("_CommentsPartial", comments);
        }
    }

}