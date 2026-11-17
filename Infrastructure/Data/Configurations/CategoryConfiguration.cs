using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.NameAr)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(c => c.NameEn)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.NameAr)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(c => c.NameEn)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
