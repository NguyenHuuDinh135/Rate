using MudBlazor.Services;
using WebUI.Server.Services.Device;
using WebUI.Shared.Services.Admin;
using WebUI.Shared.Services.Device;

namespace WebUI.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddWebUIServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddMudServices();

        services.AddHttpClient("BFFProxy");
        services.AddHttpClient("BFFProxy").ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "http://localhost:15000"));

        services.AddScoped<IDeviceService, WebDeviceService>();
        services.AddScoped<IAdminThemeService, AdminThemeService>();

        return services;
    }
}