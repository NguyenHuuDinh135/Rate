using backend.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres(Services.DatabaseServer);

var database = postgres.AddDatabase("MovieDb");

var redis = builder.AddRedis(Services.Redis);

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
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.Build().Run();