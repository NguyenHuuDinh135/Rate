using backend.Application.Common.Interfaces;
using backend.Domain.Constants;
using backend.Infrastructure.Caching;
using backend.Infrastructure.Data;
using backend.Infrastructure.Data.Interceptors;
using backend.Infrastructure.Identity;
using backend.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Medallion.Threading; 
using Medallion.Threading.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");

        builder.Services
            .AddOptions<RedisOptions>()
            .Bind(builder.Configuration.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            var redisConnectionString =
                builder.Configuration.GetConnectionString(Services.Redis)
                ?? builder.Configuration[$"{RedisOptions.SectionName}:ConnectionString"];

            Guard.Against.NullOrWhiteSpace(redisConnectionString, message: "Redis connection string is not configured.");
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });
        builder.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = sp.GetRequiredService<IConnectionMultiplexer>();
            // Medallion sẽ quản lý việc tạo Lock trên Database của Redis
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
        builder.Services.AddSingleton<ILockService, RedisLockService>();
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
