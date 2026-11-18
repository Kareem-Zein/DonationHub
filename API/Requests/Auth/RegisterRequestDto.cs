using Domain.Enums;

namespace API.Requests.Auth;

public class RegisterRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Language PrefferLanguage { get; set; }
}
