﻿using Firebase.Auth;
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
        public async Task AddFollowAsync(string followingId, string followerId)
        {
            var followingUser = await GetUserProfileAsync(followingId);
            var followerUser = await GetUserProfileAsync(followerId);

            if (followingUser == null || followerUser == null)
            {
                throw new ArgumentException("Following and Followers cannot be null or empty.");
            }


            try
            {
                FirebaseResponse response = await _firebaseClient.GetAsync($"users/{followerId}/followings");
                var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>>();
                var isChanged = false;
                foreach(var follow in followedDictionary)
                {
                    if (follow.Value.Equals(followingUser))
                    {
                        await _firebaseClient.SetAsync($"users/{followerId}/followings/{follow.Key}", followingUser);
                        isChanged = true;
                        break;
                    }
                }

                if (!isChanged)
                {
                    await _firebaseClient.PushAsync($"users/{followerId}/followings", followingUser);
                    await _firebaseClient.PushAsync($"users/{followingId}/followers", followerUser);
                }
               
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the post.", ex);
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
                FirebaseResponse response = await _firebaseClient.GetAsync($"users/{followerId}/followings");
                var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>>();
               
                foreach (var follow in followedDictionary)
                {
                    if (follow.Value.UserId.Equals(followingId) )
                    {
                        await _firebaseClient.DeleteAsync($"users/{followerId}/followings/{follow.Key}");
                       
                        break;
                    }
                }
               
                FirebaseResponse followerResponse = await _firebaseClient.GetAsync($"users/{followingId}/followers");
                var followerDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>>();
                foreach (var follow in followerDictionary)
                {
                    if (follow.Value.UserId.Equals(followerId))
                    {
                        await _firebaseClient.DeleteAsync($"users/{followingId}/followers/{follow.Key}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error saving the post.", ex);
            }

        }


        public async Task<List<SocialMediaApplication.Models.User>> GetFollowerIdsAsync(string userId)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/followers");
            var followedDictionary = response.ResultAs<Dictionary<string, SocialMediaApplication.Models.User>> ();
            var users = await GetUsersAsync();
            var followers = followedDictionary.Values.ToList();
            if (followedDictionary != null)
            {
                if (users != null && followers != null)
                {
                    foreach (var follower in followers)
                    {
                        if(users.TryGetValue(follower.UserId,out var user))
                        {
                            follower.Name = user.Name;
                            follower.Email = user.Email;
                            follower.ProfilePictureUrl = user.ProfilePictureUrl;
                            follower.Bio = user.Bio;
                            follower.Password = "";
                        }
                        await _firebaseClient.SetAsync($"users/{userId}/followers/{follower.UserId}", follower);
                    }
                    
                    return followers;
                }
                
            }

            return new List<SocialMediaApplication.Models.User>();
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

        /*
        public async Task<List<Post>> FindFollowedPostsAsync(string userId, int numberToShow)
        {
            var posts = new List<Post>();
            var followedUsers = await _firebaseClient.GetAsync($"follows");
            var followedIds = new HashSet<string>(followedUsers.Select(f => f.UserId));
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
        }*/


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
