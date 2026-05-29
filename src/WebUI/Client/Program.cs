using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Fluxor;
using WebUI.Shared.Models.Common;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Storage;
using WebUI.Shared.Services.Device;
using WebUI.Client.Services.Device;
using WebUI.Shared.Layout;
using Blazored.LocalStorage;
using MudBlazor.Services;
using WebUI.Shared.Services.Admin;
using WebUI.Shared.Services.Auth;
using WebUI.Shared.Store.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFluentUIComponents();
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ITokenStorage, WebTokenStorage>();
builder.Services.AddScoped<IDeviceService, WasmDeviceService>();
builder.Services.AddScoped<IAdminThemeService, AdminThemeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddFluxor(options => 
    options.ScanAssemblies(typeof(AuthState).Assembly));

// Register API Clients

var apiBaseUrl = builder.HostEnvironment.BaseAddress; 

var refitSettings = new RefitSettings
{
    ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        Converters = { new ApiResponseConverterFactory() }
    })
};

builder.Services.AddRefitClient<IAuthApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IMovieApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IGenreApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IShowApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<ITheaterApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IBookingApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPaymentApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPersonApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPermissionApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
// AddRefitClient<IUserApi>()
builder.Services.AddRefitClient<IUserApi>(refitSettings).ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();

await builder.Build().RunAsync();
