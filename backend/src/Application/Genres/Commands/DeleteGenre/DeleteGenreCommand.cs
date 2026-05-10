using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Genres.Commands.DeleteGenre;

public sealed record DeleteGenreCommand(int Id) : IRequest<Result>;

public sealed class DeleteGenreCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<DeleteGenreCommand, Result>
{
    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken ct)
    {
        var entity = await db.Genres.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null)
            return Result.Failure(new[] { "Genre not found." });

        db.Genres.Remove(entity);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("genres:all", ct);
        return Result.Success();
    }
}
