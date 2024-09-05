using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Hosting;
using SocialMediaApplication.Models;
using System.ComponentModel.Design;

namespace SocialMediaApplication.Services
{
    public class PostService
    {
        private readonly FirebaseAuthProvider _authProvider;
        private readonly IFirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;

        public PostService(IConfiguration configuration)
        {
            _authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(configuration["Firebase:ApiKey"]));

            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = configuration["Firebase:ApiKey"],
                BasePath = configuration["Firebase:DatabaseURL"]
            };
            _firebaseClient = new FireSharp.FirebaseClient(config);
            _firebaseStorage = new FirebaseStorage(configuration["Firebase:StorageBucket"]);
        }

        /*_______________Posts_______________________*/
        
        //User
        public async Task<Dictionary<string,SocialMediaApplication.Models.User>> GetUsersAsync()
        {
            FirebaseResponse response = await _firebaseClient.GetAsync("users");

 
            var usersDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>>();

            if (usersDictionary != null)
            {
                return usersDictionary;
            }

            return null;
        }

        public async Task<SocialMediaApplication.Models.User> GetUserProfileAsync(string userId)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/profile");
            return response.ResultAs<SocialMediaApplication.Models.User>();
        }

        //Follow
        public async Task<List<SocialMediaApplication.Models.Follow>> GetFollowedIdsAsync(string userId)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/follows");


            var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Follow>>();

            if (followedDictionary != null)
            {
                return followedDictionary.Values.ToList();
            }

            return new List<SocialMediaApplication.Models.Follow>();
        }

        public async Task<List<SocialMediaApplication.Models.Follow>> GetFollowerIdsAsync(string userId)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/followers");


            var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Follow>>();

            if (followedDictionary != null)
            {
                return followedDictionary.Values.ToList();
            }

            return new List<SocialMediaApplication.Models.Follow>();
        }

        // Post

        public async Task AddPost(string content,string authorId, string authorName, string authorAvatar)
        {
            
            var post = new Post
            {
                Content = content,
                AuthorId = authorId,
                AuthorName = authorName,
                AuthorAvatar = authorAvatar,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            if (post == null )
            {
                throw new ArgumentException("Post ID and Post object cannot be null or empty.");
            }


            try
            {
                var response = await _firebaseClient.PushAsync("posts", post);
                string generatedKey = response.Result.name;
                post.Id = generatedKey;
                await _firebaseClient.SetAsync($"posts/{generatedKey}", post);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the post.", ex);
            }

           
        }

        public async Task<FirebaseResponse> SavePostAsync(string id, string content)
        {
            var post = await GetPostByPostIdAsync(id);
            post.Content = content;

            if (post == null || string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Post object or Id cannot be null or empty.");
            }
            return await _firebaseClient.SetAsync($"posts/{id}", post);
        }

        public async Task DeletePostAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("Post ID cannot be null or empty.");
            }

            try
            {

                await _firebaseClient.DeleteAsync($"posts/{postId}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting the post.", ex);
            }
        }

        public async Task<Dictionary<string,SocialMediaApplication.Models.Post>> GetPostsAsync()
        {
            FirebaseResponse response = await _firebaseClient.GetAsync("posts");

            
            var postsDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Post>>();

            if (postsDictionary != null)
            {
                return postsDictionary;
            }

            return null;
        }

        public async Task<List<SocialMediaApplication.Models.Post>> GetAllPostsAsync()
        {
            var posts = new List<Post>();
            var allPosts = await GetPostsAsync();

            if (allPosts != null)
            {
                posts = allPosts
                     .Select(post => new Post
                     {
                         Id = post.Key,
                         AuthorId = post.Value.AuthorId,
                         AuthorName = post.Value.AuthorName,
                         AuthorAvatar = post.Value.AuthorAvatar,
                         Content = post.Value.Content,
                         CreatedTime = post.Value.CreatedTime,
                     })
                     .OrderByDescending(post => post.CreatedTime)
                     .ToList();
            }

            return posts;
        }

        public async Task<SocialMediaApplication.Models.Post> GetPostByPostIdAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return null;
            }

            var allPosts = await GetPostsAsync();

            if (allPosts != null && allPosts.TryGetValue(postId, out var post))
            {
                return post;
            }

            return null;

        }

        public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
        {
            var posts = new List<Post>();
            var followedUsers = await GetFollowedIdsAsync(userId);
            var followedIds = new HashSet<string>(followedUsers.Select(f => f.FollowedId));
            var allPosts = await GetPostsAsync();

            if (allPosts != null && followedIds.Count > 0)
            {
                posts = allPosts
                    .Where(post => followedIds.Contains(userId))
                     .Select(post => new Post
                     {
                         Id = post.Key,
                         AuthorId = post.Value.AuthorId,
                         AuthorName = post.Value.AuthorName,
                         AuthorAvatar = post.Value.AuthorAvatar,
                         Content = post.Value.Content,
                         CreatedTime = post.Value.CreatedTime,
                     })
                     .OrderByDescending(post => post.CreatedTime)
                     .Take(numberToShow)
                     .ToList();
            }

            return posts;
        }


        public async Task<List<Post>> FindPostListAsync(string id, int numberToShow)
        {
            var posts = new List<Post>();
            var allPosts = await GetPostsAsync();

            if (allPosts != null)
            {
                posts = allPosts
                    .Where(kvp => kvp.Value.AuthorId == id)
                     .Select(kvp => new Post
                     {
                         Id = kvp.Key,
                         AuthorId = kvp.Value.AuthorId,
                         AuthorName = kvp.Value.AuthorName,
                         AuthorAvatar = kvp.Value.AuthorAvatar,
                         Content = kvp.Value.Content,
                         CreatedTime = kvp.Value.CreatedTime
                     })
                     .OrderByDescending(post => post.CreatedTime)
                     .Take(numberToShow)
                     .ToList();
            }

            return posts;
        }
    }
}
