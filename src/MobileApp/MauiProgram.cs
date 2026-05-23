using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Fluxor;
using Refit;
using WebUI.Shared.Layout;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Storage;
using MobileApp.Services;

namespace MobileApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddServiceDiscovery();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddFluentUIComponents();
        
        builder.Services.AddScoped<ITokenStorage, MobileTokenStorage>();
        builder.Services.AddSingleton<IDeviceService, MobileDeviceService>();

        builder.Services.AddFluxor(options => 
            options.ScanAssemblies(typeof(MainLayout).Assembly));

        // Configure HttpClient for APIs using Aspire Service Discovery
        // Note: For Android Emulator to access host's local services without Aspire proxy, 
        // 10.0.2.2 is usually used. However, with Aspire .NET 9 Service Discovery in MAUI,
        // it can resolve "http://webapi" if configured properly via AppHost.
        var apiBaseUrl = "http://webapi";

        builder.Services.AddRefitClient<IAuthApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IMovieApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IGenreApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IShowApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<ITheaterApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IBookingApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IPaymentApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IPersonApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
        builder.Services.AddRefitClient<IPermissionApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        return builder.Build();
    }
}
