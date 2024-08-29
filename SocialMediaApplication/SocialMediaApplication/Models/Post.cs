namespace SocialMediaApplication.Models
{
    public class Post
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public required int AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
        
    }
}