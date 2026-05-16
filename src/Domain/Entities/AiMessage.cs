namespace backend.Domain.Entities;

public class AiMessage : BaseAuditableEntity
{
    public int SessionId { get; set; }
    public string Role { get; set; } = string.Empty; // system, user, assistant, tool
    public string Content { get; set; } = string.Empty;
    public int? TokenCount { get; set; }
    public string? ToolCallId { get; set; }
    public string? ToolName { get; set; }

    public AiSession Session { get; set; } = null!;
}
