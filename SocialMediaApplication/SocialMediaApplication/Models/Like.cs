namespace SocialMediaApplication.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}