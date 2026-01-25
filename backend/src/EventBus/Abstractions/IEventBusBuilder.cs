namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder dùng để cấu hình EventBus trong Dependency Injection
/// </summary>
public interface IEventBusBuilder
{
    /// <summary>
    /// IServiceCollection chứa các service đã/đang được đăng ký
    /// 
    /// Cho phép:
    /// - Add EventBus implementation (RabbitMQ, InMemory, …)
    /// - Add handlers, subscriptions
    /// - Chain các extension method
    /// </summary>
    IServiceCollection Services { get; }
}