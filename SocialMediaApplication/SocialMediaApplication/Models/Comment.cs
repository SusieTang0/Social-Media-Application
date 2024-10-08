namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public required string PostId { get; set; }
        public required string UserId { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public List<Like> Likes {get;set;} = new List<Like>();
    }
}