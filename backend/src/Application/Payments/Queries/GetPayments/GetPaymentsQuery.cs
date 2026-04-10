using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Payments.Queries.GetPayments;

public sealed record PaymentBriefDto(
    int Id, int Amount, DateTime PaymentDateTime,
    PaymentMethod Method, string UserId, int ShowId);

public sealed record GetPaymentsQuery : IRequest<IReadOnlyList<PaymentBriefDto>>;

public sealed class GetPaymentsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPaymentsQuery, IReadOnlyList<PaymentBriefDto>>
{
    public async Task<IReadOnlyList<PaymentBriefDto>> Handle(GetPaymentsQuery request, CancellationToken ct)
        => await db.Payments.AsNoTracking()
            .OrderByDescending(x => x.PaymentDateTime)
            .Select(x => new PaymentBriefDto(x.Id, x.Amount, x.PaymentDateTime, x.Method, x.UserId, x.ShowId))
            .ToListAsync(ct);
}
