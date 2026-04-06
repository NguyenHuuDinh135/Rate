using backend.Application.Common.Interfaces;
using backend.Infrastructure.Redis;
using StackExchange.Redis;

namespace backend.Infrastructure.Identity;

public sealed class RedisOneTimeTokenService(IConnectionMultiplexer multiplexer) : IOneTimeTokenService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public Task StoreAsync(string purpose, string subject, string token, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _db.StringSetAsync(RedisKeys.OneTimeToken(purpose, subject), token, ttl);

    public async Task<bool> ConsumeAsync(string purpose, string subject, string token, CancellationToken cancellationToken = default)
    {
        var key = RedisKeys.OneTimeToken(purpose, subject);
        var stored = await _db.StringGetAsync(key).ConfigureAwait(false);
        if (!stored.HasValue || stored.ToString() != token)
        {
            return false;
        }

        await _db.KeyDeleteAsync(key).ConfigureAwait(false);
        return true;
    }

}

