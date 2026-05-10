using backend.Application.Common.Interfaces;
using backend.Application.Shows.Queries.GetShows;

namespace backend.Application.Shows.Queries.GetFilteredShows;

public sealed record GetFilteredShowsQuery(DateTime? Date, int? MovieId) : IRequest<IReadOnlyList<ShowDetailDto>>;

public sealed record ShowDetailDto(int Id, DateTime Date, TimeSpan StartTime, int MovieId, string MovieTitle, int TheaterId, string TheaterName, decimal Price);

public sealed class GetFilteredShowsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFilteredShowsQuery, IReadOnlyList<ShowDetailDto>>
{
    public async Task<IReadOnlyList<ShowDetailDto>> Handle(GetFilteredShowsQuery request, CancellationToken ct)
    {
        var query = db.Shows
            .Include(s => s.Movie)
            .Include(s => s.Theater)
            .AsNoTracking().AsQueryable();

        if (request.Date.HasValue)
            query = query.Where(x => x.Date.Date == request.Date.Value.Date);
            
        if (request.MovieId.HasValue)
            query = query.Where(x => x.MovieId == request.MovieId.Value);

        return await query
            .OrderBy(x => x.Date).ThenBy(x => x.StartTime)
            .Select(x => new ShowDetailDto(
                x.Id, 
                x.Date, 
                x.StartTime, 
                x.MovieId, 
                x.Movie.Title, 
                x.TheaterId, 
                x.Theater.Name,
                100000m // Placeholder for price if not in entity
            ))
            .ToListAsync(ct);
    }
}
