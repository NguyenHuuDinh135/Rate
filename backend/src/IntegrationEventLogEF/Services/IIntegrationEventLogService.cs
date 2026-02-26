namespace IntegrationEventLogEF.Services;

/// <summary>
/// Service quản lý bảng IntegrationEventLog.
/// Dùng để lưu event vào DB trước khi publish ra message broker (Outbox Pattern).
/// </summary>
public interface IIntegrationEventLogService
{
    /// <summary>
    /// Lấy danh sách event chưa publish theo transactionId.
    /// Dùng sau khi transaction business commit thành công.
    /// </summary>
    Task<IEnumerable<IntegrationEventLogEntry>> 
        RetrieveEventLogsPendingToPublishAsync(Guid transactionId);

    /// <summary>
    /// Lưu event vào bảng IntegrationEventLog
    /// Event được lưu chung transaction với business data.
    /// </summary>
    Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);

    /// <summary>
    /// Đánh dấu event đã publish thành công.
    /// </summary>
    Task MarkEventAsPublishedAsync(Guid eventId);

    /// <summary>
    /// Đánh dấu event đang publish (tăng TimesSent).
    /// </summary>
    Task MarkEventAsInProgressAsync(Guid eventId);

    /// <summary>
    /// Đánh dấu event publish thất bại.
    /// </summary>
    Task MarkEventAsFailedAsync(Guid eventId);
}