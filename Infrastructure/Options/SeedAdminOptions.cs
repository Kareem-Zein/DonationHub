using System.ComponentModel.DataAnnotations;

internal class SeedAdminOptions
{
    public const string SectionName = "AdminData";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
