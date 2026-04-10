using backend.Application.Common.Interfaces;
using backend.Application.Persons.Queries.GetPersons;

namespace backend.Application.Persons.Queries.GetPersonById;

public sealed record GetPersonByIdQuery(int Id) : IRequest<PersonBriefDto?>;

public sealed class GetPersonByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPersonByIdQuery, PersonBriefDto?>
{
    public async Task<PersonBriefDto?> Handle(GetPersonByIdQuery request, CancellationToken ct)
        => await db.Persons.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new PersonBriefDto(x.Id, x.FullName, x.Age, x.PictureUrl))
            .FirstOrDefaultAsync(ct);
}
