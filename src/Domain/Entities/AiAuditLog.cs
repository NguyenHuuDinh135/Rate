namespace backend.Domain.Entities;

public class AiAuditLog : BaseEntity
{
    public string? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? PromptName { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public long LatencyMs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
