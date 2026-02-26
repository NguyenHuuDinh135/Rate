namespace IntegrationEventLogEF;

public static class IntegrationLogExtensions
{
    /// <summary>
    /// Thêm cấu hình bảng IntegrationEventLog vào DbContext
    /// </summary>
    public static void UseIntegrationEventLogs(this ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLogEntry>(builder =>
        {
            builder.ToTable("IntegrationEventLog");
            builder.HasKey(e => e.EventId);
        });
    }
}