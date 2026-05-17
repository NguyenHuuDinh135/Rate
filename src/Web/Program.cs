using backend.Infrastructure.Data;
using Hangfire;
using Scalar.AspNetCore;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using backend.Infrastructure.Consumers;
using Microsoft.Extensions.Hosting;
using backend.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

// Elasticsearch Client
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("elasticsearch");
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = "http://localhost:9200";
    }

    var settings = new ElasticsearchClientSettings(new Uri(connectionString))
        .DefaultIndex("movies");

    return new ElasticsearchClient(settings);
});

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(ApplicationDbContext).Assembly);

    x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("messaging");
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Use the connection string provided by Aspire directly
            cfg.Host(connectionString);
        }
        else
        {
            cfg.Host("localhost", "/");
        }

        cfg.ConfigureEndpoints(context);
        cfg.UseInMemoryOutbox(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    try 
    {
        await app.InitialiseDatabaseAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
    
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseStaticFiles();

app.MapEndpoints(typeof(Program).Assembly);

app.Run();
