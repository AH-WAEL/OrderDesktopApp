

namespace desktopAPI.Models
{
    public class registerResponseDto
    {
        public Guid id { get; set; }

        public string username { get; set; }

        public string email { get; set;  }

        public string Role { get; set; } = "User";
    }
}
