using WebUI.Shared.Pages;
using WebUI.Server.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Fluxor;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Storage;
using WebUI.Shared.Services.Device;
using WebUI.Server.Services.Device;
using WebUI.Shared.Layout;
using Blazored.LocalStorage;
using Tailwind;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.UseTailwindCli();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ITokenStorage, WebTokenStorage>();
builder.Services.AddScoped<IDeviceService, WebDeviceService>();

builder.Services.AddFluxor(options => 
    options.ScanAssemblies(typeof(MainLayout).Assembly));

// Register API Clients
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:15000";
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT")) || 
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")))
{
    apiBaseUrl = "http://webapi";
}
builder.Services.AddRefitClient<IAuthApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IMovieApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IGenreApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IShowApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<ITheaterApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IBookingApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPaymentApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPersonApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddRefitClient<IPermissionApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MainLayout).Assembly);

app.Run();
