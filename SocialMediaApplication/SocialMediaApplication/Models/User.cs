namespace SocialMediaApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public List<string> Tabs { get; set; }
        public string AvatarURL { get; set; }

        List<UserInfo> MyFollows { get; set; }
        List<UserInfo> MyFollowers { get; set; }

       
    }
}
