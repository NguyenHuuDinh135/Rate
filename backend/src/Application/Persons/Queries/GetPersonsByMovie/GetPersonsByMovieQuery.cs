using backend.Application.Common.Interfaces;
using backend.Application.Persons.Queries.GetPersons;

namespace backend.Application.Persons.Queries.GetPersonsByMovie;

public sealed record GetPersonsByMovieQuery(int MovieId) : IRequest<IReadOnlyList<PersonBriefDto>>;

public sealed class GetPersonsByMovieQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPersonsByMovieQuery, IReadOnlyList<PersonBriefDto>>
{
    public async Task<IReadOnlyList<PersonBriefDto>> Handle(GetPersonsByMovieQuery request, CancellationToken ct)
        => await db.MoviePersons.AsNoTracking()
            .Where(x => x.MovieId == request.MovieId)
            .Select(x => new PersonBriefDto(x.Person.Id, x.Person.FullName, x.Person.Age, x.Person.PictureUrl))
            .ToListAsync(ct);
}
