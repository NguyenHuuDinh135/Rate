using Blazored.LocalStorage;
using Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;
using MudBlazor.Services;
using Refit;
using WebUI.Server.Services.Device;
using WebUI.Shared.Layout;
using WebUI.Shared.Models.Common;
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
        var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:15000";

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT")) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")))
        {
            apiBaseUrl = "http://webapi";
        }

        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                Converters = { new ApiResponseConverterFactory() }
            })
        };

        services.AddRefitClient<IAuthApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IMovieApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IGenreApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IShowApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<ITheaterApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IBookingApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IPaymentApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IPersonApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IPermissionApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        services.AddRefitClient<IUserApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        return services;
    }
}