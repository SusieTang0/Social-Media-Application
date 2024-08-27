namespace SocialMediaApplication.Models
{
    public class Follow
    {
        public int Id {  get; set; }
        public int FollowedId { get; set; }
        public int FollowerId { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}