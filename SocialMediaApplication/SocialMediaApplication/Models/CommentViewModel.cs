namespace SocialMediaApplication.Models
{
    public class CommentViewModel
    {
        public string PostId { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
