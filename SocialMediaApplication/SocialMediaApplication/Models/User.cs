namespace SocialMediaApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarURL { get; set; } = "";
        public DateTime CreatedTime { get; set; }
    }
}
