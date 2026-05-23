using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Shows.Commands.DeleteShow;

public sealed record DeleteShowCommand(int Id) : IRequest<Result>;

public sealed class DeleteShowCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteShowCommand, Result>
{
    public async Task<Result> Handle(DeleteShowCommand request, CancellationToken ct)
    {
        var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (show is null)
            return Result.Failure(new[] { "Show not found." });

        db.Shows.Remove(show);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
