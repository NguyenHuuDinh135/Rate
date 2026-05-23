using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Persons.Commands.DeletePerson;

public sealed record DeletePersonCommand(int Id) : IRequest<Result>;

public sealed class DeletePersonCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeletePersonCommand, Result>
{
    public async Task<Result> Handle(DeletePersonCommand request, CancellationToken ct)
    {
        var person = await db.Persons.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (person is null)
            return Result.Failure(new[] { "Person not found." });

        db.Persons.Remove(person);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
