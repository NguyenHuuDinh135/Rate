using WebUI.Shared.Services.Device;
using Microsoft.JSInterop;

namespace WebUI.Server.Services.Device;

public class WebDeviceService : IDeviceService
{
    private readonly IJSRuntime _jsRuntime;

    public WebDeviceService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetLocationAsync()
    {
        try
        {
            // Trong thực tế sẽ gọi hàm JSInterop để lấy tọa độ từ trình duyệt
            // return await _jsRuntime.InvokeAsync<string>("navigator.geolocation.getCurrentPosition...");
            await Task.Delay(100);
            return "Web Location Data (Mock)";
        }
        catch
        {
            return "Unknown";
        }
    }

    public bool IsMobileDevice()
    {
        return false; // Web application
    }
}
