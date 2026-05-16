using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;

namespace backend.Application.Persons.Commands.CreatePerson;

public sealed record CreatePersonCommand(string FullName, byte Age, string PictureUrl) : IRequest<Result<int>>;

public sealed class CreatePersonCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreatePersonCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePersonCommand request, CancellationToken ct)
    {
        var person = new Person
        {
            FullName = request.FullName.Trim(),
            Age = request.Age,
            PictureUrl = request.PictureUrl ?? string.Empty
        };
        db.Persons.Add(person);
        await db.SaveChangesAsync(ct);
        return Result<int>.Success(person.Id);
    }
}
