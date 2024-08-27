using AspNetCore;
using SocialMediaApplication.Models;

namespace SocialMediaApplication.Data
{
    public class ApplicationData
    {
        public static List<User> Users = new List<User>() {
            new User{Id = 1 ,Name = "Test 1", Email = "test1@test.com", AvatarURL = "/images/logo.png",CreatedTime = new DateTime(2023,1,29,13,0,0)},
            new User{Id = 2 ,Name = "Test 2", Email = "test2@test.com",AvatarURL = "/images/logo.png", CreatedTime = new DateTime(2023,2,29,13,0,0)},
            new User{Id = 3 ,Name = "Test 3", Email = "test3@test.com",AvatarURL = "/images/logo.png", CreatedTime = new DateTime(2023,3,29,13,0,0)},
            new User{Id = 4 ,Name = "Test 4", Email = "test4@test.com", AvatarURL = "/images/logo.png", CreatedTime = new DateTime(2023,4,29,13,0,0)},
        };

        public static List<Post> Posts = new List<Post>() {
            new Post{Id = 1 ,Content = "Post 1 content", AuthorId = 1, CreatedTime = new DateTime(2023,1,29,13,0,0)},
            new Post{Id = 2 ,Content = "Post 2 content", AuthorId = 2, CreatedTime = new DateTime(2023,2,29,13,0,0)},
            new Post{Id = 3 ,Content = "Post 3 content", AuthorId = 1, CreatedTime = new DateTime(2023,3,29,13,0,0)},
            new Post{Id = 4 ,Content = "Post 4 content", AuthorId = 3, CreatedTime = new DateTime(2023,4,29,13,0,0)},
        };


        public static List<Comment> Comments = new List<Comment>() {
            new Comment{Id = 1 , PostId = 1, Content = "Post 1 Comment 1 content", AuthorId = 2, CreatedTime = new DateTime(2023,2,29,13,0,0)},
            new Comment{Id = 2 , PostId = 1, Content = "Post 1 Comment 2 content", AuthorId = 3, CreatedTime = new DateTime(2023,3,11,15,0,0)},
            new Comment{Id = 3 , PostId = 1, Content = "Post 1 Comment 3 content", AuthorId = 4, CreatedTime = new DateTime(2023,3,29,19,0,0)},
            new Comment{Id = 4 , PostId = 2, Content = "Post 2 Comment 1 content", AuthorId = 1, CreatedTime = new DateTime(2023,4,29,13,0,0)},
            new Comment{Id = 5 , PostId = 2, Content = "Post 2 Comment 2 content", AuthorId = 3, CreatedTime = new DateTime(2023,4,30,9,0,0)},
            new Comment{Id = 6 , PostId = 3, Content = "Post 3 Comment 1 content ", AuthorId = 4, CreatedTime = new DateTime(2023,5,3,21,0,0)},

        };


        public static List<Like> Likes = new List<Like>() {
            new Like{Id = 1 , PostId = 1, AuthorId = 2, CreatedTime = new DateTime(2023,2,29,13,0,0)},
            new Like{Id = 2 , PostId = 1, AuthorId = 3, CreatedTime = new DateTime(2023,3,11,15,0,0)},
            new Like{Id = 3 , PostId = 2, AuthorId = 1, CreatedTime = new DateTime(2023,3,29,13,0,0)},
            new Like{Id = 4 , PostId = 1, AuthorId = 4, CreatedTime = new DateTime(2023,3,29,19,0,0)},
            new Like{Id = 5 , PostId = 1, CommentId = 1, AuthorId = 1, CreatedTime = new DateTime(2023,3,29,13,5,0)},
            new Like{Id = 6 , PostId = 1, CommentId = 1, AuthorId = 3, CreatedTime = new DateTime(2023,4,8,13,5,0)},
            new Like{Id = 7 , PostId = 1, CommentId = 2, AuthorId = 2, CreatedTime = new DateTime(2023,4,8,17,5,0)},
            new Like{Id = 8 , PostId = 1, CommentId = 1,  AuthorId = 4, CreatedTime = new DateTime(2023,4,13,13,5,0)},
            new Like{Id = 9 , PostId = 1, CommentId = 2,  AuthorId = 4, CreatedTime = new DateTime(2023,4,29,13,0,0)},

        };

        public static List<Follow> Follows = new List<Follow>() {
            new Follow{Id = 1 , FollowedId = 1, FollowerId = 2, CreatedTime = new DateTime(2023,2,29,13,0,0)},
            new Follow{Id = 2 , FollowedId = 2, FollowerId = 1,CreatedTime = new DateTime(2023,3,11,15,0,0)},
            new Follow{Id = 3 , FollowedId = 1, FollowerId = 3, CreatedTime = new DateTime(2023,3,29,13,0,0)},
            new Follow{Id = 4 , FollowedId = 2, FollowerId = 3,CreatedTime = new DateTime(2023,3,29,19,0,0)},
            new Follow{Id = 5 , FollowedId = 3, FollowerId = 2, CreatedTime = new DateTime(2023,3,29,13,5,0)},
            new Follow{Id = 6 , FollowedId = 1, FollowerId = 4, CreatedTime = new DateTime(2023,4,29,19,5,0)},
            new Follow{Id = 7 , FollowedId = 4, FollowerId = 1, CreatedTime = new DateTime(2023,4,29,20,5,0)},
           
        };

        public static void AddPost(Post post)
        {
            post.Id = Posts[(Posts.Count - 1)].Id + 1;
            post.CreatedTime = DateTime.Now;
            Posts.Add(post);
        }

        public static void UpdatePost(Post post)
        {
            var existingPost = Posts.FirstOrDefault(p => p.Id == post.Id);
            if (existingPost != null)
            {
                existingPost.Content = post.Content;
                existingPost.AuthorId = post.AuthorId;
                existingPost.CreatedTime = DateTime.Now;
            }

        }

        public static void DeletePost(int postId)
        {
            var existingPost = Posts.FirstOrDefault(x => x.Id == postId);
            if (existingPost != null)
            {
                Posts.Remove(existingPost);
            }
        }
    }
}
