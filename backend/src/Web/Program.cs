using backend.Infrastructure.Data;
using Hangfire;
using Scalar.AspNetCore;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using backend.Infrastructure.Consumers;
using Microsoft.Extensions.Hosting;

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
}

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.MapOpenApi();
app.MapScalarApiReference();


app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/scalar"));
app.UseHangfireDashboard("/hangfire");
app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.UseFileServer();



app.Run();
