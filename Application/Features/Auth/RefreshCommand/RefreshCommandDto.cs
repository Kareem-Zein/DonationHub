using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.RefreshCommand;

public class RefreshCommandDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
