using backend.Application.Common.Interfaces;
using backend.Application.Bookings.Queries.GetBookings;

namespace backend.Application.Bookings.Queries.GetBookingById;

public sealed record GetBookingByIdQuery(int Id) : IRequest<BookingBriefDto?>;

public sealed class GetBookingByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBookingByIdQuery, BookingBriefDto?>
{
    public async Task<BookingBriefDto?> Handle(GetBookingByIdQuery request, CancellationToken ct)
        => await db.Bookings.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new BookingBriefDto(x.Id, x.UserId, x.ShowId, x.SeatRow, x.SeatNumber, x.Price, x.Status, x.BookingDateTime))
            .FirstOrDefaultAsync(ct);
}
