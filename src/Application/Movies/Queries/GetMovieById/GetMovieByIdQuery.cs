using backend.Application.Common.Interfaces;
using backend.Application.Movies.Queries.GetMovies;
using backend.Application.Genres.Queries.GetGenres;

namespace backend.Application.Movies.Queries.GetMovieById;

public sealed record GetMovieByIdQuery(int Id) : IRequest<MovieBriefDto?>;

public sealed class GetMovieByIdQueryHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<GetMovieByIdQuery, MovieBriefDto?>
{
    public async Task<MovieBriefDto?> Handle(GetMovieByIdQuery request, CancellationToken ct)
    {
        var key = $"movies:{request.Id}:v2";
        var cached = await cache.GetAsync<MovieBriefDto>(key, ct);
        if (cached is not null) return cached;

        var item = await db.Movies.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new MovieBriefDto(
                x.Id, 
                x.Title, 
                x.Summary, 
                x.Year, 
                x.Rating, 
                x.TrailerUrl, 
                x.PosterUrl, 
                x.MovieType,
                x.MovieGenres.Select(mg => new GenreBriefDto(mg.Genre.Id, mg.Genre.Name)).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (item is not null)
            await cache.SetAsync(key, item, TimeSpan.FromMinutes(5), ct);

        return item;
    }
}
