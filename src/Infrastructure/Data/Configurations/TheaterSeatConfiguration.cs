using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class TheaterSeatConfiguration : IEntityTypeConfiguration<TheaterSeat>
{
    public void Configure(EntityTypeBuilder<TheaterSeat> builder)
    {
        // Composite key: Theater + Row + Number
        builder.HasKey(x => new { x.TheaterId, x.SeatRow, x.SeatNumber });

        // Properties
        builder.Property(x => x.SeatRow)
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(x => x.SeatNumber)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        // Relationship -> Theater
        builder.HasOne(ts => ts.Theater)
            .WithMany(t => t.TheaterSeats)
            .HasForeignKey(ts => ts.TheaterId);
    }
}