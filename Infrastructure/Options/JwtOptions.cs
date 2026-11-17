using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required, MinLength(32)]
        public string Key { get; set; } = null!;

        [Required]
        public string Issuer { get; set; } = null!;

        [Required]
        public string Audience { get; set; } = null!;

        [Range(1, 1440)]
        public int AccessTokenExpiryMinutes { get; set; } = 15;
    }
}
