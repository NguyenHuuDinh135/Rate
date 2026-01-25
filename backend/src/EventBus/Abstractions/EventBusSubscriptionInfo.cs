using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace EventBus.Abstractions;

/// <summary>
/// Lưu trữ thông tin subscription của EventBus:
/// - Mapping giữa tên event và kiểu dữ liệu (Type)
/// - Cấu hình JsonSerializerOptions dùng chung cho EventBus
/// </summary>
public class EventBusSubscriptionInfo
{
    /// <summary>
    /// Dictionary lưu mapping:
    /// Key   : tên event (string)
    /// Value : kiểu dữ liệu của event (Type)
    /// 
    /// Ví dụ:
    /// "OrderCreatedEvent" -> typeof(OrderCreatedEvent)
    /// </summary>
    public Dictionary<string, Type> EventTypes { get; } = [];

    /// <summary>
    /// JsonSerializerOptions dùng để serialize / deserialize event
    /// Khởi tạo từ DefaultSerializerOptions để đảm bảo cấu hình thống nhất
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; }
        = new(DefaultSerializerOptions);

    /// <summary>
    /// Cấu hình mặc định cho JsonSerializerOptions
    /// 
    /// TypeInfoResolver quyết định cách System.Text.Json
    /// lấy metadata của object khi serialize / deserialize
    /// </summary>
    internal static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        // Nếu runtime cho phép dùng reflection
        // → dùng DefaultJsonTypeInfoResolver (chuẩn, linh hoạt)
        // Nếu không (AOT, trimming)
        // → dùng JsonTypeInfoResolver.Combine() (an toàn hơn)
        TypeInfoResolver =
            JsonSerializer.IsReflectionEnabledByDefault
                ? CreateDefaultTypeResolver()
                : JsonTypeInfoResolver.Combine()
    };

    // Bỏ cảnh báo khi build với AOT / trimming
    // Vì DefaultJsonTypeInfoResolver dùng reflection & dynamic code
#pragma warning disable IL2026
#pragma warning disable IL3050
    /// <summary>
    /// Tạo TypeInfoResolver mặc định dựa trên reflection
    /// Dùng cho môi trường không bị hạn chế (non-AOT)
    /// </summary>
    private static IJsonTypeInfoResolver CreateDefaultTypeResolver()
        => new DefaultJsonTypeInfoResolver();
#pragma warning restore IL3050
#pragma warning restore IL2026
}
