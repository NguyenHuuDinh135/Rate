using backend.Application.Common.Interfaces;
using backend.Application.Shows.Queries.GetShows;

namespace backend.Application.Shows.Queries.GetFilteredShows;

public sealed record GetFilteredShowsQuery(DateTime? Date) : IRequest<IReadOnlyList<ShowBriefDto>>;

public sealed class GetFilteredShowsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFilteredShowsQuery, IReadOnlyList<ShowBriefDto>>
{
    public async Task<IReadOnlyList<ShowBriefDto>> Handle(GetFilteredShowsQuery request, CancellationToken ct)
    {
        var query = db.Shows.AsNoTracking().AsQueryable();

        if (request.Date.HasValue)
            query = query.Where(x => x.Date.Date == request.Date.Value.Date);

        return await query
            .OrderBy(x => x.Date).ThenBy(x => x.StartTime)
            .Select(x => new ShowBriefDto(x.Id, x.Date, x.StartTime, x.EndTime, x.MovieId, x.TheaterId, x.Status, x.Type))
            .ToListAsync(ct);
    }
}
