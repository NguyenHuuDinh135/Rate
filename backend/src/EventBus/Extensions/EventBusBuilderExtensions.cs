using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using EventBus.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Các extension method cho IEventBusBuilder
/// Dùng để cấu hình EventBus theo fluent API
/// </summary>
public static class EventBusBuilderExtensions
{
    /// <summary>
    /// Cho phép cấu hình JsonSerializerOptions
    /// dùng cho serialize / deserialize IntegrationEvent
    /// </summary>
    /// <param name="eventBusBuilder">EventBus builder</param>
    /// <param name="configure">Action cấu hình JsonSerializerOptions</param>
    /// <returns>IEventBusBuilder để tiếp tục chaining</returns>
    public static IEventBusBuilder ConfigureJsonOptions(
        this IEventBusBuilder eventBusBuilder,
        Action<JsonSerializerOptions> configure)
    {
        // Cấu hình EventBusSubscriptionInfo thông qua Options pattern
        eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
        {
            configure(o.JsonSerializerOptions);
        });

        return eventBusBuilder;
    }

    /// <summary>
    /// Đăng ký subscription:
    /// - Event type (T)
    /// - Event handler (TH)
    /// </summary>
    /// <typeparam name="T">Kiểu IntegrationEvent</typeparam>
    /// <typeparam name="TH">
    /// Handler xử lý event
    /// 
    /// DynamicallyAccessedMembers:
    /// - Giữ lại public constructors khi AOT / trimming
    /// - Tránh lỗi không tạo được handler qua DI
    /// </typeparam>
    public static IEventBusBuilder AddSubscription<
        T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TH>
        (this IEventBusBuilder eventBusBuilder)
        where T : IntegrationEvent
        where TH : class, IIntegrationEventHandler<T>
    {
        // Đăng ký handler bằng Keyed Services
        // Key = typeof(T)
        // Cho phép nhiều handler xử lý cùng một event type
        eventBusBuilder.Services.AddKeyedTransient<IIntegrationEventHandler, TH>(typeof(T));

        // Lưu thông tin subscription vào EventBusSubscriptionInfo
        eventBusBuilder.Services.Configure<EventBusSubscriptionInfo>(o =>
        {
            // Mapping:
            // "OrderCreatedEvent" -> typeof(OrderCreatedEvent)
            //
            // Dùng để:
            // - deserialize đúng kiểu event
            // - subscribe tới broker (RabbitMQ, ...)
            // - tránh Type.GetType (không an toàn khi trimming)
            o.EventTypes[typeof(T).Name] = typeof(T);
        });

        return eventBusBuilder;
    }
}
