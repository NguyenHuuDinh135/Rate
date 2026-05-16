using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Shows.Queries.GetShows;

public sealed record ShowBriefDto(
    int Id, DateTime Date, TimeSpan StartTime, TimeSpan EndTime,
    int MovieId, int TheaterId, ShowStatus Status, ShowType Type);

public sealed record GetShowsQuery : IRequest<IReadOnlyList<ShowBriefDto>>;

public sealed class GetShowsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetShowsQuery, IReadOnlyList<ShowBriefDto>>
{
    public async Task<IReadOnlyList<ShowBriefDto>> Handle(GetShowsQuery request, CancellationToken ct)
        => await db.Shows.AsNoTracking()
            .OrderBy(x => x.Date).ThenBy(x => x.StartTime)
            .Select(x => new ShowBriefDto(x.Id, x.Date, x.StartTime, x.EndTime, x.MovieId, x.TheaterId, x.Status, x.Type))
            .ToListAsync(ct);
}
