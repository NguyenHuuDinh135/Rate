using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Shows.Commands.CreateShow;

public sealed record CreateShowCommand : IRequest<Result<int>>
{
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int MovieId { get; init; }
    public int TheaterId { get; init; }
    public ShowType Type { get; init; }
}

public sealed class CreateShowCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateShowCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateShowCommand request, CancellationToken ct)
    {
        if (request.EndTime <= request.StartTime)
            return Result<int>.Failure(new[] { "EndTime must be after StartTime." });

        var movieExists = await db.Movies.AnyAsync(x => x.Id == request.MovieId, ct);
        var theaterExists = await db.Theaters.AnyAsync(x => x.Id == request.TheaterId, ct);
        if (!movieExists || !theaterExists)
            return Result<int>.Failure(new[] { "Movie or Theater not found." });

        var show = new Show
        {
            Date = request.Date.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MovieId = request.MovieId,
            TheaterId = request.TheaterId,
            Type = request.Type,
            Status = ShowStatus.Free
        };

        db.Shows.Add(show);
        await db.SaveChangesAsync(ct);
        return Result<int>.Success(show.Id);
    }
}
