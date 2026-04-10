using backend.Application.Common.Interfaces;
using backend.Application.Genres.Queries.GetGenres;

namespace backend.Application.Genres.Queries.GetGenresByMovie;

public sealed record GetGenresByMovieQuery(int MovieId) : IRequest<IReadOnlyList<GenreBriefDto>>;

public sealed class GetGenresByMovieQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetGenresByMovieQuery, IReadOnlyList<GenreBriefDto>>
{
    public async Task<IReadOnlyList<GenreBriefDto>> Handle(GetGenresByMovieQuery request, CancellationToken ct)
        => await db.MovieGenres.AsNoTracking()
            .Where(x => x.MovieId == request.MovieId)
            .Select(x => new GenreBriefDto(x.Genre.Id, x.Genre.Name))
            .ToListAsync(ct);
}
