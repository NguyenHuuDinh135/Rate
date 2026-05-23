namespace backend.Application.Common.Interfaces;

public interface IRefreshTokenStore
{
    Task StoreAsync(string userId, string refreshToken, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);

    Task<string?> GetUserIdAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeAsync(string userId, CancellationToken cancellationToken = default);
}

