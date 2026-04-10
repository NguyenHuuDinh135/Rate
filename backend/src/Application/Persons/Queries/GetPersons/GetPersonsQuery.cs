using backend.Application.Common.Interfaces;

namespace backend.Application.Persons.Queries.GetPersons;

public sealed record PersonBriefDto(int Id, string FullName, byte Age, string PictureUrl);

public sealed record GetPersonsQuery : IRequest<IReadOnlyList<PersonBriefDto>>;

public sealed class GetPersonsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPersonsQuery, IReadOnlyList<PersonBriefDto>>
{
    public async Task<IReadOnlyList<PersonBriefDto>> Handle(GetPersonsQuery request, CancellationToken ct)
        => await db.Persons.AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new PersonBriefDto(x.Id, x.FullName, x.Age, x.PictureUrl))
            .ToListAsync(ct);
}
