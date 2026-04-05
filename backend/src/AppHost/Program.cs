using backend.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres(Services.DatabaseServer);

var database = postgres.AddDatabase("MovieDb");

var redis = builder.AddRedis(Services.Redis);

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