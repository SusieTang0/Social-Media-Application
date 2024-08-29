using System.Collections.Generic;

namespace SocialMediaApplication.Models
{
    public class FollowViewModel
    {
        public User CurrentUser { get; set; }
        public List<User> Users { get; set; }
    }

}
