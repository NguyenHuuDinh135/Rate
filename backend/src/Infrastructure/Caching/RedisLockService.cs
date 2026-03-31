using backend.Application.Common.Interfaces;
using Medallion.Threading;

namespace backend.Infrastructure.Caching;

public class RedisLockService : ILockService
{
    private readonly IDistributedLockProvider _lockProvider;   
    public RedisLockService(IDistributedLockProvider lockProvider)
    {
        _lockProvider = lockProvider;
    }
    public async Task<IDisposable?> AcquireLockAsync(string resourceKey, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        // TryAcquireAsync sẽ không throw exception nếu hết giờ, nó chỉ trả về null
        // Rất an toàn để handle logic báo lỗi cho User
        return await _lockProvider.AcquireLockAsync(resourceKey, timeout, cancellationToken);
    }
}
