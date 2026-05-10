namespace backend.Application.Common.Interfaces;

public interface IOneTimeTokenService
{
    Task StoreAsync(string purpose, string subject, string token, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task<bool> ConsumeAsync(string purpose, string subject, string token, CancellationToken cancellationToken = default);
}

