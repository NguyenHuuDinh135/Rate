using backend.Application.Common.Interfaces;
using backend.Application.Payments.Queries.GetPayments;

namespace backend.Application.Payments.Queries.GetPaymentById;

public sealed record GetPaymentByIdQuery(int Id) : IRequest<PaymentBriefDto?>;

public sealed class GetPaymentByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPaymentByIdQuery, PaymentBriefDto?>
{
    public async Task<PaymentBriefDto?> Handle(GetPaymentByIdQuery request, CancellationToken ct)
        => await db.Payments.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new PaymentBriefDto(x.Id, x.Amount, x.PaymentDateTime, x.Method, x.UserId, x.ShowId))
            .FirstOrDefaultAsync(ct);
}
