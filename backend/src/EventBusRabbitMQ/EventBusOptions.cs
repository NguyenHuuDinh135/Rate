namespace EventBusRabbitMQ;

/// <summary>
/// Options cấu hình cho EventBusRabbitMQ
/// Được bind từ appsettings.json
/// </summary>
public class EventBusOptions
{
    // Tên queue (mỗi service nên có 1 tên riêng)
    public string SubscriptionClientName { get; set; }

    // Số lần retry khi publish thất bại
    public int RetryCount { get; set; } = 10;
}