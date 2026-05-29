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
        try
        {
            var value = await _db.StringGetAsync(Prefix(key)).ConfigureAwait(false);
            if (!value.HasValue)
            {
                return default;
            }

            var json = value.ToString();
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var expiry = ttl ?? TimeSpan.FromSeconds(_options.DefaultTtlSeconds);
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(Prefix(key), json, expiry).ConfigureAwait(false);
        }
        catch
        {
            // Gracefully ignore cache write failures
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.KeyDeleteAsync(Prefix(key)).ConfigureAwait(false);
        }
        catch
        {
            // Gracefully ignore cache remove failures
        }
    }

    private string Prefix(string key) => $"{_options.InstanceName}:{key}";
}

