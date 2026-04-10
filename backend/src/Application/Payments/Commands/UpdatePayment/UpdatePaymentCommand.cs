using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Payments.Commands.UpdatePayment;

public sealed record UpdatePaymentCommand : IRequest<Result>
{
    public int Id { get; init; }
    public int Amount { get; init; }
    public PaymentMethod Method { get; init; }
}

public sealed class UpdatePaymentCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdatePaymentCommand, Result>
{
    public async Task<Result> Handle(UpdatePaymentCommand request, CancellationToken ct)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (payment is null)
            return Result.Failure(new[] { "Payment not found." });

        payment.Amount = request.Amount;
        payment.Method = request.Method;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
