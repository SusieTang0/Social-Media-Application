namespace SocialMediaApplication.Models
{
    public class Post
    {
        public string Id { get; set; }
        public required string Content { get; set; }
        public required string AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
        
    }
}