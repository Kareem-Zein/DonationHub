using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.RegisterCommand;

public class RegisterCommandDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Language PrefferLanguage { get; set; }
}
