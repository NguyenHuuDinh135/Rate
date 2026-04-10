using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Genres.Commands.UpdateGenre;

public sealed record UpdateGenreCommand(int Id, string Name) : IRequest<Result>;

public sealed class UpdateGenreCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<UpdateGenreCommand, Result>
{
    public async Task<Result> Handle(UpdateGenreCommand request, CancellationToken ct)
    {
        var entity = await db.Genres.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null)
            return Result.Failure(new[] { "Genre not found." });

        entity.Name = request.Name.Trim();
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("genres:all", ct);
        return Result.Success();
    }
}
