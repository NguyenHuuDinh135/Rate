using backend.Application.Common.BackgroundJobs;
using backend.Infrastructure.AI.BackgroundJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Application.FunctionalTests.AI;

public class EmbeddingStartupHostedServiceTests
{
    [Test]
    public async Task StartupService_ShouldSyncPendingMovieAndReviewEmbeddings_WhenEnabled()
    {
        var syncJob = new RecordingEmbeddingSyncJob(
            movieResults:
            [
                CreateResult("movies", attempted: 2, succeeded: 2, failed: 0, pendingAfter: 2),
                CreateResult("movies", attempted: 2, succeeded: 2, failed: 0, pendingAfter: 0)
            ],
            reviewResults:
            [
                CreateResult("reviews", attempted: 1, succeeded: 1, failed: 0, pendingAfter: 0)
            ]);

        using var services = new ServiceCollection()
            .AddSingleton<IEmbeddingSyncJob>(syncJob)
            .BuildServiceProvider();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ai:Embeddings:SyncOnStartup"] = "true",
                ["Ai:Embeddings:StartupDelaySeconds"] = "0",
                ["Ai:Embeddings:StartupBatchSize"] = "25",
                ["Ai:Embeddings:MaxStartupBatches"] = "5",
                ["Ai:Ollama:EmbeddingModelId"] = "bge-m3:latest"
            })
            .Build();

        var service = new EmbeddingStartupHostedService(
            services.GetRequiredService<IServiceScopeFactory>(),
            configuration,
            NullLogger<EmbeddingStartupHostedService>.Instance);

        await service.StartAsync(CancellationToken.None);
        await WaitUntilAsync(() => syncJob.CallCount >= 3);
        await service.StopAsync(CancellationToken.None);

        var calls = syncJob.Calls;
        calls.Select(call => call.Target).ShouldBe(new[] { "movies", "movies", "reviews" });
        calls.Select(call => call.BatchSize).ShouldAllBe(batchSize => batchSize == 25);
        calls.Select(call => call.Force).ShouldAllBe(force => force == false);
    }

    private static EmbeddingSyncResult CreateResult(
        string target,
        int attempted,
        int succeeded,
        int failed,
        int pendingAfter)
        => new(
            target,
            attempted + pendingAfter,
            attempted + pendingAfter,
            attempted,
            succeeded,
            failed,
            pendingAfter,
            1024,
            []);

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(20);
        }

        condition().ShouldBeTrue();
    }

    private sealed class RecordingEmbeddingSyncJob(
        IReadOnlyCollection<EmbeddingSyncResult> movieResults,
        IReadOnlyCollection<EmbeddingSyncResult> reviewResults) : IEmbeddingSyncJob
    {
        private readonly object _gate = new();
        private readonly Queue<EmbeddingSyncResult> _movieResults = new(movieResults);
        private readonly Queue<EmbeddingSyncResult> _reviewResults = new(reviewResults);
        private readonly List<(string Target, int BatchSize, bool Force)> _calls = [];

        public int CallCount
        {
            get
            {
                lock (_gate)
                {
                    return _calls.Count;
                }
            }
        }

        public IReadOnlyList<(string Target, int BatchSize, bool Force)> Calls
        {
            get
            {
                lock (_gate)
                {
                    return _calls.ToList();
                }
            }
        }

        public Task SyncMovieEmbeddingsAsync(CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task SyncReviewEmbeddingsAsync(CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<EmbeddingSyncResult> SyncMovieEmbeddingsBatchAsync(
            int batchSize = 50,
            bool force = false,
            CancellationToken ct = default)
        {
            AddCall("movies", batchSize, force);
            return Task.FromResult(_movieResults.Count > 0
                ? _movieResults.Dequeue()
                : CreateResult("movies", attempted: 0, succeeded: 0, failed: 0, pendingAfter: 0));
        }

        public Task<EmbeddingSyncResult> SyncReviewEmbeddingsBatchAsync(
            int batchSize = 100,
            bool force = false,
            CancellationToken ct = default)
        {
            AddCall("reviews", batchSize, force);
            return Task.FromResult(_reviewResults.Count > 0
                ? _reviewResults.Dequeue()
                : CreateResult("reviews", attempted: 0, succeeded: 0, failed: 0, pendingAfter: 0));
        }

        private void AddCall(string target, int batchSize, bool force)
        {
            lock (_gate)
            {
                _calls.Add((target, batchSize, force));
            }
        }
    }
}
