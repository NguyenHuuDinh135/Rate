using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Bookings.Queries.GetBookings;

public sealed record BookingBriefDto(
    int Id, string UserId, int ShowId, string SeatRow,
    int SeatNumber, float Price, BookingStatus Status, DateTime BookingDateTime);

public sealed record GetBookingsQuery : IRequest<IReadOnlyList<BookingBriefDto>>;

public sealed class GetBookingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBookingsQuery, IReadOnlyList<BookingBriefDto>>
{
    public async Task<IReadOnlyList<BookingBriefDto>> Handle(GetBookingsQuery request, CancellationToken ct)
        => await db.Bookings.AsNoTracking()
            .OrderByDescending(x => x.BookingDateTime)
            .Select(x => new BookingBriefDto(x.Id, x.UserId, x.ShowId, x.SeatRow, x.SeatNumber, x.Price, x.Status, x.BookingDateTime))
            .ToListAsync(ct);
}
