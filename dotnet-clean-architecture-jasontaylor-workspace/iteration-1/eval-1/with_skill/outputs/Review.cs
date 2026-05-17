using Pgvector;
using backend.Domain.Common;

namespace backend.Domain.Entities;

public class Review : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
