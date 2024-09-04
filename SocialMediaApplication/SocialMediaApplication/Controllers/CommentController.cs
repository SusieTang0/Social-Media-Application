using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;
using Microsoft.AspNetCore.Authorization;

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
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post Id is missing");
            }
            string authorId = HttpContext.Session.GetString("userId");
            await _firebaseService.AddComment(postId, authorId, content);
            return RedirectToAction("Index", "UserPage", new { id = postId });
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