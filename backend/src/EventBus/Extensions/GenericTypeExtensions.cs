namespace EventBus.Extensions;

/// <summary>
/// Extension methods hỗ trợ xử lý tên kiểu (Type),
/// đặc biệt hữu ích khi làm việc với generic type
/// </summary>
public static class GenericTypeExtensions
{
    /// <summary>
    /// Trả về tên type thân thiện cho generic type
    /// 
    /// Ví dụ:
    /// typeof(List<int>)        → "List<Int32>"
    /// typeof(Dictionary<int,string>) → "Dictionary<Int32,String>"
    /// typeof(OrderCreatedEvent) → "OrderCreatedEvent"
    /// </summary>
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        // Kiểm tra type có phải generic hay không
        if (type.IsGenericType)
        {
            // Lấy danh sách các generic argument (T, TKey, TValue, ...)
            var genericTypes = string.Join(
                ",",
                type.GetGenericArguments()
                    .Select(t => t.Name)
                    .ToArray());

            // Loại bỏ ký tự `1, `2... khỏi tên type
            // Ví dụ: List`1 → List
            typeName =
                $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            // Type không generic → lấy tên trực tiếp
            typeName = type.Name;
        }

        return typeName;
    }

    /// <summary>
    /// Overload cho object
    /// Giúp gọi tiện hơn:
    /// obj.GetGenericTypeName()
    /// </summary>
    public static string GetGenericTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }
}