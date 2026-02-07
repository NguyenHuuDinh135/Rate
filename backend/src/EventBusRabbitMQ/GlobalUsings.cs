// Làm việc với TCP/UDP socket (networking mức thấp)
global using System.Net.Sockets;

// Xử lý encoding/decoding chuỗi (UTF-8, ASCII, byte[] ↔ string)
global using System.Text;

// Làm việc với JSON: serialize / deserialize object
global using System.Text.Json;

// Các interface trừu tượng cho EventBus (Publish, Subscribe, etc.)
global using EventBus.Abstractions;

// Các lớp sự kiện (Event, IntegrationEvent, …)
global using EventBus.Events;

// Ghi log (ILogger, LogLevel, …)
global using Microsoft.Extensions.Logging;

// Thư viện xử lý retry, circuit breaker, fallback (resilience)
global using Polly;

// Client chính để kết nối RabbitMQ
global using RabbitMQ.Client;

// Xử lý event khi nhận message từ RabbitMQ
global using RabbitMQ.Client.Events;

// Các exception liên quan đến RabbitMQ (kết nối lỗi, channel lỗi, …)
global using RabbitMQ.Client.Exceptions;