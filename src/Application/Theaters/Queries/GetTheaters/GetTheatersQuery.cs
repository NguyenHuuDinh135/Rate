using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Theaters.Queries.GetTheaters;

public sealed record TheaterBriefDto(int Id, string Name, int NumOfRows, int SeatsPerRow, TheaterType Type);

public sealed record GetTheatersQuery : IRequest<IReadOnlyList<TheaterBriefDto>>;

public sealed class GetTheatersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTheatersQuery, IReadOnlyList<TheaterBriefDto>>
{
    public async Task<IReadOnlyList<TheaterBriefDto>> Handle(GetTheatersQuery request, CancellationToken ct)
        => await db.Theaters.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TheaterBriefDto(x.Id, x.Name, x.NumOfRows, x.SeatsPerRow, x.Type))
            .ToListAsync(ct);
}
