using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Theaters.Commands.DeleteTheater;

public sealed record DeleteTheaterCommand(int Id) : IRequest<Result>;

public sealed class DeleteTheaterCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteTheaterCommand, Result>
{
    public async Task<Result> Handle(DeleteTheaterCommand request, CancellationToken ct)
    {
        var theater = await db.Theaters.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (theater is null)
            return Result.Failure(new[] { "Theater not found." });

        db.Theaters.Remove(theater);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
