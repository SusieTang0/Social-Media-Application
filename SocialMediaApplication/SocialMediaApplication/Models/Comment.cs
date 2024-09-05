namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public required string PostId { get; set; }
        public required string AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedTime { get; set; }

        public List<Like> Likes {get;set;} = new List<Like>();
    }
}