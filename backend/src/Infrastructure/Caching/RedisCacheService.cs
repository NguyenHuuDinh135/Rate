using System.Text.Json;
using backend.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace backend.Infrastructure.Caching;

public sealed class RedisCacheService(
    IConnectionMultiplexer multiplexer,
    IOptions<RedisOptions> options)
    : ICacheService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();
    private readonly RedisOptions _options = options.Value;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(Prefix(key)).ConfigureAwait(false);
        if (!value.HasValue)
        {
            return default;
        }

        var json = value.ToString();
        return JsonSerializer.Deserialize<T>(json);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        var expiry = ttl ?? TimeSpan.FromSeconds(_options.DefaultTtlSeconds);
        var json = JsonSerializer.Serialize(value);
        return _db.StringSetAsync(Prefix(key), json, expiry);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _db.KeyDeleteAsync(Prefix(key));

    private string Prefix(string key) => $"{_options.InstanceName}:{key}";
}

