using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class TheaterConfiguration : IEntityTypeConfiguration<Theater>
{
    public void Configure(EntityTypeBuilder<Theater> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.NumOfRows)
            .IsRequired();

        builder.Property(x => x.SeatsPerRow)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        // ===== Relationships =====

        // Theater -> Shows
        builder.HasMany(t => t.Shows)
            .WithOne(s => s.Theater)
            .HasForeignKey(s => s.TheaterId);

        // Theater -> TheaterSeats
        builder.HasMany(t => t.TheaterSeats)
            .WithOne(ts => ts.Theater)
            .HasForeignKey(ts => ts.TheaterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}