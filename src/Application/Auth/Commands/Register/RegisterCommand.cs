using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.Register;

public record RegisterRequest : IRequest<Result>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class RegisterCommand(IIdentityService identityService) : IRequestHandler<RegisterRequest, Result>
{
    public async Task<Result> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var (result, _) = await identityService.CreateUserAsync(
            request.FullName,
            request.Email,
            request.Password);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors);
    }
}
