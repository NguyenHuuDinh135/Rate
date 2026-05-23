using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Bookings.Commands.UpdateBooking;

public sealed record UpdateBookingCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string SeatRow { get; init; } = string.Empty;
    public int SeatNumber { get; init; }
    public float Price { get; init; }
    public BookingStatus Status { get; init; }
}

public sealed class UpdateBookingCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateBookingCommand, Result>
{
    public async Task<Result> Handle(UpdateBookingCommand request, CancellationToken ct)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (booking is null)
            return Result.Failure(new[] { "Booking not found." });

        booking.SeatRow = request.SeatRow.Trim().ToUpperInvariant();
        booking.SeatNumber = request.SeatNumber;
        booking.Price = request.Price;
        booking.Status = request.Status;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
