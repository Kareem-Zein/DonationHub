using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");
        builder.HasKey(pa => pa.Id);

        builder.OwnsOne(c => c.Amount, amount =>
        {
            amount.Property(ra => ra.Amount).HasPrecision(18, 2).IsRequired();
            amount.Property(ra => ra.Currency).HasMaxLength(3).IsRequired();
        });

        builder.HasOne<Donation>()
            .WithMany()
            .HasForeignKey(pa => pa.DonationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pa => pa.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
