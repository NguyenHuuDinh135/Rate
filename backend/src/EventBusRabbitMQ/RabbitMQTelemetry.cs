using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace EventBusRabbitMQ;

public class RabbitMQTelemetry
{
    // Tên ActivitySource để trace
    public static string ActivitySourceName = "EventBusRabbitMQ";

    // Dùng để tạo span
    public ActivitySource ActivitySource { get; } =
        new(ActivitySourceName);

    // Dùng để propagate trace context qua message
    public TextMapPropagator Propagator { get; } =
        Propagators.DefaultTextMapPropagator;
}
