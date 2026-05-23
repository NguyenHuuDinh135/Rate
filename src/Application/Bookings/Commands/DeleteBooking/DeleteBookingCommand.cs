using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Bookings.Commands.DeleteBooking;

public sealed record DeleteBookingCommand(int Id) : IRequest<Result>;

public sealed class DeleteBookingCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteBookingCommand, Result>
{
    public async Task<Result> Handle(DeleteBookingCommand request, CancellationToken ct)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (booking is null)
            return Result.Failure(new[] { "Booking not found." });

        db.Bookings.Remove(booking);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
