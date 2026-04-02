namespace backend.Application.Common.Interfaces;

public interface ILockService
{
    Task<IDisposable?> AcquireLockAsync(string resourceKey, TimeSpan timeout, CancellationToken cancellationToken = default);
    
}
