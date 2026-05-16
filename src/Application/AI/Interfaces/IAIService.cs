using backend.Domain.Entities;

namespace backend.Application.AI.Interfaces;

public interface IAIService
{
    IAsyncEnumerable<string> ChatAsync(int sessionId, string userMessage, CancellationToken ct = default);
    Task<string> SummarizeReviewsAsync(int movieId, CancellationToken ct = default);
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default);
}

public interface IPromptManager
{
    Task<string> GetPromptTemplateAsync(string name, CancellationToken ct = default);
    Task<string> RenderPromptAsync(string name, object data, CancellationToken ct = default);
}
