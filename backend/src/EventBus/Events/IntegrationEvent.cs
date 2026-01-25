namespace EventBus.Events;

/// <summary>
/// Base class cho tất cả Integration Event
/// Dùng để giao tiếp giữa các microservice thông qua EventBus
/// </summary>
public record IntegrationEvent
{
    /// <summary>
    /// ID duy nhất của event
    /// 
    /// Dùng để:
    /// - trace / logging
    /// - idempotency (tránh xử lý trùng event)
    /// </summary>
    [JsonInclude]
    public Guid Id { get; set; }

    /// <summary>
    /// Thời điểm event được tạo (UTC)
    /// 
    /// Dùng cho:
    /// - logging
    /// - debug
    /// - audit
    /// </summary>
    [JsonInclude]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Constructor mặc định
    /// Tự động gán:
    /// - Id mới
    /// - Thời gian tạo hiện tại (UTC)
    /// </summary>
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}