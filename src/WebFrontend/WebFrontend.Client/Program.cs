using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Fluxor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFluentUIComponents();
builder.Services.AddFluxor(options => 
    options.ScanAssemblies(typeof(WebFrontend.Client._Imports).Assembly));

await builder.Build().RunAsync();
