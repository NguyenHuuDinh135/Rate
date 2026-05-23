using backend.Application.Common.Interfaces;
using StackExchange.Redis;
using backend.Infrastructure.Redis;

namespace backend.Infrastructure.Identity;

public sealed class RedisRefreshTokenStore(IConnectionMultiplexer multiplexer) : IRefreshTokenStore
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public async Task StoreAsync(string userId, string refreshToken, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        await _db.StringSetAsync(RedisKeys.RefreshToken(userId), refreshToken, ttl);
        await _db.StringSetAsync(RedisKeys.RefreshTokenMap(refreshToken), userId, ttl);
    }

    public async Task<bool> ValidateAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await _db.StringGetAsync(RedisKeys.RefreshToken(userId)).ConfigureAwait(false);
        return stored.HasValue && stored.ToString() == refreshToken;
    }

    public async Task<string?> GetUserIdAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = await _db.StringGetAsync(RedisKeys.RefreshTokenMap(refreshToken)).ConfigureAwait(false);
        return userId.HasValue ? userId.ToString() : null;
    }

    public async Task RevokeAsync(string userId, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _db.StringGetAsync(RedisKeys.RefreshToken(userId)).ConfigureAwait(false);
        if (refreshToken.HasValue)
        {
            await _db.KeyDeleteAsync(RedisKeys.RefreshTokenMap(refreshToken.ToString()));
        }
        await _db.KeyDeleteAsync(RedisKeys.RefreshToken(userId));
    }
}

