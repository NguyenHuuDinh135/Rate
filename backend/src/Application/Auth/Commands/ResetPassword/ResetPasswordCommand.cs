using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
    public string ResetToken { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ResetPasswordCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var ok = await authenticationService.ResetPasswordAsync(
            request.Email,
            request.ResetToken,
            request.NewPassword,
            cancellationToken);

        return ok
            ? Result.Success()
            : Result.Failure(new[] { "Reset password failed or token invalid." });
    }
}

