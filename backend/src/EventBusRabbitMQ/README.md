# EventBusRabbitMQ

EventBusRabbitMQ là implementation của `IEventBus` dùng RabbitMQ để giao tiếp giữa các service theo mô hình event-driven.

## Flow

Publish:
IntegrationEvent → Serialize(JSON) → RabbitMQ Exchange → RoutingKey = EventName

Consume:
RabbitMQ Queue → Deserialize(JSON) → IntegrationEvent → Handler

## Cấu hình

{
"EventBus": {
"SubscriptionClientName": "order-service",
"RetryCount": 5
}
}

## Đăng ký

builder.AddRabbitMqEventBus("rabbitmq");

## IntegrationEvent

public record IntegrationEvent
{
public Guid Id { get; set; }
public DateTime CreationDate { get; set; }
}

Mọi event đều phải kế thừa IntegrationEvent.

## Publish

await eventBus.PublishAsync(
new OrderCreatedIntegrationEvent(123));

## Subscribe

builder.Services
.AddRabbitMqEventBus("rabbitmq")
.AddSubscription<OrderCreatedIntegrationEvent, OrderCreatedHandler>();

public class OrderCreatedHandler
: IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
public Task Handle(OrderCreatedIntegrationEvent @event)
{
return Task.CompletedTask;
}
}

## Serialize & Map

Serialize: object → JSON → byte[]  
Deserialize: JSON → object

EventName (RoutingKey) → EventType để deserialize đúng kiểu.

## Retry & Trace

- Retry publish bằng Polly (exponential backoff)
- OpenTelemetry trace cho publish / consume
