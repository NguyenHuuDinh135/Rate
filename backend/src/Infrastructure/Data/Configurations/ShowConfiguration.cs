using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====
        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.MovieId)
            .IsRequired();

        builder.Property(x => x.TheaterId)
            .IsRequired();

        // ===== Relationships =====

        // Show -> Movie (many shows belong to one movie)
        builder.HasOne(s => s.Movie)
            .WithMany(m => m.Shows)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Show -> Theater (many shows belong to one theater)
        builder.HasOne(s => s.Theater)
            .WithMany(t => t.Shows)
            .HasForeignKey(s => s.TheaterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Show -> Booking
        builder.HasMany(s => s.Bookings)
            .WithOne(b => b.Show)
            .HasForeignKey(b => b.ShowId);

        // Show -> Payment
        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Show)
            .HasForeignKey(p => p.ShowId);

        // ===== IMPORTANT: tránh trùng suất chiếu =====
        builder.HasIndex(s => new { s.TheaterId, s.Date, s.StartTime })
            .IsUnique();
    }
}