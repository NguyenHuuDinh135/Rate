# IntegrationEventLogEF

## 🎯 Mục đích

`IntegrationEventLogEF` triển khai **Transactional Outbox Pattern** với Entity Framework Core.

Giải quyết vấn đề mất event khi làm việc với kiến trúc microservices và EventBus.

---

# ❗ Vấn đề

Nếu viết:

```csharp
await _dbContext.SaveChangesAsync();
await _eventBus.PublishAsync(orderCreatedEvent);
```

Có thể xảy ra:

- DB commit thành công nhưng publish thất bại → ❌ Mất event
- Publish thành công nhưng DB rollback → ❌ Sai lệch dữ liệu

---

# 💡 Giải pháp: Transactional Outbox Pattern

Thay vì publish trực tiếp:

1. Lưu event vào bảng `IntegrationEventLog`
2. Commit transaction
3. Sau commit → đọc event từ bảng
4. Publish ra RabbitMQ
5. Cập nhật trạng thái event

---

# 📦 Thành phần chính

## 1️⃣ IntegrationEventLogEntry

Entity lưu event trong database.

Chứa:
- EventId
- EventTypeName
- Content (JSON)
- State
- TimesSent
- TransactionId
- CreationTime

Event được serialize thành JSON trước khi lưu.

---

## 2️⃣ IIntegrationEventLogService

Interface định nghĩa:

```csharp
Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
Task MarkEventAsPublishedAsync(Guid eventId);
Task MarkEventAsInProgressAsync(Guid eventId);
Task MarkEventAsFailedAsync(Guid eventId);
```

---

## 3️⃣ IntegrationEventLogService<TContext>

Triển khai chính:

### SaveEventAsync
- Lưu event vào bảng log
- Dùng cùng transaction với business data

### RetrieveEventLogsPendingToPublishAsync
- Lấy các event chưa publish
- Deserialize JSON thành object

### MarkEventAsPublishedAsync
- Đánh dấu publish thành công

### MarkEventAsFailedAsync
- Đánh dấu thất bại (có thể retry)

---

## 4️⃣ ResilientTransaction

Wrapper cho EF ExecutionStrategy để retry khi lỗi transient:

```csharp
await ResilientTransaction.New(_context)
    .ExecuteAsync(async () =>
{
    // Begin transaction
    // Save business data
    // Save integration event
    // Commit
});
```

---

# 🔄 Luồng hoạt động

Khi tạo Order:

1. Begin Transaction
2. Save Order
3. SaveEventAsync(orderCreatedEvent)
4. Commit
5. RetrieveEventLogsPendingToPublishAsync
6. Publish EventBus
7. MarkEventAsPublishedAsync

---

# 📊 Event State

| State | Ý nghĩa |
|--------|----------|
| NotPublished | Chưa publish |
| InProgress | Đang publish |
| Published | Thành công |
| PublishedFailed | Thất bại |

---

# 🏗 Cấu hình DbContext

Trong `OnModelCreating`:

```csharp
builder.UseIntegrationEventLogs();
```

Sẽ tạo bảng:

```
IntegrationEventLog
```

---

# 🚀 Kết luận

IntegrationEventLogEF giúp:

- Không mất event
- Không lệch dữ liệu
- Có retry khi publish lỗi
- Đảm bảo atomic giữa DB và EventBus

Đây là implementation của:

**Transactional Outbox Pattern**