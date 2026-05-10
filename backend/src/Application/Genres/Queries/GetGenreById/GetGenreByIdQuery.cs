using backend.Application.Common.Interfaces;
using backend.Application.Genres.Queries.GetGenres;

namespace backend.Application.Genres.Queries.GetGenreById;

public sealed record GetGenreByIdQuery(int Id) : IRequest<GenreBriefDto?>;

public sealed class GetGenreByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetGenreByIdQuery, GenreBriefDto?>
{
    public async Task<GenreBriefDto?> Handle(GetGenreByIdQuery request, CancellationToken ct)
        => await db.Genres.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GenreBriefDto(x.Id, x.Name))
            .FirstOrDefaultAsync(ct);
}
