using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Application.Common.Interfaces.AI;
using Microsoft.Extensions.AI;

namespace backend.Infrastructure.AI.Providers
{
    public class OllamaEmbeddingProvider : IEmbeddingProvider
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;
        private const string ModelId = "bge-m3";
        private const int Dimensions = 1024;

        public OllamaEmbeddingProvider(IEmbeddingGenerator<string, Embedding<float>> generator)
        {
            _generator = generator;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new float[Dimensions];
            }

            var result = await _generator.GenerateAsync([text], cancellationToken: cancellationToken);
            
            if (result.Count > 0)
            {
                return result[0].Vector.ToArray();
            }

            return new float[Dimensions];
        }

        public int GetEmbeddingDimension() => Dimensions;
    }
}
