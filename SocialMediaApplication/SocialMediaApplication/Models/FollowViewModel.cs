// a view model that combines the User model with additional context for the view, such as the current user and a list of users available to follow.

using System.Collections.Generic;
using SocialMediaApplication.Models;
using SocialMediaApplication.Services.FollowService;

namespace SocialMediaApplication.Models
{
    public class FollowViewModel
    {
        // Property for the current user
        public UserFollow CurrentUser { get; set; }

        // Property for the list of users available to follow
       public List<UserFollow> Users { get; set; } // Users to follow (suggestions)
    }
}
