namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public required int PostId { get; set; }
        public required int AuthorId { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}