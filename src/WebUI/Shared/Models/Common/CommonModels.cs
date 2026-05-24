using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebUI.Shared.Models.Common;

[JsonConverter(typeof(ApiResponseConverterFactory))]
public record ApiResponse<T>(ApiHeaders Headers, T Body);

public record ApiHeaders(int Success, string Message);

public enum MovieType { ComingSoon, NowShowing, Removed }
public enum PaymentMethod { Cash, Card, Cod, Momo, VnPay }
public enum ShowType { ThreeD, TwoD }
public enum BookingStatus { Reserved, Paid, Cancelled }

public record PaymentDialogData(decimal TotalAmount, int SeatCount);

public class ApiResponseConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && 
               typeToConvert.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(ApiResponseConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class ApiResponseConverter<T> : JsonConverter<ApiResponse<T>>
{
    public override ApiResponse<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Object && 
                (root.TryGetProperty("headers", out _) || root.TryGetProperty("Headers", out _)) && 
                (root.TryGetProperty("body", out _) || root.TryGetProperty("Body", out _)))
            {
                var headersProp = root.TryGetProperty("headers", out var h) ? h : root.GetProperty("Headers");
                var bodyProp = root.TryGetProperty("body", out var b) ? b : root.GetProperty("Body");
                
                var headers = JsonSerializer.Deserialize<ApiHeaders>(headersProp.GetRawText(), options);
                var body = JsonSerializer.Deserialize<T>(bodyProp.GetRawText(), options);
                
                return new ApiResponse<T>(headers ?? new ApiHeaders(1, "Success"), body!);
            }
            else
            {
                var body = JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                return new ApiResponse<T>(new ApiHeaders(1, "Success"), body!);
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, ApiResponse<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("headers");
        JsonSerializer.Serialize(writer, value.Headers, options);
        writer.WritePropertyName("body");
        JsonSerializer.Serialize(writer, value.Body, options);
        writer.WriteEndObject();
    }
}

