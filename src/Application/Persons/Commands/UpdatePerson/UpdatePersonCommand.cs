using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Persons.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(int Id, string FullName, byte Age, string PictureUrl) : IRequest<Result>;

public sealed class UpdatePersonCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdatePersonCommand, Result>
{
    public async Task<Result> Handle(UpdatePersonCommand request, CancellationToken ct)
    {
        var person = await db.Persons.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (person is null)
            return Result.Failure(new[] { "Person not found." });

        person.FullName = request.FullName.Trim();
        person.Age = request.Age;
        person.PictureUrl = request.PictureUrl ?? string.Empty;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
