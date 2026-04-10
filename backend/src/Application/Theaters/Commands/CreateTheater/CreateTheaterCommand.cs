using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Theaters.Commands.CreateTheater;

public sealed record CreateTheaterCommand(string Name, int NumOfRows, int SeatsPerRow, TheaterType Type)
    : IRequest<Result<int>>;

public sealed class CreateTheaterCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateTheaterCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTheaterCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.NumOfRows <= 0 || request.SeatsPerRow <= 0)
            return Result<int>.Failure(new[] { "Invalid theater payload." });

        var theater = new Theater
        {
            Name = request.Name.Trim(),
            NumOfRows = request.NumOfRows,
            SeatsPerRow = request.SeatsPerRow,
            Type = request.Type
        };
        db.Theaters.Add(theater);
        await db.SaveChangesAsync(ct);
        return Result<int>.Success(theater.Id);
    }
}
