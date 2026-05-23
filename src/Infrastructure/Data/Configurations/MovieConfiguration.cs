using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // ===== Properties =====
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Summary)
            .HasMaxLength(2000);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasPrecision(3, 1); 
        // ví dụ: 8.5 / 9.2

        builder.Property(x => x.TrailerUrl)
            .HasColumnType("text");

        builder.Property(x => x.PosterUrl)
            .HasColumnType("text");

        builder.Property(x => x.MovieType)
            .IsRequired();

        builder.Property(m => m.Embedding)
            .HasColumnType("vector(768)");


        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");

        // ===== Relationships =====

        // Movie -> Show (1 - many)
        builder.HasMany(m => m.Shows)
            .WithOne(s => s.Movie)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movie -> MovieGenre (1 - many)
        builder.HasMany(m => m.MovieGenres)
            .WithOne(mg => mg.Movie)
            .HasForeignKey(mg => mg.MovieId);

        // Movie -> MoviePerson (1 - many)
        builder.HasMany(m => m.MoviePersons)
            .WithOne(mp => mp.Movie)
            .HasForeignKey(mp => mp.MovieId);
    }
}