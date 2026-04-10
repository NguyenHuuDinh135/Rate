using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Movies.Commands.DeleteMovie;

public sealed record DeleteMovieCommand(int Id) : IRequest<Result>;

public sealed class DeleteMovieCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<DeleteMovieCommand, Result>
{
    public async Task<Result> Handle(DeleteMovieCommand request, CancellationToken ct)
    {
        var movie = await db.Movies.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (movie is null)
            return Result.Failure(new[] { "Movie not found." });

        db.Movies.Remove(movie);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("movies:all", ct);
        await cache.RemoveAsync($"movies:{request.Id}", ct);
        return Result.Success();
    }
}
