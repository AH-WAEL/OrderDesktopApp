namespace desktopAPI.Models;

public class UserRegisterDTO
{
    public Guid Id { get; set; }
    public string username { get; set; }

    public string email { get; set; }
    public string password { get; set; }

    public string Role { get; set; } = "User"; // Default role is 'User', can be changed later if needed
}
