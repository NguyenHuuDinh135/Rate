using WebUI.Shared.Services.Device;
using Microsoft.Maui.Devices.Sensors;

namespace MobileApp.Services;

public class MobileDeviceService : IDeviceService
{
    public async Task<string> GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                return $"{location.Latitude},{location.Longitude}";
            }
        }
        catch (Exception ex)
        {
            // Xử lý quyền hoặc lỗi phần cứng
            Console.WriteLine($"Unable to get location: {ex.Message}");
        }

        return "Unknown";
    }

    public bool IsMobileDevice()
    {
        return true;
    }
}
