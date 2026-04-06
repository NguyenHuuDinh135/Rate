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
        port: 5432)
    .WithImage("postgres:16")
    .WithDataVolume("rate-postgres-data");

var database = postgres.AddDatabase(Services.Database);

// Redis: volume + persistence so cache/refresh-token keys survive container recreation.
var redis = builder.AddRedis(Services.Redis, port: 6379)
    .WithDataVolume("rate-redis-data");

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(database)   // inject connection string
    .WithReference(redis)      // inject redis
    .WaitFor(database)
    .WaitFor(redis)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.Build().Run();