namespace AuthWebapi.Entities
{
    public class users
    {
        public Guid id { get; set; }
        public string username { get; set; }

        public string email { get; set; } = string.Empty;
        public string HashPassword { get; set; }

        public string Role { get; set; } = "user";

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
