namespace backend.Application.Common.Interfaces;

public interface IRateLimitService
{
    Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window, CancellationToken cancellationToken = default);
}

