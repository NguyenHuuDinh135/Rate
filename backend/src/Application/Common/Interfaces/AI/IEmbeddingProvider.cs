using System.Threading;
using System.Threading.Tasks;

namespace backend.Application.Common.Interfaces.AI
{
    public interface IEmbeddingProvider
    {
        Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
        int GetEmbeddingDimension();
    }
}
