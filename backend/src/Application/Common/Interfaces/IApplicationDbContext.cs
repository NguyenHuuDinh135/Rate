using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;

namespace backend.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Movie> Movies { get; }
    DbSet<Genre> Genres { get; }
    DbSet<MovieGenre> MovieGenres { get; }
    DbSet<MoviePerson> MoviePersons { get; }
    DbSet<Person> Persons { get; }

    DbSet<Theater> Theaters { get; }
    DbSet<TheaterSeat> TheaterSeats { get; }
    DbSet<Show> Shows { get; }

    DbSet<Booking> Bookings { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}