// defines the relationship between two users, capturing details about who follows whom, including relevant IDs, names, avatars, and the timestamp of the follow action.

using System;

namespace SocialMediaApplication.Models
{
    public class UserFollow
    {
      // User details
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }

        // Followers and Following
        public List<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public List<UserFollow> Following { get; set; } = new List<UserFollow>();

        // The ID of the user who is following the current user
        public Guid FollowerId { get; set; }

        // The name of the user who is following
        public string FollowerName { get; set; }

        // The avatar of the user who is following
        public string FollowerAvatar { get; set; }

        // The ID of the user being followed by current user
          public Guid FollowingId { get; set; }

        // The name of the user being followed
        public string FollowingName { get; set; }

        // The avatar of the user being followed
        public string FollowingAvatar { get; set; }

        // Timestamp of when the follow action occurred
        public DateTime FollowedOn { get; set; } = DateTime.UtcNow;
    }
}


