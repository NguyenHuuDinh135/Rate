using Azure.Identity;
using backend.Application.Common.Interfaces;
using backend.Infrastructure.Data;
using backend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Elastic.Clients.Elasticsearch;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<ApiExceptionOperationTransformer>();
            options.AddOperationTransformer<IdentityApiOperationTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("WebUI", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:16000",
                        "https://localhost:16000",
                        "http://127.0.0.1:16000",
                        "https://127.0.0.1:16000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
