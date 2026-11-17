using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.LoginCommand;

public sealed class LoginCommandDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
