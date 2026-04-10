using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Persons.Commands.RemovePersonFromMovie;

public sealed record RemovePersonFromMovieCommand(int MovieId, int PersonId) : IRequest<Result>;

public sealed class RemovePersonFromMovieCommandHandler(IApplicationDbContext db)
    : IRequestHandler<RemovePersonFromMovieCommand, Result>
{
    public async Task<Result> Handle(RemovePersonFromMovieCommand request, CancellationToken ct)
    {
        var mp = await db.MoviePersons.FirstOrDefaultAsync(
            x => x.MovieId == request.MovieId && x.PersonId == request.PersonId, ct);
        if (mp is null)
            return Result.Failure(new[] { "Person-movie association not found." });

        db.MoviePersons.Remove(mp);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
