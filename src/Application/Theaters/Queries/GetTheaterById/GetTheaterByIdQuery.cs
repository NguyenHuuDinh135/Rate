using backend.Application.Common.Interfaces;
using backend.Application.Theaters.Queries.GetTheaters;

namespace backend.Application.Theaters.Queries.GetTheaterById;

public sealed record GetTheaterByIdQuery(int Id) : IRequest<TheaterBriefDto?>;

public sealed class GetTheaterByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTheaterByIdQuery, TheaterBriefDto?>
{
    public async Task<TheaterBriefDto?> Handle(GetTheaterByIdQuery request, CancellationToken ct)
        => await db.Theaters.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new TheaterBriefDto(x.Id, x.Name, x.NumOfRows, x.SeatsPerRow, x.Type))
            .FirstOrDefaultAsync(ct);
}
