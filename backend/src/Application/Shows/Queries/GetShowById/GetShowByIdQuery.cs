using backend.Application.Common.Interfaces;
using backend.Application.Shows.Queries.GetShows;

namespace backend.Application.Shows.Queries.GetShowById;

public sealed record GetShowByIdQuery(int Id) : IRequest<ShowBriefDto?>;

public sealed class GetShowByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetShowByIdQuery, ShowBriefDto?>
{
    public async Task<ShowBriefDto?> Handle(GetShowByIdQuery request, CancellationToken ct)
        => await db.Shows.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ShowBriefDto(x.Id, x.Date, x.StartTime, x.EndTime, x.MovieId, x.TheaterId, x.Status, x.Type))
            .FirstOrDefaultAsync(ct);
}
