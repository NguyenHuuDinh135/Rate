namespace IntegrationEventLogEF.Services;

/// <summary>
/// Service chịu trách nhiệm:
/// - Lưu IntegrationEvent vào DB (Event Log Table)
/// - Lấy các event chưa publish
/// - Cập nhật trạng thái event (Published / Failed / InProgress)
///
/// Đây là phần quan trọng của pattern:
/// 👉 Transactional Outbox Pattern
/// </summary>
public class IntegrationEventLogService<TContext> 
    : IIntegrationEventLogService, IDisposable
    where TContext : DbContext
{
    // Đảm bảo Dispose chỉ chạy 1 lần
    private volatile bool _disposedValue;

    // DbContext được inject từ service bên ngoài
    private readonly TContext _context;

    // Danh sách tất cả các loại IntegrationEvent trong assembly
    // Dùng để deserialize JSON thành đúng type
    private readonly Type[] _eventTypes;

    /// <summary>
    /// Constructor
    /// Load tất cả các class kết thúc bằng "IntegrationEvent"
    /// để phục vụ deserialize khi đọc từ DB
    /// </summary>
    public IntegrationEventLogService(TContext context)
    {
        _context = context;

        // Load tất cả các type trong Assembly hiện tại
        _eventTypes = Assembly
            .Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes()
            .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            .ToArray();
    }

    /// <summary>
    /// Lấy các event chưa được publish theo transactionId
    /// Thường được gọi sau khi transaction business commit xong
    /// </summary>
    public async Task<IEnumerable<IntegrationEventLogEntry>> 
        RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        // Query các event có cùng TransactionId và chưa publish
        var result = await _context.Set<IntegrationEventLogEntry>()
            .Where(e => e.TransactionId == transactionId &&
                        e.State == EventStateEnum.NotPublished)
            .ToListAsync();

        if (result.Count != 0)
        {
            return result
                .OrderBy(o => o.CreationTime) // Publish theo thứ tự tạo
                .Select(e =>
                    e.DeserializeJsonContent(
                        _eventTypes.FirstOrDefault(t =>
                            t.Name == e.EventTypeShortName)));
        }

        return [];
    }

    /// <summary>
    /// Lưu event vào bảng IntegrationEventLog
    /// Phải dùng cùng transaction với business data
    /// để đảm bảo atomic (All or Nothing)
    /// </summary>
    public Task SaveEventAsync(
        IntegrationEvent @event,
        IDbContextTransaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        // Tạo log entry từ event
        var eventLogEntry =
            new IntegrationEventLogEntry(@event, transaction.TransactionId);

        // Bắt buộc sử dụng cùng transaction với business
        _context.Database.UseTransaction(transaction.GetDbTransaction());

        // Thêm vào bảng
        _context.Set<IntegrationEventLogEntry>().Add(eventLogEntry);

        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Đánh dấu event đã publish thành công
    /// </summary>
    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.Published);
    }

    /// <summary>
    /// Đánh dấu event đang được publish
    /// (Tránh bị publish trùng khi retry)
    /// </summary>
    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.InProgress);
    }

    /// <summary>
    /// Đánh dấu event publish thất bại
    /// </summary>
    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
    }

    /// <summary>
    /// Hàm dùng chung để cập nhật trạng thái event
    /// </summary>
    private Task UpdateEventStatus(
        Guid eventId,
        EventStateEnum status)
    {
        var eventLogEntry =
            _context.Set<IntegrationEventLogEntry>()
                .Single(ie => ie.EventId == eventId);

        eventLogEntry.State = status;

        // Nếu đang publish thì tăng số lần gửi
        if (status == EventStateEnum.InProgress)
            eventLogEntry.TimesSent++;

        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Dispose DbContext khi không còn dùng
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}