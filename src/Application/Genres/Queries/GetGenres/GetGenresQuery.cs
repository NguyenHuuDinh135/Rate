using backend.Application.Common.Interfaces;

namespace backend.Application.Genres.Queries.GetGenres;

public sealed record GenreBriefDto(int Id, string Name);

public sealed record GetGenresQuery : IRequest<IReadOnlyList<GenreBriefDto>>;

public sealed class GetGenresQueryHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<GetGenresQuery, IReadOnlyList<GenreBriefDto>>
{
    public async Task<IReadOnlyList<GenreBriefDto>> Handle(GetGenresQuery request, CancellationToken ct)
    {
        const string key = "genres:all";
        var cached = await cache.GetAsync<List<GenreBriefDto>>(key, ct);
        if (cached is not null) return cached;

        var items = await db.Genres.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GenreBriefDto(x.Id, x.Name))
            .ToListAsync(ct);

        await cache.SetAsync(key, items, TimeSpan.FromMinutes(10), ct);
        return items;
    }
}
