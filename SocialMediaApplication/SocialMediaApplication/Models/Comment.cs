namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}