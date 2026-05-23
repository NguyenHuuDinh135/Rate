using backend.Application.Common.Interfaces;
using backend.Application.Bookings.Queries.GetBookings;

namespace backend.Application.Bookings.Queries.GetBookingsByUser;

public sealed record GetBookingsByUserQuery(string UserId) : IRequest<IReadOnlyList<BookingBriefDto>>;

public sealed class GetBookingsByUserQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBookingsByUserQuery, IReadOnlyList<BookingBriefDto>>
{
    public async Task<IReadOnlyList<BookingBriefDto>> Handle(GetBookingsByUserQuery request, CancellationToken ct)
        => await db.Bookings.AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.BookingDateTime)
            .Select(x => new BookingBriefDto(x.Id, x.UserId, x.ShowId, x.SeatRow, x.SeatNumber, x.Price, x.Status, x.BookingDateTime))
            .ToListAsync(ct);
}
