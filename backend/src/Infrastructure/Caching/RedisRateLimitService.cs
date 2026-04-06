using backend.Application.Common.Interfaces;
using backend.Infrastructure.Redis;
using StackExchange.Redis;

namespace backend.Infrastructure.Caching;

public sealed class RedisRateLimitService(IConnectionMultiplexer multiplexer) : IRateLimitService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public async Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var redisKey = RedisKeys.RateLimit(key);
        var count = await _db.StringIncrementAsync(redisKey).ConfigureAwait(false);

        if (count == 1)
        {
            await _db.KeyExpireAsync(redisKey, window).ConfigureAwait(false);
        }

        return count <= limit;
    }
}

