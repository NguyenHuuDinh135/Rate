namespace backend.Application.Common.Interfaces;

public interface IRevokeTokenService
{
    Task RevokeAsync(string jti, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default);
}

