using backend.Application.AI.Interfaces;
using backend.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.AI.Prompts;

public class DatabasePromptManager(IApplicationDbContext dbContext) : IPromptManager
{
    public async Task<string> GetPromptTemplateAsync(string name, CancellationToken ct = default)
    {
        var prompt = await dbContext.AiPrompts
            .Where(p => p.Name == name && p.IsActive)
            .OrderByDescending(p => p.Version)
            .FirstOrDefaultAsync(ct);

        if (prompt != null) return prompt.Template;

        // Fallback for development
        if (name == "DefaultSystemPrompt")
        {
            return "Bạn là trợ lý ảo thông minh của hệ thống đặt vé xem phim Rate. Hãy giúp người dùng tìm phim, xem lịch chiếu và đặt vé.";
        }

        return string.Empty;
    }

    public Task<string> RenderPromptAsync(string name, object data, CancellationToken ct = default)
    {
        // For now, simple replacement. Could use Handlebars or Liquid in the future.
        return GetPromptTemplateAsync(name, ct);
    }
}
