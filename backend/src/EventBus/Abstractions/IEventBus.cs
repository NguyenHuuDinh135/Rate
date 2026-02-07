namespace EventBus.Abstractions;

/// <summary>
/// Interface định nghĩa EventBus
/// Chịu trách nhiệm publish (phát) các IntegrationEvent
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish một IntegrationEvent lên message broker
    /// (RabbitMQ, Azure Service Bus, Kafka, …)
    /// </summary>
    /// <param name="event">
    /// Event cần được publish.
    ///  Đây thường là event dùng để giao tiếp giữa các microservice
    /// </param>
    /// <returns>
    /// Task bất đồng bộ để:
    /// - gửi message
    /// - retry nếu lỗi
    /// - không block thread
    /// </returns>
    Task PublishAsync(IntegrationEvent @event);
}