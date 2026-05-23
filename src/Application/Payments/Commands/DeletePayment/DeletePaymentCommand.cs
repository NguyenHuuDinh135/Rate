using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Payments.Commands.DeletePayment;

public sealed record DeletePaymentCommand(int Id) : IRequest<Result>;

public sealed class DeletePaymentCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeletePaymentCommand, Result>
{
    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (payment is null)
            return Result.Failure(new[] { "Payment not found." });

        db.Payments.Remove(payment);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
