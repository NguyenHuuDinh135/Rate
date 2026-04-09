using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.Login;

public sealed record LoginCommand : IRequest<AuthTokenResult?>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginCommandHandler(IAuthenticationService authService)
    : IRequestHandler<LoginCommand, AuthTokenResult?>
{
    public Task<AuthTokenResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password, cancellationToken);
}

