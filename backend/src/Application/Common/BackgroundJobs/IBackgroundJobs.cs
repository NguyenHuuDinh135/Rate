using System.Threading;
using System.Threading.Tasks;

namespace backend.Application.Common.BackgroundJobs
{
    public interface IEmbeddingSyncJob
    {
        Task SyncMovieEmbeddingsAsync(CancellationToken ct = default);
        Task SyncReviewEmbeddingsAsync(CancellationToken ct = default);
    }

    public interface IReviewSummarizerJob
    {
        Task SummarizeMovieReviewsAsync(int movieId);
    }
}
