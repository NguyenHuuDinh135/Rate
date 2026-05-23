using WebFrontend.Shared.Pages;
using WebFrontend.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;
using Fluxor;
using WebFrontend.Shared.Services.Api;
using WebFrontend.Shared.Services.Storage;
using WebFrontend.Shared.Services.Device;
using WebFrontend.Services.Device;
using WebFrontend.Shared.Layout;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
