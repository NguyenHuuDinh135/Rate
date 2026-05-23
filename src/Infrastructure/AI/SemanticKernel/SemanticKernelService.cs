using System.Runtime.CompilerServices;
using backend.Application.AI.Interfaces;
using backend.Application.Common.Interfaces;
using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace backend.Infrastructure.AI.SemanticKernel;

public class SemanticKernelService(
    Kernel kernel,
    IApplicationDbContext dbContext,
    IPromptManager promptManager) : IAIService
{
    public async IAsyncEnumerable<string> ChatAsync(int sessionId, string userMessage, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var session = await dbContext.AiSessions
            .Include(s => s.Messages.OrderBy(m => m.Created))
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

        if (session == null) yield break;

        var chatHistory = new ChatHistory();
        
        // Add system prompt
        var systemPrompt = await promptManager.GetPromptTemplateAsync("DefaultSystemPrompt", ct);
        chatHistory.AddSystemMessage(systemPrompt);

        foreach (var msg in session.Messages)
        {
            if (msg.Role == "user") chatHistory.AddUserMessage(msg.Content);
            else if (msg.Role == "assistant") chatHistory.AddAssistantMessage(msg.Content);
        }

        chatHistory.AddUserMessage(userMessage);

        // Save user message to DB
        session.Messages.Add(new AiMessage { Role = "user", Content = userMessage });
        await dbContext.SaveChangesAsync(ct);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var fullResponse = "";

        await foreach (var content in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, kernel: kernel, cancellationToken: ct))
        {
            if (content.Content != null)
            {
                fullResponse += content.Content;
                yield return content.Content;
            }
        }

        // Save assistant response to DB
        session.Messages.Add(new AiMessage { Role = "assistant", Content = fullResponse });
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<string> SummarizeReviewsAsync(int movieId, CancellationToken ct = default)
    {
        // Implementation for summarization logic
        return "Not implemented yet";
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        // Implementation for embedding logic
        return [];
    }
}
