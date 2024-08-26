namespace SocialMediaApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string Phone { get; set; }="";
        public string AvatarURL { get; set; }="";
    }
}
