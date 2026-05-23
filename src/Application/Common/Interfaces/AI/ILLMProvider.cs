using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Application.Common.Interfaces.AI
{
    public interface ILLMProvider
    {
        IAsyncEnumerable<string> ChatAsync(string prompt, string? sessionId = null, CancellationToken cancellationToken = default);
        Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
