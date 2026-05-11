using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand : IRequest<Result>
{
    public string Id { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public sealed class UpdateUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        return await identityService.UpdateUserAsync(request.Id, request.FullName, request.Email);
    }
}
