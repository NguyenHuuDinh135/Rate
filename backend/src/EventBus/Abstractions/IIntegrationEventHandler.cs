namespace EventBus.Abstractions;

/// <summary>
/// Interface handler generic cho từng loại IntegrationEvent cụ thể
/// </summary>
/// <typeparam name="TIntegrationEvent">
/// Kiểu event cụ thể mà handler xử lý
/// </typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent>
    : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Xử lý event với kiểu cụ thể
    /// </summary>
    Task Handle(TIntegrationEvent @event);

    /// <summary>
    /// Explicit implementation của interface không generic
    /// 
    /// Cho phép gọi handler thông qua:
    /// IIntegrationEventHandler
    /// mà vẫn dispatch đúng kiểu generic
    /// </summary>
    Task IIntegrationEventHandler.Handle(IntegrationEvent @event)
        => Handle((TIntegrationEvent)@event);
}

/// <summary>
/// Interface handler không generic
/// Dùng để lưu trữ & gọi handler một cách thống nhất
/// (không cần biết kiểu event cụ thể lúc runtime)
/// </summary>
public interface IIntegrationEventHandler
{
    /// <summary>
    /// Xử lý IntegrationEvent (base type)
    /// </summary>
    Task Handle(IntegrationEvent @event);
}