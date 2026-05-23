using backend.Application.Common.BackgroundJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace backend.Infrastructure.AI.BackgroundJobs;

public sealed class EmbeddingStartupHostedService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<EmbeddingStartupHostedService> logger) : BackgroundService
{
    private const int DefaultBatchSize = 50;
    private const int DefaultMaxBatches = 100;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue<bool>("Ai:Embeddings:SyncOnStartup"))
        {
            logger.LogDebug("Startup embedding sync is disabled.");
            return;
        }

        var startupDelaySeconds = Math.Max(
            0,
            configuration.GetValue<int?>("Ai:Embeddings:StartupDelaySeconds") ?? 2);

        if (startupDelaySeconds > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(startupDelaySeconds), stoppingToken);
        }

        var batchSize = Math.Clamp(
            configuration.GetValue<int?>("Ai:Embeddings:StartupBatchSize") ?? DefaultBatchSize,
            1,
            500);
        var maxBatches = Math.Clamp(
            configuration.GetValue<int?>("Ai:Embeddings:MaxStartupBatches") ?? DefaultMaxBatches,
            1,
            10_000);
        var force = configuration.GetValue<bool>("Ai:Embeddings:ForceSyncOnStartup");
        var model = configuration["Ai:Ollama:EmbeddingModelId"]
            ?? configuration["Ai:Gemini:EmbeddingModelId"]
            ?? "unknown";

        try
        {
            using var scope = scopeFactory.CreateScope();
            var embeddingSyncJob = scope.ServiceProvider.GetService<IEmbeddingSyncJob>();
            if (embeddingSyncJob is null)
            {
                logger.LogWarning("Startup embedding sync is enabled, but IEmbeddingSyncJob is not registered.");
                return;
            }

            logger.LogInformation(
                "Starting startup embedding sync. Model: {Model}. BatchSize: {BatchSize}. Force: {Force}.",
                model,
                batchSize,
                force);

            await SyncTargetUntilCompleteAsync(
                "movies",
                (job, ct) => job.SyncMovieEmbeddingsBatchAsync(batchSize, force, ct),
                embeddingSyncJob,
                maxBatches,
                force,
                stoppingToken);

            await SyncTargetUntilCompleteAsync(
                "reviews",
                (job, ct) => job.SyncReviewEmbeddingsBatchAsync(batchSize, force, ct),
                embeddingSyncJob,
                maxBatches,
                force,
                stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Startup embedding sync was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Startup embedding sync failed. The API will keep running; use /api/ai/embeddings/sync after the embedding model is available.");
        }
    }

    private async Task SyncTargetUntilCompleteAsync(
        string target,
        Func<IEmbeddingSyncJob, CancellationToken, Task<EmbeddingSyncResult>> syncBatch,
        IEmbeddingSyncJob embeddingSyncJob,
        int maxBatches,
        bool force,
        CancellationToken ct)
    {
        for (var batch = 1; batch <= maxBatches; batch++)
        {
            ct.ThrowIfCancellationRequested();

            var result = await syncBatch(embeddingSyncJob, ct);
            logger.LogInformation(
                "Startup embedding sync {Target} batch {Batch}: attempted {Attempted}, succeeded {Succeeded}, failed {Failed}, pending {PendingAfter}.",
                target,
                batch,
                result.Attempted,
                result.Succeeded,
                result.Failed,
                result.PendingAfter);

            if (result.Attempted == 0 || result.PendingAfter == 0)
            {
                return;
            }

            if (result.Failed > 0 && result.Succeeded == 0)
            {
                logger.LogWarning(
                    "Startup embedding sync for {Target} stopped because the current batch failed completely.",
                    target);
                return;
            }

            if (force)
            {
                logger.LogWarning(
                    "Force startup embedding sync for {Target} ran one batch. Use /api/ai/embeddings/sync with force=true for additional manual batches.",
                    target);
                return;
            }
        }

        logger.LogWarning(
            "Startup embedding sync for {Target} stopped after reaching MaxStartupBatches={MaxBatches}.",
            target,
            maxBatches);
    }
}
