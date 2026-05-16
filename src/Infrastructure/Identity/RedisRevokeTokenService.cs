using backend.Application.Common.Interfaces;
using backend.Infrastructure.Redis;
using StackExchange.Redis;

namespace backend.Infrastructure.Identity;

public sealed class RedisTokenRevocationService(IConnectionMultiplexer multiplexer) : IRevokeTokenService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public Task RevokeAsync(string jti, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _db.StringSetAsync(RedisKeys.RevokedToken(jti), "1", ttl);

    public Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default)
        => _db.KeyExistsAsync(RedisKeys.RevokedToken(jti));
}

