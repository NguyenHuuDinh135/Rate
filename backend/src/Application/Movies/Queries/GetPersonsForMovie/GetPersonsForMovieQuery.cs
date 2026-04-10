using backend.Application.Common.Interfaces;
using backend.Domain.Enums;

namespace backend.Application.Movies.Queries.GetPersonsForMovie;

public sealed record MoviePersonDto(int PersonId, string FullName, string PictureUrl, RoleType RoleType);

public sealed record GetPersonsForMovieQuery(int MovieId) : IRequest<IReadOnlyList<MoviePersonDto>>;

public sealed class GetPersonsForMovieQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPersonsForMovieQuery, IReadOnlyList<MoviePersonDto>>
{
    public async Task<IReadOnlyList<MoviePersonDto>> Handle(GetPersonsForMovieQuery request, CancellationToken ct)
        => await db.MoviePersons.AsNoTracking()
            .Where(x => x.MovieId == request.MovieId)
            .Select(x => new MoviePersonDto(x.PersonId, x.Person.FullName, x.Person.PictureUrl, x.RoleType))
            .ToListAsync(ct);
}
