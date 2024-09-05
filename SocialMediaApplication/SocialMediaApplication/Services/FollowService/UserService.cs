using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SocialMediaApplication.Services.FollowService
{
    public class UserService : IUserService
    {
        private readonly List<UserFollow> _users;
        private UserFollow _currentUser;

        public UserService()
        {
           // hardcode
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var userId3 = Guid.NewGuid();
            var userId4 = Guid.NewGuid();

            _users = new List<UserFollow>
            {
                new UserFollow { UserId = userId1, UserName = "Alice", AvatarUrl = "/avatars/female1.jpg", Following = new List<UserFollow>(), Followers = new List<UserFollow>() },
                new UserFollow { UserId = userId2, UserName = "Bob", AvatarUrl = "/avatars/male1.jpg", Following = new List<UserFollow>(), Followers = new List<UserFollow>() },
                new UserFollow { UserId = userId3, UserName = "Charlie", AvatarUrl = "/avatars/male2.jpg", Following = new List<UserFollow>(), Followers = new List<UserFollow>() },
                new UserFollow { UserId = userId4, UserName = "Diana", AvatarUrl = "/avatars/female2.jpg", Following = new List<UserFollow>(), Followers = new List<UserFollow>() }
            };

           // Set the current user (for example purposes)
            _currentUser = _users.First();
            _currentUser.Following.Add(_users[1]); // Example: current user follows Bob
            _users[1].Followers.Add(_currentUser); // Bob is followed by current user

            // Add additional followers for testing
            _users[2].Following.Add(_currentUser); // Charlie follows current user
            _currentUser.Followers.Add(_users[2]); // Current user is followed by Charlie
        }

         public async Task<UserFollow> GetCurrentUserAsync()
        {
            return await Task.FromResult(_currentUser);
        }

        public async Task<List<UserFollow>> GetUsersToFollowAsync()
        {
            var usersToFollow = _users
                .Where(u => u.UserId != _currentUser.UserId && !_currentUser.Following.Any(f => f.FollowingId == u.UserId))
                .ToList();

            return await Task.FromResult(usersToFollow);
        }


        public async Task<List<UserFollow>> GetFollowersAsync(Guid userId)
        {
            // var user = _users.FirstOrDefault(u => u.Id == userId);
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            return await Task.FromResult(user?.Followers ?? new List<UserFollow>());
        }

        public async Task<List<UserFollow>> GetFollowingAsync(Guid userId)
        {
            // var user = _users.FirstOrDefault(u => u.Id == userId);
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            return await Task.FromResult(user?.Following ?? new List<UserFollow>());
        }

        public async Task ToggleFollowAsync(Guid currentUserId, Guid targetUserId)
        {
            // var currentUser = _users.FirstOrDefault(u => u.Id == currentUserId);
                var currentUser = _users.FirstOrDefault(u => u.UserId == currentUserId);

            // var targetUser = _users.FirstOrDefault(u => u.Id == targetUserId);
            var targetUser = _users.FirstOrDefault(u => u.UserId == targetUserId);

            if (currentUser == null || targetUser == null || currentUserId == targetUserId)
                return;

            var followingEntry = currentUser.Following.FirstOrDefault(f => f.FollowingId == targetUserId);
            if (followingEntry != null)
            {
                // Unfollow logic
                currentUser.Following.Remove(followingEntry);
                var followerEntry = targetUser.Followers.FirstOrDefault(f => f.FollowerId == currentUserId);
                targetUser.Followers.Remove(followerEntry);
            }
            else
            {
                // Follow logic
               currentUser.Following.Add(new UserFollow
                {
                    FollowerId = currentUser.UserId,
                    FollowerName = currentUser.UserName,
                    FollowerAvatar = currentUser.AvatarUrl,
                    FollowingId = targetUser.UserId,
                    FollowingName = targetUser.UserName,
                    FollowingAvatar = targetUser.AvatarUrl,
                    FollowedOn = DateTime.UtcNow
                });

                targetUser.Followers.Add(new UserFollow
                {
                    FollowerId = currentUser.UserId,
                    FollowerName = currentUser.UserName,
                    FollowerAvatar = currentUser.AvatarUrl,
                    FollowingId = targetUser.UserId,
                    FollowingName = targetUser.UserName,
                    FollowingAvatar = targetUser.AvatarUrl,
                    FollowedOn = DateTime.UtcNow
                });
            }

            await Task.CompletedTask;
        }

       
    }
}

