using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using backend.Application.Common.Interfaces.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace backend.Infrastructure.AI.Providers
{
    public class GeminiLLMProvider : ILLMProvider
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletion;

        public GeminiLLMProvider(Kernel kernel)
        {
            _kernel = kernel;
            _chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        }

        public async IAsyncEnumerable<string> ChatAsync(string prompt, string? sessionId = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Note: In a real implementation, we would manage chat history based on sessionId.
            // For now, we'll just send a single prompt or use the history passed via the kernel if applicable.
            
            var history = new ChatHistory();
            history.AddUserMessage(prompt);

            await foreach (var content in _chatCompletion.GetStreamingChatMessageContentsAsync(history, kernel: _kernel, cancellationToken: cancellationToken))
            {
                if (content.Content != null)
                {
                    yield return content.Content;
                }
            }
        }

        public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var result = await _chatCompletion.GetChatMessageContentAsync(prompt, kernel: _kernel, cancellationToken: cancellationToken);
            return result.Content ?? string.Empty;
        }
    }
}
