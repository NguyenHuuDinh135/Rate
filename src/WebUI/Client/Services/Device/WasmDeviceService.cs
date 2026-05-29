using WebUI.Shared.Services.Device;
using Microsoft.JSInterop;

namespace WebUI.Client.Services.Device;

public class WasmDeviceService : IDeviceService
{
    private readonly IJSRuntime _jsRuntime;

    public WasmDeviceService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetLocationAsync()
    {
        try
        {
            await Task.Delay(100);
            return "WASM Location Data (Mock)";
        }
        catch
        {
            return "Unknown";
        }
    }
}
