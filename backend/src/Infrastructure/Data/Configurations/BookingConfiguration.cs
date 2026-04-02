using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====


        builder.Property(x => x.ShowId)
            .IsRequired();

        builder.Property(x => x.SeatRow)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(x => x.SeatNumber)
            .IsRequired();

        builder.Property(x => x.Price)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.BookingDateTime)
            .IsRequired();

        // ===== Relationships =====

        // Booking -> Show (Many bookings belong to one show)
        builder.HasOne(b => b.Show)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}