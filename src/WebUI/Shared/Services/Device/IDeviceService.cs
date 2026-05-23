namespace WebUI.Shared.Services.Device;

/// <summary>
/// Trừu tượng hóa các API liên quan đến phần cứng thiết bị.
/// Giúp code Blazor chạy tốt trên cả Web (dùng JSInterop) và Mobile (dùng MAUI Essentials).
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Lấy tọa độ GPS hiện tại.
    /// </summary>
    Task<string> GetLocationAsync();
    
    /// <summary>
    /// Kiểm tra xem ứng dụng đang chạy trên Mobile hay Web.
    /// </summary>
    bool IsMobileDevice();
}
