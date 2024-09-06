using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Hosting;
using SocialMediaApplication.Models;
using System.Collections.Generic;
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
        public async Task<List<SocialMediaApplication.Models.Follow>> GetFollowsAsync()
        {
            FirebaseResponse response = await _firebaseClient.GetAsync("follows");


            var followsDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Follow>>();

            if (followsDictionary != null)
            {
                return followsDictionary.Values.ToList();
            }

            return new List<Follow>();
        }

      
        public async Task<List<SocialMediaApplication.Models.User>> GetFollowedsUserAsync(string userId)
        {
            var users = await GetUsersAsync();
            var result = new List<SocialMediaApplication.Models.User>();
            var followings = await GetFollowsAsync();
            
            if (users != null && followings != null)
            {
                foreach (var following in followings)
                {
                    if (users.TryGetValue(following.FollowerId, out var user))
                    {
                      result.Add(user);
                    }
                }
                return result;
            }
            return new List<Models.User>();
        }

        public async Task<HashSet<string>> GetFollowedIdsSetAsync(string userId)
        {
            var users = await GetUsersAsync();
            var result = new HashSet<string>();
            var followings = await GetFollowsAsync();
            
            if (users != null && followings != null)
            {
                foreach (var following in followings)
                {
                    if (users.TryGetValue(following.FollowedId, out var user))
                    {
                      result.Add(following.FollowedId);
                    }
                }
                return result;
            }
            return new HashSet<string>();
        }

        public async Task<HashSet<string>> GetFollowerIdsSetAsync(string userId)
        {
            var users = await GetUsersAsync();
            var result = new HashSet<string>();
            var followings = await GetFollowsAsync();

            if (users != null && followings != null)
            {
                foreach (var following in followings)
                {
                    if (users.TryGetValue(following.FollowedId, out var user))
                    {
                        result.Add(following.FollowerId);
                    }
                }
                return result;
            }
            return new HashSet<string>();
        }

        public async Task<List<SocialMediaApplication.Models.User>> GetFollowersUserAsync(string userId)
        {
            var users = await GetUsersAsync();
            var result = new List<SocialMediaApplication.Models.User>();
            var followings = await GetFollowsAsync();
            
            if (users != null && followings != null)
            {
                foreach (var following in followings)
                {
                    if (users.TryGetValue(following.FollowedId, out var user))
                    {
                      result.Add(user);
                    }
                }
                return result;
            }
            return new List<Models.User>();
        }

         public async Task AddFollowAsync(string followingId, string followerId)
        {
          var follow = new Follow
                  {
                      FollowedId = followingId,
                      FollowerId = followerId,
                     
                      CreatedTime = DateTime.Now,
                  };

            if (follow == null )
            {
                throw new ArgumentException("Following Id and Follower Id cannot be null or empty.");
            }


            try
            {
                var response = await _firebaseClient.PushAsync("follows", follow);
                string generatedKey = response.Result.name;
                follow.Id = generatedKey;
                await _firebaseClient.SetAsync($"follows/{generatedKey}", follow);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the follow.", ex);
            }
        }


        public async Task DeleteFollowingAsync(string followingId, string followerId)
        {
            if (string.IsNullOrEmpty(followingId) || string.IsNullOrEmpty(followerId))
            {
                throw new ArgumentException("Post ID cannot be null or empty.");
            }


            try
            {
                FirebaseResponse response = await _firebaseClient.GetAsync($"follows/");
                var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Follow>>();
               
                foreach (var follow in followedDictionary)
                {
                    var followKey = follow.Key;
                    if (follow.Value.FollowedId.Equals(followingId) && follow.Value.FollowerId.Equals(followerId) )
                    {
                        if(followKey != null)
                        {
                            await _firebaseClient.DeleteAsync($"follows/{followKey}");

                            break;
                        }
                       
                    }
                }
               
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the post.", ex);
            }

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
            var response = await _firebaseClient.GetAsync("posts");
            var posts = response.ResultAs<Dictionary<string, Post>>();

            if (posts != null)
            {
                foreach (var postId in posts.Keys.ToList())
                {
                    var post = posts[postId];

                    var commentsResponse = await _firebaseClient.GetAsync($"posts/{postId}/comments");
                    var comments = commentsResponse.ResultAs<Dictionary<string, Comment>>();

                    post.Comments = comments ?? new Dictionary<string, Comment>();


                    //Fetch likes for posts
                    var likesResponse = await _firebaseClient.GetAsync($"posts/{postId}/likes");
                    var likes = likesResponse.ResultAs<Dictionary<string, Like>>();

                    post.Likes = likes ?? new Dictionary<string, Like>();

                    //Fetch likes for comments
                    foreach (var commentId in post.Comments.Keys.ToList())
                    {
                        var comment = post.Comments[commentId];

                        var clikesResponse = await _firebaseClient.GetAsync($"posts/{postId}/comments/{commentId}/likes");
                        var clikes = clikesResponse.ResultAs<Dictionary<string, Like>>();

                        comment.Likes = likes ?? new Dictionary<string, Like>();
                    }
                }
            }
            return posts;
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
            var followedIds = await GetFollowedIdsSetAsync(userId);
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
                         CreatedTime = kvp.Value.CreatedTime,
                         Comments = kvp.Value.Comments,
                          Likes = kvp.Value.Likes
                     })
                     .OrderByDescending(post => post.CreatedTime)
                     .Take(numberToShow)
                     .ToList();
            }

            return posts;
        }

        public async Task<PostList> GetPostlistsAsync(string id)
        {
            var thePosts = new PostList
            {
                MyPosts = await FindPostListAsync(id, 5),
                MyFollowedPosts = await FindFollowedPostsAsync(id, 3),//await _postService.FindFollowedPostsAsync(id, 5)
            };

            return thePosts;
        }

    }
}
