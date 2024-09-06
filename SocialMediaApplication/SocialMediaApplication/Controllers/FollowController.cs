using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Models;
using System.Collections.Generic;

namespace SocialMediaApplication.Controllers
{
    public class FollowController : Controller
    {
        // Hardcoded Follows List
        public IActionResult FollowsList()
        {
            var followsList = new FollowsList
            {
                MyFollows = new List<Follows>

                {
                    // new Follows { FollowsId = "1", FollowsName = "Alice", FollowsAvatar = "alice.jpg" },
                    // new Follows { FollowsId = "2", FollowsName = "Bob", FollowsAvatar = "bob.jpg" }
                    new Follows { FollowsId = "1", FollowsName = "Alice" },
                    new Follows { FollowsId = "2", FollowsName = "Bob" }
                }
            };
            
            return View("FollowsPage",followsList);  // This will render FollowsPage.cshtml
        }

        // Hardcoded Followers List
        public IActionResult FollowersList()
        {
            var followersList = new FollowersList
            {
                MyFollowers = new List<Follower>
                {
                    // new Follower { FollowerId = "1", FollowerName = "Charlie", FollowerAvatar = "charlie.jpg" },
                    // new Follower { FollowerId = "2", FollowerName = "Dave", FollowerAvatar = "dave.jpg" }
                    new Follower { FollowerId = "1", FollowerName = "Charlie" },
                    new Follower { FollowerId = "2", FollowerName = "Dave" }
                }
            };
            
            return View("followerPage",followersList);  // This will render FollowerPage.cshtml
        }
    }
}


//  <img src="~/avatars/@follower.FollowerAvatar" alt="Avatar" class="rounded-circle" width="50" height="50" /> 
// <!-- <img src="~/avatars/@follows.FollowsAvatar" alt="Avatar" class="rounded-circle" width="50" height="50" /> -->