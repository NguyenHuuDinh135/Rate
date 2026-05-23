namespace backend.Application.Common.Models;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }

    public ApiResponse(T data, string? message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }

    public static ApiResponse<T> Succeeded(T data, string? message = null) 
        => new(data, message, true);

    public static ApiResponse<T> Failed(T data, string? message = null) 
        => new(data, message, false);
}
