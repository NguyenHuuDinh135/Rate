namespace backend.Domain.Entities;

public class AiSession : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    
    public IList<AiMessage> Messages { get; private set; } = new List<AiMessage>();
}
