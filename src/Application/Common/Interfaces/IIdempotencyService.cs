namespace backend.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
}

