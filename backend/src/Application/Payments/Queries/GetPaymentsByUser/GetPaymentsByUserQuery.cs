using backend.Application.Common.Interfaces;
using backend.Application.Payments.Queries.GetPayments;

namespace backend.Application.Payments.Queries.GetPaymentsByUser;

public sealed record GetPaymentsByUserQuery(string UserId) : IRequest<IReadOnlyList<PaymentBriefDto>>;

public sealed class GetPaymentsByUserQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPaymentsByUserQuery, IReadOnlyList<PaymentBriefDto>>
{
    public async Task<IReadOnlyList<PaymentBriefDto>> Handle(GetPaymentsByUserQuery request, CancellationToken ct)
        => await db.Payments.AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.PaymentDateTime)
            .Select(x => new PaymentBriefDto(x.Id, x.Amount, x.PaymentDateTime, x.Method, x.UserId, x.ShowId))
            .ToListAsync(ct);
}
