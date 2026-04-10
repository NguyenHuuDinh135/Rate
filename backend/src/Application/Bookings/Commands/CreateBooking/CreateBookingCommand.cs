using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand : IRequest<Result<int>>
{
    public int ShowId { get; init; }
    public string SeatRow { get; init; } = string.Empty;
    public int SeatNumber { get; init; }
    public float Price { get; init; }
    public string? UserId { get; init; }
    public string? IdempotencyKey { get; init; }
}

public sealed class CreateBookingCommandHandler(
    IApplicationDbContext db,
    IUser user,
    IIdempotencyService idempotencyService,
    ILockService lockService)
    : IRequestHandler<CreateBookingCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var userId = request.UserId ?? user.Id;
        if (string.IsNullOrWhiteSpace(userId))
            return Result<int>.Failure(new[] { "User is required." });

        var seatKey = $"{request.ShowId}:{request.SeatRow}:{request.SeatNumber}";

        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var isNew = await idempotencyService.TryAcquireAsync(
                $"booking:{request.IdempotencyKey}", TimeSpan.FromMinutes(5), ct);
            if (!isNew)
                return Result<int>.Failure(new[] { "Duplicate booking request." });
        }

        using var lockHandle = await lockService.AcquireLockAsync($"booking-seat:{seatKey}", TimeSpan.FromSeconds(10), ct);
        if (lockHandle is null)
            return Result<int>.Failure(new[] { "Seat is being processed by another request." });

        var showExists = await db.Shows.AnyAsync(x => x.Id == request.ShowId, ct);
        if (!showExists)
            return Result<int>.Failure(new[] { "Show not found." });

        var isTaken = await db.Bookings.AnyAsync(
            x => x.ShowId == request.ShowId
                 && x.SeatRow == request.SeatRow
                 && x.SeatNumber == request.SeatNumber
                 && x.Status != BookingStatus.Cancelled, ct);
        if (isTaken)
            return Result<int>.Failure(new[] { "Seat already reserved." });

        var booking = new Domain.Entities.Booking
        {
            UserId = userId,
            ShowId = request.ShowId,
            SeatRow = request.SeatRow.Trim().ToUpperInvariant(),
            SeatNumber = request.SeatNumber,
            Price = request.Price,
            BookingDateTime = DateTime.UtcNow,
            Status = BookingStatus.Reserved
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);
        return Result<int>.Success(booking.Id);
    }
}
