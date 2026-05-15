using backend.Infrastructure.Data;
using Hangfire;
using Scalar.AspNetCore;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using backend.Infrastructure.Consumers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using backend.Application.Common.Models;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

// ===============================
// PostgreSQL DbContext từ Aspire
// ===============================
// builder.AddNpgsqlDbContext<ApplicationDbContext>("MovieDb");


// ===============================
// Elasticsearch Client
// Aspire tự inject connection string "elasticsearch"
// ===============================
builder.Services.AddSingleton(sp =>
{
    var uri = builder.Configuration.GetConnectionString("elasticsearch");

    var settings = new ElasticsearchClientSettings(new Uri(uri!))
        .DefaultIndex("movies");

    return new ElasticsearchClient(settings);
});


// ==============================
// MassTransit + RabbitMQ + Outbox
// ===============================
builder.Services.AddMassTransit(x =>
{
    // đăng ký consumer
    x.AddConsumer<MovieCreatedConsumer>();

    // bật Outbox lưu message trong DB (Postgres)
    x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    // format tên queue cho đẹp
    x.SetKebabCaseEndpointNameFormatter();

    // cấu hình RabbitMQ từ Aspire
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));

        cfg.ConfigureEndpoints(context);

        // Khuyến nghị dùng khi development (nhanh và ổn định)
        cfg.UseInMemoryOutbox(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseCors(static builder => 
    builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();


app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var result = backend.Application.Common.Models.Result.Failure(new[] { exception?.Message ?? "An unexpected error occurred." });
        await context.Response.WriteAsJsonAsync(result);
    });
});

app.Map("/", () => Results.Redirect("/scalar"));
app.UseHangfireDashboard("/hangfire");
app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.UseFileServer();



app.Run();
