using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Bookings.Queries.GetBookingLayout;

public sealed record TheaterSeatDto(string SeatRow, int SeatNumber, SeatType Type);

public sealed record BookingLayoutDto(
    int ShowId,
    int TheaterId,
    string TheaterName,
    int NumOfRows,
    int SeatsPerRow,
    IReadOnlyList<TheaterSeatDto> Seats,
    IReadOnlyList<string> BookedSeats);

public sealed record GetBookingLayoutQuery(int ShowId) : IRequest<BookingLayoutDto?>;

public sealed class GetBookingLayoutQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBookingLayoutQuery, BookingLayoutDto?>
{
    public async Task<BookingLayoutDto?> Handle(GetBookingLayoutQuery request, CancellationToken ct)
    {
        var show = await db.Shows
            .Include(s => s.Theater)
            .ThenInclude(t => t.TheaterSeats)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ShowId, ct);

        if (show == null) return null;

        var bookedSeats = await db.Bookings
            .AsNoTracking()
            .Where(x => x.ShowId == request.ShowId && x.Status != BookingStatus.Cancelled)
            .Select(x => $"{x.SeatRow}{x.SeatNumber}")
            .ToListAsync(ct);

        return new BookingLayoutDto(
            show.Id,
            show.TheaterId,
            show.Theater.Name,
            show.Theater.NumOfRows,
            show.Theater.SeatsPerRow,
            show.Theater.TheaterSeats.Select(s => new TheaterSeatDto(s.SeatRow, s.SeatNumber, s.Type)).ToList(),
            bookedSeats
        );
    }
}
