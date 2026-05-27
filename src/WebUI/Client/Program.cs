using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Fluxor;
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
var apiBaseUrl = "http://localhost:15000"; 
builder.Services.AddRefitClient<IAuthApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IMovieApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IGenreApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IShowApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<ITheaterApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IBookingApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPaymentApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPersonApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IPermissionApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddRefitClient<IUserApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<BearerTokenHandler>();

await builder.Build().RunAsync();
