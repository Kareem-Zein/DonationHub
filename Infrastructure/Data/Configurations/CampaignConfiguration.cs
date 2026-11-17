using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");

            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.GoalAmount, goalAmount =>
            {
                goalAmount.Property(ga => ga.Amount).HasPrecision(18, 2).IsRequired();
                goalAmount.Property(ga => ga.Currency).HasMaxLength(3).IsRequired();
            });
            
            builder.OwnsOne(c => c.RaisedAmount, raisedAmount =>
            {
                raisedAmount.Property(ra => ra.Amount).HasPrecision(18, 2).IsRequired();
                raisedAmount.Property(ra => ra.Currency).HasMaxLength(3).IsRequired();
            });

            builder.Property(c => c.TitleAr)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.TitleEn)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.TitleAr)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(c => c.TitleEn)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(c => c.RowVersion)
                .IsRowVersion();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
