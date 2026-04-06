using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Application.Common.Interfaces;
using backend.Domain.Constants;
using backend.Infrastructure.Caching;
using backend.Infrastructure.Data;
using backend.Infrastructure.Data.Interceptors;
using backend.Infrastructure.Email;
using backend.Infrastructure.Identity;
using backend.Infrastructure.Jwt;
using backend.Shared;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddDatabase(configuration)
            .AddRedis(configuration)
            .AddEmail(configuration)
            .AddHangfire(configuration)
            .AddMessaging(configuration) // optional
            .AddIdentity()
            .AddJwt(configuration)
            .AddApplicationServices();

        return builder.Services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, $"Connection string '{Services.Database}' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        Console.WriteLine(
            $"DB: {connectionString}"
        );

        return services;
    }

    private static IServiceCollection AddEmail(
        this IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<SmtpOptions>()
            .Bind(config.GetSection(SmtpOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }

    private static IServiceCollection AddRedis(
        this IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<RedisOptions>()
            .Bind(config.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnectionString = config.GetConnectionString(Services.Redis)
                ?? config[$"{RedisOptions.SectionName}:ConnectionString"];

            Guard.Against.NullOrWhiteSpace(redisConnectionString, "Redis not configured.");

            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = sp.GetRequiredService<IConnectionMultiplexer>();
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<ILockService, RedisLockService>();
        services.AddSingleton<IRateLimitService, RedisRateLimitService>();
        services.AddSingleton<IIdempotencyService, RedisIdempotencyService>();
        services.AddSingleton<IRevokeTokenService, RedisTokenRevocationService>();
        services.AddSingleton<IOneTimeTokenService, RedisOneTimeTokenService>();

        return services;
    }

    private static IServiceCollection AddHangfire(
        this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString(Services.Database);

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(connectionString!),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    InvisibilityTimeout = TimeSpan.FromMinutes(5),
                    DistributedLockTimeout = TimeSpan.FromMinutes(5)
                }
            ));

        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services, IConfiguration config)
    {
        var rabbitConnectionString = config.GetConnectionString("rabbitmq");

        if (string.IsNullOrWhiteSpace(rabbitConnectionString))
            return services;

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConnectionString);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection AddIdentity(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();
        services
            .AddIdentityCore<ApplicationUser>(
                options =>{
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 6;
                }
            )
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge,
                policy => policy.RequireRole(Roles.Administrator)));

        return services;
    }

    private static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IRefreshTokenStore, RedisRefreshTokenStore>();

        return services;
    }

    private static IServiceCollection AddJwt(
        this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        
        if (jwtSettings == null)
            throw new InvalidOperationException("JWT settings not found in configuration.");

        services.Configure<JwtSettings>(jwtSettings);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var settings = jwtSettings.Get<JwtSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings!.Issuer,
                    
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                    
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (string.IsNullOrWhiteSpace(jti))
                        {
                            context.Fail("Token is missing jti.");
                            return;
                        }

                        var tokenRevocationService = context.HttpContext.RequestServices.GetRequiredService<IRevokeTokenService>();
                        if (await tokenRevocationService.IsRevokedAsync(jti, context.HttpContext.RequestAborted))
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                };
            });

        services.AddScoped<IJwtService, JwtService>();
            
        return services;
    }
}