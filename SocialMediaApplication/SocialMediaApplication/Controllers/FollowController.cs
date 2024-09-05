//  responsible for handling follow/unfollow actions
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services.FollowService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMediaApplication.Controllers
{
    public class FollowController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public FollowController(IUserService userService, IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> FollowPage()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewBag.RequestVerificationToken = tokens.RequestToken;

            var currentUser = await _userService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return NotFound(); // or some other appropriate action
            }

            var model = new FollowViewModel
            {
                CurrentUser = currentUser,
                Users = await _userService.GetUsersToFollowAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Follow()
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var usersToFollow = await _userService.GetUsersToFollowAsync();

            var model = new FollowViewModel
            {
                CurrentUser = currentUser,
                Users = usersToFollow
            };
            return View("FollowPage", model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFollow(Guid targetUserId)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Current user not found" });
                }

                // Replace `Id` with `UserId` in the ToggleFollowAsync call
                await _userService.ToggleFollowAsync(currentUser.UserId, targetUserId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}

