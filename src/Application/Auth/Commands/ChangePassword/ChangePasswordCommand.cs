using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand : IRequest<Result>
{
    public string UserId { get; init; } = string.Empty;
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ChangePasswordCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var ok = await authenticationService.ChangePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        return ok
            ? Result.Success()
            : Result.Failure(new[] { "Change password failed." });
    }
}

