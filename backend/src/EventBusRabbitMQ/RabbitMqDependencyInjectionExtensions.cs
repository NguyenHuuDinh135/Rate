using EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqDependencyInjectionExtensions
{
    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(
        this IHostApplicationBuilder builder,
        string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Đăng ký RabbitMQ connection (từ Aspire / infra)
        builder.AddRabbitMQClient(connectionName);

        // RabbitMQ không có OpenTelemetry sẵn
        // => tự thêm ActivitySource để trace publish/consume
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource(RabbitMQTelemetry.ActivitySourceName);
            });

        // Bind EventBusOptions từ appsettings.json
        builder.Services.Configure<EventBusOptions>(
            builder.Configuration.GetSection(SectionName));

        // Đăng ký telemetry helper
        builder.Services.AddSingleton<RabbitMQTelemetry>();

        // Đăng ký EventBus chính
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

        // Vì RabbitMQEventBus implement IHostedService
        // => app start là tự consume message
        builder.Services.AddSingleton<IHostedService>(
            sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);
    }

    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}