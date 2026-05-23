using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Shows.Commands.UpdateShow;

public sealed record UpdateShowCommand : IRequest<Result>
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int MovieId { get; init; }
    public int TheaterId { get; init; }
    public ShowType Type { get; init; }
    public ShowStatus Status { get; init; }
}

public sealed class UpdateShowCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateShowCommand, Result>
{
    public async Task<Result> Handle(UpdateShowCommand request, CancellationToken ct)
    {
        var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (show is null)
            return Result.Failure(new[] { "Show not found." });

        show.Date = request.Date.Date;
        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.MovieId = request.MovieId;
        show.TheaterId = request.TheaterId;
        show.Type = request.Type;
        show.Status = request.Status;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
