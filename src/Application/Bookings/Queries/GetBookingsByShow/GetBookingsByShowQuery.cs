using backend.Application.Common.Interfaces;
using backend.Application.Bookings.Queries.GetBookings;

namespace backend.Application.Bookings.Queries.GetBookingsByShow;

public sealed record GetBookingsByShowQuery(int ShowId) : IRequest<IReadOnlyList<BookingBriefDto>>;

public sealed class GetBookingsByShowQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBookingsByShowQuery, IReadOnlyList<BookingBriefDto>>
{
    public async Task<IReadOnlyList<BookingBriefDto>> Handle(GetBookingsByShowQuery request, CancellationToken ct)
        => await db.Bookings.AsNoTracking()
            .Where(x => x.ShowId == request.ShowId)
            .OrderByDescending(x => x.BookingDateTime)
            .Select(x => new BookingBriefDto(x.Id, x.UserId, x.ShowId, x.SeatRow, x.SeatNumber, x.Price, x.Status, x.BookingDateTime))
            .ToListAsync(ct);
}
