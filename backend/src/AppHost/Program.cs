using backend.Shared;

var builder = DistributedApplication.CreateBuilder(args);

// Keep credentials stable across runs so the generated connection string
// does not break when containers are reused.
var postgresPassword = builder.AddParameter("postgresadmin", "postgres", secret: true);

// Named Docker volumes persist data between AppHost runs (see Aspire docs: persist-data-volumes).
// Pin Postgres 16: PG 18+ changed data paths; WithDataVolume() targets the classic layout.
var postgres = builder.AddPostgres(
        name: Services.DatabaseServer,
        password: postgresPassword,
        port: 54322)
    .WithImage("pgvector/pgvector")
    .WithImageTag("pg16")
    .WithDataVolume("rate-postgres-data");

var database = postgres.AddDatabase(Services.Database);

// Redis: volume + persistence so cache/refresh-token keys survive container recreation.
var redis = builder.AddRedis(Services.Redis, port: 6379)
    .WithDataVolume("rate-redis-data");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin(); // Để vào được giao diện quản lý RabbitMQ

// Khai báo Elasticsearch cho Search
var elasticsearch = builder.AddElasticsearch("elasticsearch");

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(database)   // inject connection string
    .WithReference(redis)      // inject redis
    .WithReference(rabbitmq)      // Kết nối API với RabbitMQ
    .WithReference(elasticsearch) // Kết nối API với Elasticsearch
    .WaitFor(database)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WithEnvironment("GEMINI_API_KEY", builder.Configuration["GEMINI_API_KEY"] ?? string.Empty)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.Build().Run();