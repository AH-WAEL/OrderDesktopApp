namespace desktopAPI.Models
{
    public class RefreshTokenValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public DateTime DatabaseTime { get; set; }
    }
}
