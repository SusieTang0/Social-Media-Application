namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        List<int> Likes { get; set; }
    }
}
