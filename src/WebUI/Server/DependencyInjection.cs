using Blazored.LocalStorage;
using Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;
using MudBlazor.Services;
using Refit;
using WebUI.Server.Services.Device;
using WebUI.Shared.Layout;
using WebUI.Shared.Services.Admin;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Device;
using WebUI.Shared.Services.Storage;

namespace WebUI.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddWebUIServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddFluentUIComponents();
        services.AddMudServices();
        services.AddBlazoredLocalStorage();

        services.AddScoped<ITokenStorage, WebTokenStorage>();
        services.AddScoped<IDeviceService, WebDeviceService>();
        services.AddScoped<IAdminThemeService, AdminThemeService>();

        services.AddFluxor(options =>
            options.ScanAssemblies(typeof(MainLayout).Assembly));

        services.AddBffRefitClients(configuration);

        services.AddHttpClient("BFFProxy");

        return services;
    }

    private static IServiceCollection AddBffRefitClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var bffBaseUrl = configuration["BffBaseUrl"] ?? "http://localhost:5000";

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IMovieApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IGenreApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IShowApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<ITheaterApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IBookingApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IPaymentApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IPersonApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IPermissionApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        services.AddRefitClient<IUserApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(bffBaseUrl));

        return services;
    }
}