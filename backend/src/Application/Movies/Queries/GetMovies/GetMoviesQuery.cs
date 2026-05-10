using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Movies.Queries.GetMovies;

public sealed record MovieBriefDto(
    int Id, string Title, string Summary, int Year,
    decimal? Rating, string TrailerUrl, string PosterUrl, MovieType MovieType);

public sealed record GetMoviesQuery : IRequest<IReadOnlyList<MovieBriefDto>>;

public sealed class GetMoviesQueryHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<GetMoviesQuery, IReadOnlyList<MovieBriefDto>>
{
    public async Task<IReadOnlyList<MovieBriefDto>> Handle(GetMoviesQuery request, CancellationToken ct)
    {
        const string key = "movies:all";
        var cached = await cache.GetAsync<List<MovieBriefDto>>(key, ct);
        if (cached is not null) return cached;

        var items = await db.Movies.AsNoTracking()
            .OrderByDescending(x => x.Year)
            .Select(x => new MovieBriefDto(x.Id, x.Title, x.Summary, x.Year, x.Rating, x.TrailerUrl, x.PosterUrl, x.MovieType))
            .ToListAsync(ct);

        await cache.SetAsync(key, items, TimeSpan.FromMinutes(5), ct);
        return items;
    }
}
