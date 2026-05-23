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

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ITokenStorage, WebTokenStorage>();
builder.Services.AddScoped<IDeviceService, WasmDeviceService>();

builder.Services.AddFluxor(options => 
    options.ScanAssemblies(typeof(MainLayout).Assembly));

// Register API Clients
var apiBaseUrl = builder.HostEnvironment.BaseAddress; 
builder.Services.AddRefitClient<IAuthApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IMovieApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IGenreApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IShowApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<ITheaterApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IBookingApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPaymentApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPersonApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPermissionApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

await builder.Build().RunAsync();
