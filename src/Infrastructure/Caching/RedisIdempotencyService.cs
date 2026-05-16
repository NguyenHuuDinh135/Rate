using backend.Application.Common.Interfaces;
using backend.Infrastructure.Redis;
using StackExchange.Redis;

namespace backend.Infrastructure.Caching;

public sealed class RedisIdempotencyService(IConnectionMultiplexer multiplexer) : IIdempotencyService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _db.StringSetAsync(RedisKeys.Idempotency(key), "1", ttl, when: When.NotExists);
}

