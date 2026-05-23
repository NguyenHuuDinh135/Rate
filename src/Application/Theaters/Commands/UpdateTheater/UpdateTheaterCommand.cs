using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Theaters.Commands.UpdateTheater;

public sealed record UpdateTheaterCommand(int Id, string Name, int NumOfRows, int SeatsPerRow, TheaterType Type)
    : IRequest<Result>;

public sealed class UpdateTheaterCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateTheaterCommand, Result>
{
    public async Task<Result> Handle(UpdateTheaterCommand request, CancellationToken ct)
    {
        var theater = await db.Theaters.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (theater is null)
            return Result.Failure(new[] { "Theater not found." });

        theater.Name = request.Name.Trim();
        theater.NumOfRows = request.NumOfRows;
        theater.SeatsPerRow = request.SeatsPerRow;
        theater.Type = request.Type;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
