using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Age)
            .IsRequired();

        builder.Property(x => x.PictureUrl)
            .HasMaxLength(500);

        // ===== Relationships =====

        // Person -> MoviePerson (1 - many)
        builder.HasMany(p => p.MoviePersons)
            .WithOne(mp => mp.Person)
            .HasForeignKey(mp => mp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}