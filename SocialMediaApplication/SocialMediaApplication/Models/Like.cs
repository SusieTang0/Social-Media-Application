namespace SocialMediaApplication.Models
{
    public class Like
    {
        public string Id { get; set; }
        public string? PostId { get; set; }
        public string? CommentId { get; set; }
        public required string UserId { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}