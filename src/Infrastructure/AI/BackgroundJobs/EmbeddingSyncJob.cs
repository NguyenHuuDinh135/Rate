using System;
using System.Linq;
using System.Threading.Tasks;
using backend.Application.Common.BackgroundJobs;
using backend.Application.Common.Interfaces;
using backend.Application.Common.Interfaces.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace backend.Infrastructure.AI.BackgroundJobs
{
    public class EmbeddingSyncJob : IEmbeddingSyncJob
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmbeddingProvider _embeddingProvider;
        private readonly ILogger<EmbeddingSyncJob> _logger;

        public EmbeddingSyncJob(
            IApplicationDbContext context,
            IEmbeddingProvider embeddingProvider,
            ILogger<EmbeddingSyncJob> logger)
        {
            _context = context;
            _embeddingProvider = embeddingProvider;
            _logger = logger;
        }

        public async Task SyncMovieEmbeddingsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Bắt đầu quét và sinh embedding cho Phim...");

            var moviesToUpdate = await _context.Movies
                .Where(m => m.Embedding == null)
                .OrderBy(m => m.Id)
                .Take(50) // Xử lý theo đợt
                .ToListAsync(ct);

            if (!moviesToUpdate.Any())
            {
                _logger.LogInformation("Không có phim nào cần sinh embedding.");
                return;
            }

            foreach (var movie in moviesToUpdate)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var textToEmbed = $"Title: {movie.Title}. Summary: {movie.Summary}";
                    var vectorArray = await _embeddingProvider.GenerateEmbeddingAsync(textToEmbed, ct);
                    
                    movie.Embedding = new Vector(vectorArray);
                    _logger.LogInformation($"Đã sinh embedding cho phim: {movie.Title}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Job SyncMovieEmbeddings đã bị hủy (Cancellation requested).");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Lỗi khi sinh embedding cho phim {movie.Title}");
                }
            }

            await _context.SaveChangesAsync(ct);
            _logger.LogInformation($"Hoàn tất cập nhật {moviesToUpdate.Count} phim.");
        }

        public async Task SyncReviewEmbeddingsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Bắt đầu quét và sinh embedding cho Reviews...");

            var reviewsToUpdate = await _context.Reviews
                .Where(r => r.Embedding == null)
                .OrderBy(r => r.Id)
                .Take(100)
                .ToListAsync(ct);

            if (!reviewsToUpdate.Any())
            {
                _logger.LogInformation("Không có review nào cần sinh embedding.");
                return;
            }

            foreach (var review in reviewsToUpdate)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var vectorArray = await _embeddingProvider.GenerateEmbeddingAsync(review.Content, ct);
                    review.Embedding = new Vector(vectorArray);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Job SyncReviewEmbeddings đã bị hủy.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Lỗi khi sinh embedding cho review ID {review.Id}");
                }
            }

            await _context.SaveChangesAsync(ct);
            _logger.LogInformation($"Hoàn tất cập nhật {reviewsToUpdate.Count} reviews.");
        }
    }
}
