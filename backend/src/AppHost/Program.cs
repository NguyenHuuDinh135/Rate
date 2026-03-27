using backend.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var databaseServer = builder
    .AddPostgres(Services.DatabaseServer)
    .AddDatabase(Services.Database);

var redis = builder.AddRedis(Services.Redis);

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(databaseServer)
    .WithReference(redis)
    .WaitFor(databaseServer)
    .WaitFor(redis)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });


builder.Build().Run();
