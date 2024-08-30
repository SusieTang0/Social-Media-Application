
using SocialMediaApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMediaApplication.Services
{
    public interface IUserService
    {
        Task<List<User>> GetFollowersAsync(int userId);
        Task<List<User>> GetFollowingAsync(int userId);
        // Other methods related to user operations
    }
}
