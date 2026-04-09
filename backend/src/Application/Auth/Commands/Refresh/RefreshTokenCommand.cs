using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.Refresh;

public sealed record RefreshTokenCommand : IRequest<AuthTokenResult?>
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

public sealed class RefreshTokenCommandHandler(IAuthenticationService authService)
    : IRequestHandler<RefreshTokenCommand, AuthTokenResult?>
{
    public Task<AuthTokenResult?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => authService.RefreshAsync(request.AccessToken, request.RefreshToken, cancellationToken);
}

