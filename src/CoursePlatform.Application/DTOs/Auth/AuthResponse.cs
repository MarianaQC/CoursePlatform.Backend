namespace CoursePlatform.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public List<string> Roles { get; set; } = new();
}