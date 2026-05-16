using backend.Application.Common.Interfaces;
using backend.Application.Movies.Queries.GetMovies;
using backend.Domain.Enums;

namespace backend.Application.Movies.Queries.GetFilteredMovies;

public sealed record GetFilteredMoviesQuery(string? Title, MovieType? Type, int? Year) : IRequest<IReadOnlyList<MovieBriefDto>>;

public sealed class GetFilteredMoviesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFilteredMoviesQuery, IReadOnlyList<MovieBriefDto>>
{
    public async Task<IReadOnlyList<MovieBriefDto>> Handle(GetFilteredMoviesQuery request, CancellationToken ct)
    {
        var query = db.Movies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Title))
            query = query.Where(x => x.Title.Contains(request.Title));
        if (request.Type.HasValue)
            query = query.Where(x => x.MovieType == request.Type.Value);
        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        return await query
            .OrderByDescending(x => x.Year)
            .Select(x => new MovieBriefDto(x.Id, x.Title, x.Summary, x.Year, x.Rating, x.TrailerUrl, x.PosterUrl, x.MovieType))
            .ToListAsync(ct);
    }
}
