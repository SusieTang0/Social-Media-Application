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

        public async Task AddPost(string userId, string content)
        {
            var post = new
            {
                AuthorId = userId,
                Content = content,
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
            return posts;
        }

        public async Task<List<SocialMediaApplication.Models.Post>> GetPostsByUserIdAsync(string userId)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync($"posts/{userId}");

            var userPostsDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.Post>>();

            if (userPostsDictionary != null)
            {
                return userPostsDictionary.Values.ToList();
            }

            return new List<SocialMediaApplication.Models.Post>();
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
