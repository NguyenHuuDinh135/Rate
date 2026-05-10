namespace backend.Domain.Entities;

public class AiPrompt : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}
