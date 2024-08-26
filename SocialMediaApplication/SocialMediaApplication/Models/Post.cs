namespace SocialMediaApplication.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
        
    }
}