namespace SocialMediaApplication.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<User> Followers { get; set; } = new List<User>();
        public List<User> Following { get; set; } = new List<User>();
    }
}
