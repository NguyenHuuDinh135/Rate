using backend.Application.Common.Interfaces;
using backend.Application.Movies.Queries.GetMovies;
using backend.Application.Genres.Queries.GetGenres;

namespace backend.Application.Persons.Queries.GetMoviesByPerson;

public sealed record GetMoviesByPersonQuery(int PersonId) : IRequest<IReadOnlyList<MovieBriefDto>>;

public sealed class GetMoviesByPersonQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMoviesByPersonQuery, IReadOnlyList<MovieBriefDto>>
{
    public async Task<IReadOnlyList<MovieBriefDto>> Handle(GetMoviesByPersonQuery request, CancellationToken ct)
        => await db.MoviePersons.AsNoTracking()
            .Where(x => x.PersonId == request.PersonId)
            .Select(x => new MovieBriefDto(
                x.Movie.Id, 
                x.Movie.Title, 
                x.Movie.Summary, 
                x.Movie.Year, 
                x.Movie.Rating, 
                x.Movie.TrailerUrl, 
                x.Movie.PosterUrl, 
                x.Movie.MovieType,
                x.Movie.MovieGenres.Select(mg => new GenreBriefDto(mg.Genre.Id, mg.Genre.Name)).ToList()
            ))
            .ToListAsync(ct);
}
