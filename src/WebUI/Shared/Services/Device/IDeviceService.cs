namespace WebUI.Shared.Services.Device;

/// <summary>
/// Trừu tượng hóa các API trình duyệt liên quan đến thiết bị.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Lấy tọa độ GPS hiện tại.
    /// </summary>
    Task<string> GetLocationAsync();
}
