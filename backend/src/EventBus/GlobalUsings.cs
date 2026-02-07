// Cung cấp các attribute và kiểu hỗ trợ cho JSON serialization
// Ví dụ: [JsonInclude], [JsonIgnore], [JsonPropertyName], ...
global using System.Text.Json.Serialization;

// Import các IntegrationEvent và các event kế thừa
// Dùng xuyên suốt EventBus (publish, subscribe, handler)
global using EventBus.Events;