namespace SocialMediaApplication.Models
{
    public class Post
    {
        public string Id { get; set; }
        public required string Content { get; set; }
        public required string AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string AuthorAvatar { get; set; }
       
        public DateTime CreatedTime { get; set; }

        public Dictionary<string, Comment> Comments { get; set; } = new Dictionary<string, Comment>();

        public Dictionary<string, Like> Likes { get; set; } = new Dictionary<string, Like>();

    }
}