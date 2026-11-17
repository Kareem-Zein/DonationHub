using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.PreferredLanguage)
            .HasConversion<string>()
            .HasMaxLength(2);
    }
}
