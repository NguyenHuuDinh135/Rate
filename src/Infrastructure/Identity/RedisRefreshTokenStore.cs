using backend.Application.Common.Interfaces;
using StackExchange.Redis;
using backend.Infrastructure.Redis;

namespace backend.Infrastructure.Identity;

public sealed class RedisRefreshTokenStore(IConnectionMultiplexer multiplexer) : IRefreshTokenStore
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public Task StoreAsync(string userId, string refreshToken, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _db.StringSetAsync(RedisKeys.RefreshToken(userId), refreshToken, ttl);

    public async Task<bool> ValidateAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await _db.StringGetAsync(RedisKeys.RefreshToken(userId)).ConfigureAwait(false);
        return stored.HasValue && stored.ToString() == refreshToken;
    }

    public Task RevokeAsync(string userId, CancellationToken cancellationToken = default)
        => _db.KeyDeleteAsync(RedisKeys.RefreshToken(userId));
}

