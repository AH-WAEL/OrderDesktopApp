namespace AuthWebapi.Models
{
    public class userRegisterDTO
    {
        public string username { get; set; }

        public string email { get; set; }
        public string password { get; set; }

        public string Role { get; set; } = "User"; // Default role is 'user', can be changed later if needed

    }
}
