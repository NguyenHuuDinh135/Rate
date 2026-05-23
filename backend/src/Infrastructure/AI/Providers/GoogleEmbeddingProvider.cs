using backend.Application.Common.Interfaces.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Embeddings;

namespace backend.Infrastructure.AI.Providers;

#pragma warning disable SKEXP0001
public class GoogleEmbeddingProvider(
    ITextEmbeddingGenerationService embeddingService,
    IConfiguration configuration) : IEmbeddingProvider
{
    private readonly int _dimensions = configuration.GetValue<int?>("Ai:EmbeddingDimensions") ?? 1024;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new float[_dimensions];
        }

        var result = await embeddingService.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        var vector = result.ToArray();

        if (vector.Length != _dimensions)
        {
            throw new InvalidOperationException(
                $"Embedding dimension mismatch. Expected {_dimensions}, got {vector.Length}. " +
                "Update Ai:EmbeddingDimensions or the pgvector column size.");
        }

        return vector;
    }

    public int GetEmbeddingDimension() => _dimensions;
}
#pragma warning restore SKEXP0001
