using backend.Shared;

namespace backend.TestAppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddPostgres(Services.DatabaseServer)
            .WithImage("pgvector/pgvector")
            .WithImageTag("pg16")
            .AddDatabase(Services.Database);

        builder.Build().Run();
    }
}