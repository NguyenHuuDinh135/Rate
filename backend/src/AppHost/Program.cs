using backend.Shared;

var databaseName = "MovieDb";

var builder = DistributedApplication.CreateBuilder(args);

// Fix password
var postgresPassword = builder.AddParameter(
    "postgres-password",
    "123",
    secret: true
);

// Cấu hình Postgres giống đoạn trên
var postgres = builder
    .AddPostgres(
        Services.DatabaseServer,
        password: postgresPassword,   // fix password
        port: 5432                    // fix port
    )
    .WithEnvironment("POSTGRES_DB", databaseName);

// Tạo database
var database = postgres.AddDatabase(databaseName);

// Web
var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(database)        // ⚠️ nên reference database, không phải server
    .WaitFor(database)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.Build().Run();