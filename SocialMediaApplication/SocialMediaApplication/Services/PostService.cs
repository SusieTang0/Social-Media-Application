﻿using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using SocialMediaApplication.Models;

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
        public async Task<List<SocialMediaApplication.Models.User>> GetUsersAsync()
        {
            FirebaseResponse response = await _firebaseClient.GetAsync("users");

 
            var usersDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>>();

            if (usersDictionary != null)
            {
                return usersDictionary.Values.ToList();
            }

            return new List<SocialMediaApplication.Models.User>();
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

      
        public async Task<Dictionary<string,SocialMediaApplication.Models.User>> GetFollowedsUserAsync(string userId)
        {
            var users = await GetUsersAsync();
            var result = new Dictionary<string,SocialMediaApplication.Models.User>();
            var followings = await GetFollowsAsync();
            
            if (users != null && followings != null)
            {
                foreach (var following in followings)
                {
                    if (users.TryGetValue(following.FollowerId, out var user))
                    {

                        result.Add(following.FollowerId, user);
                      
                    }
                    
                }

                return result;
            }


            return null;
        }

        // Post

        public async Task AddPost(string userId, string content,string userName,string pictureUrl)
        {
            var post = new
            {
<<<<<<< HEAD
                throw new ArgumentException("Following and Followers cannot be null or empty.");
            }

=======
                Content = content,
                AuthorId = userId,
                AuthorName = userName,
                AuthorAvatar = pictureUrl,
                CreatedTime = DateTime.UtcNow
            };

            // Push the post to Firebase
            var response = await _firebaseClient.PushAsync("posts", post);
        }



       /* public async Task<List<SocialMediaApplication.Models.Post>> GetPostsAsync()
        {
            FirebaseResponse response = await _firebaseClient.GetAsync("posts");

            
            var postsDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Post>>();

            if (postsDictionary != null)
            {
                return postsDictionary.Values.ToList();
            }

            return new List<SocialMediaApplication.Models.Post>();
        }*/
        public async Task<Dictionary<string, Post>> GetPostsAsync()
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

                    if (post.Comments != null && post.Comments.Any())
                    {
                        Console.WriteLine($"Post {postId} has {post.Comments.Count} comments.");
                    }
                    else
                    {
                        Console.WriteLine($"Post {postId} has no comments.");
                    }
                }
            }
            return posts;
        }
>>>>>>> parent of 1356deb (Merge remote-tracking branch 'origin/shuting' into Shawnelle)

        public async Task<List<SocialMediaApplication.Models.Post>> GetPostsByUserIdAsync(string userId)
        {
            try
            {
                FirebaseResponse response = await _firebaseClient.GetAsync($"posts/{userId}");

                var userPostsDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Post>>();

                return userPostsDictionary != null ? userPostsDictionary.Values.ToList() : new List<SocialMediaApplication.Models.Post>();
            }
            catch (Exception ex)
            {
                return new List<SocialMediaApplication.Models.Post>();
            }
        }

        public async Task<SocialMediaApplication.Models.Post> GetPostByPostIdAsync(string postId)
        {
            try
            {
                FirebaseResponse response = await _firebaseClient.GetAsync($"posts/{postId}");
                var post = response.ResultAs<SocialMediaApplication.Models.Post>();
                return post;
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }

        public async Task<FirebaseResponse> SavePostAsync(string postId, SocialMediaApplication.Models.Post post)
        {
            if (string.IsNullOrEmpty(postId) || post == null)
            {
                throw new ArgumentException("Post ID and Post object cannot be null or empty.");
            }

            try
            {
                return await _firebaseClient.SetAsync($"posts/{postId}", post);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the post.", ex);
            }
        }

        public async Task<FirebaseResponse> DeletePostAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("Post ID cannot be null or empty.");
            }

            try
            {
                return await _firebaseClient.DeleteAsync($"posts/{postId}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting the post.", ex);
            }
        }

    }
}
