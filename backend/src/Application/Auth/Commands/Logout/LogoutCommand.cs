using backend.Application.Common.Interfaces;

namespace backend.Application.Auth.Commands.Logout;

public sealed record LogoutCommand : IRequest<bool>
{
    public string Jti { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; init; }
}

public sealed class LogoutCommandHandler(IRevokeTokenService tokenRevocationService)
    : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Jti))
        {
            return false;
        }

        var ttl = request.ExpiresAtUtc - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return true;
        }

        await tokenRevocationService.RevokeAsync(request.Jti, ttl, cancellationToken);
        return true;
    }
}

