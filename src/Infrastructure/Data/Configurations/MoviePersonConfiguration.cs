using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class MoviePersonConfiguration : IEntityTypeConfiguration<MoviePerson>
{
    public void Configure(EntityTypeBuilder<MoviePerson> builder)
    {
        // Composite key (Movie + Person + Role)
        builder.HasKey(x => new { x.MovieId, x.PersonId, x.RoleType });

        // RoleType bắt buộc
        builder.Property(x => x.RoleType)
            .IsRequired();

        // Relationship -> Movie
        builder.HasOne(mp => mp.Movie)
            .WithMany(m => m.MoviePersons)
            .HasForeignKey(mp => mp.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship -> Person
        builder.HasOne(mp => mp.Person)
            .WithMany(p => p.MoviePersons)
            .HasForeignKey(mp => mp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}