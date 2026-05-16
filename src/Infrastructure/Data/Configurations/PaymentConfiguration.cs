using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====
        builder.Property(x => x.Amount)
            .IsRequired();

        builder.Property(x => x.PaymentDateTime)
            .IsRequired();

        builder.Property(x => x.Method)
            .IsRequired();


        builder.Property(x => x.ShowId)
            .IsRequired();

        // ===== Relationships =====


        // Payment -> Show (many payments belong to one show)
        builder.HasOne(p => p.Show)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}