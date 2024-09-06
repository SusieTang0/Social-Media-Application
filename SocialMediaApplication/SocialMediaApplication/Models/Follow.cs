namespace SocialMediaApplication.Models
{
    public class Follow
    {
        public string Id {  get; set; }
        public string FollowedId { get; set; }
        public string FollowerId { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}