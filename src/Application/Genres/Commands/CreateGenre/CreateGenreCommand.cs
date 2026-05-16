using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;

namespace backend.Application.Genres.Commands.CreateGenre;

public sealed record CreateGenreCommand(string Name) : IRequest<Result<int>>;

public sealed class CreateGenreCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<CreateGenreCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateGenreCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<int>.Failure(new[] { "Genre name is required." });

        var entity = new Genre { Name = request.Name.Trim() };
        db.Genres.Add(entity);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("genres:all", ct);
        return Result<int>.Success(entity.Id);
    }
}
