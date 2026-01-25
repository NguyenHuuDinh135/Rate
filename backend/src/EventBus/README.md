
---

## 1. EventBus dùng để làm gì?

EventBus giúp các service **giao tiếp với nhau bằng sự kiện (event)** thay vì gọi trực tiếp.

```text
Service A → EventBus → Message Broker → EventBus → Service B
```

---

## 2. IntegrationEvent là gì?

Là **dữ liệu sự kiện** gửi giữa các service.

```csharp
public record OrderCreatedEvent : IntegrationEvent
{
    public int OrderId { get; init; }
    public decimal Total { get; init; }
}
```

➡ Chỉ chứa data, **không chứa business logic**.

---

## 3. Serialize là gì?

### 👉 Serialize = đổi object → JSON để gửi đi

```csharp
var evt = new OrderCreatedEvent
{
    OrderId = 123,
    Total = 500000
};

string json = JsonSerializer.Serialize(evt);
```

Kết quả JSON:

```json
{
  "OrderId": 123,
  "Total": 500000
}
```

➡ Message broker **chỉ gửi JSON**, không gửi object C#.

---

## 4. Deserialize là gì?

### 👉 Deserialize = đổi JSON → object khi nhận về

```csharp
var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(json);
```

---

## 5. Vấn đề: Làm sao biết deserialize thành kiểu nào?

Khi nhận message, ta chỉ có:

```text
EventName = "OrderCreatedEvent"
JSON = "{ ... }"
```

➡ Không biết đó là class nào trong code.

---

## 6. Map EventName → EventType là gì?

### 👉 Map = bảng tra cứu tên event → class

```csharp
EventTypes["OrderCreatedEvent"] = typeof(OrderCreatedEvent);
```

Hiểu đơn giản:

| EventName (string) | Type (class)      |
| ------------------ | ----------------- |
| OrderCreatedEvent  | OrderCreatedEvent |

---

## 7. Luồng consume event (có code)

```csharp
// 1. Nhận message từ broker
string eventName = "OrderCreatedEvent";
string json = "{ ... }";

// 2. Tra map để biết kiểu event
var eventType = EventTypes[eventName];

// 3. Deserialize đúng kiểu
var evt = (IntegrationEvent)
    JsonSerializer.Deserialize(json, eventType)!;

// 4. Gọi handler
await handler.Handle(evt);
```

---

## 8. Luồng tổng thể EventBus

```text
PUBLISH
Object → JSON (serialize) → Message Broker

CONSUME
JSON + EventName
    ↓
  MAP (tra Type)
    ↓
JSON → Object (deserialize)
    ↓
Handler.Handle()
```

---

## 9. Đăng ký subscription

```csharp
services
    .AddEventBus()
    .AddSubscription<OrderCreatedEvent, OrderCreatedEventHandler>();
```

➡ Hệ thống sẽ:

* Lưu map EventName → EventType
* Đăng ký handler vào DI

---

## 10. Tóm tắt nhớ nhanh

* **Serialize**: object → JSON (để gửi đi)
* **Deserialize**: JSON → object (để xử lý)
* **Map**: bảng tra tên event → class

```text
Service A → EventBus → Broker → EventBus → Handler
```

> EventBus giúp các service nói chuyện bằng sự kiện, không phụ thuộc trực tiếp vào nhau.
