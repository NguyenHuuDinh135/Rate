using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand : IRequest<Result<int>>
{
    public int Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int ShowId { get; init; }
}

public sealed class CreatePaymentCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreatePaymentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var showExists = await db.Shows.AnyAsync(x => x.Id == request.ShowId, ct);
        if (!showExists)
            return Result<int>.Failure(new[] { "Show not found." });

        var payment = new Payment
        {
            Amount = request.Amount,
            PaymentDateTime = DateTime.UtcNow,
            Method = request.Method,
            UserId = request.UserId,
            ShowId = request.ShowId
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync(ct);
        return Result<int>.Success(payment.Id);
    }
}
