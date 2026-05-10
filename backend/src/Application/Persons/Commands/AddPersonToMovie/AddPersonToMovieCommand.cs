using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Persons.Commands.AddPersonToMovie;

public sealed record AddPersonToMovieCommand(int MovieId, int PersonId, RoleType RoleType) : IRequest<Result>;

public sealed class AddPersonToMovieCommandHandler(IApplicationDbContext db)
    : IRequestHandler<AddPersonToMovieCommand, Result>
{
    public async Task<Result> Handle(AddPersonToMovieCommand request, CancellationToken ct)
    {
        var exists = await db.MoviePersons.AnyAsync(
            x => x.MovieId == request.MovieId && x.PersonId == request.PersonId && x.RoleType == request.RoleType, ct);
        if (exists)
            return Result.Failure(new[] { "Person already assigned to this movie with this role." });

        db.MoviePersons.Add(new MoviePerson
        {
            MovieId = request.MovieId,
            PersonId = request.PersonId,
            RoleType = request.RoleType
        });
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
