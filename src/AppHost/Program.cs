using backend.Shared;

var builder = DistributedApplication.CreateBuilder(args);

// Keep credentials stable across runs
var postgresPassword = builder.AddParameter("postgresadmin", "postgres", secret: true);

var postgres = builder.AddPostgres(
        name: Services.DatabaseServer,
        password: postgresPassword,
        port: 5432)
    .WithImage("pgvector/pgvector", "pg16")
    .WithDataVolume("rate-postgres-data");

var database = postgres.AddDatabase(Services.Database);

var redis = builder.AddRedis(Services.Redis, port: 6379)
    .WithDataVolume("rate-redis-data");

// Fix RabbitMQ credentials for consistency
var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch");

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(database)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(elasticsearch)
    .WaitFor(database)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch);

// Blazor Web App
// builder.AddProject<Projects.WebFrontend>("frontend")
//     .WithReference(web)
//     .WaitFor(web);

// Mobile App (MAUI Blazor Hybrid Client)
builder.AddProject("mobile-app", "../MobileApp/MobileApp.csproj")
    .WithReference(web)
    .WaitFor(web);

builder.Build().Run();
