using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SocialMediaApplication.Services.FollowService
{
    public interface IUserService
    {
        Task<UserFollow> GetCurrentUserAsync();  
        Task<List<UserFollow>> GetUsersToFollowAsync();
        Task<List<UserFollow>> GetFollowersAsync(Guid userId);
        Task<List<UserFollow>> GetFollowingAsync(Guid userId);
        Task ToggleFollowAsync(Guid currentUserId, Guid targetUserId);
    }
}
