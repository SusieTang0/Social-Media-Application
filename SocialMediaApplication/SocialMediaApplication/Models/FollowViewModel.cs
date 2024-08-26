using System.Collections.Generic;

namespace SocialMediaApplication.Models
{
    public class FollowViewModel
    {
        public required User CurrentUser { get; set; }
        public required List<User> Users { get; set; }
    }

}
