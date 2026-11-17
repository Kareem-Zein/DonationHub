using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    [Required]
    [Range(1, 30)]
    public int ExpiryDays { get; set; }
}
