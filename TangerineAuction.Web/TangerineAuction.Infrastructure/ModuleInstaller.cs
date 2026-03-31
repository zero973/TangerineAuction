using EFCore.NamingConventions.Internal;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;
using Refit;
using TangerineAuction.Core.Modules;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Infrastructure.Data;
using TangerineAuction.Infrastructure.Data.Repositories;
using TangerineAuction.Infrastructure.Data.Services;
using TangerineAuction.Infrastructure.Data.Services.Impl;
using TangerineAuction.Infrastructure.Email;
using TangerineAuction.Infrastructure.Integration;
using TangerineAuction.Infrastructure.Keycloak;
using TangerineAuction.Infrastructure.Keycloak.Models;
using TangerineGenerator.Shared;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TangerineAuction.Infrastructure;

/// <summary>
/// Infrastructure layer dependency installer
/// </summary>
public class ModuleInstaller : IModuleInstaller
{
    
    public byte Order => 1;

    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        #region Cache

        services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(5);
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions
            {
                Configuration = configuration.GetConnectionString("Redis")
            }))
            .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
            {
                Configuration = configuration.GetConnectionString("Redis")
            }))
            .AsHybridCache();

        #endregion

        #region Database

        services.AddDbContext<AppInMemoryDb>();
        
        services.AddDbContextPool<AppDbContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("Postgres"), options =>
            {
                options.MigrationsAssembly(typeof(AppDbContext).Assembly);
                options.MigrationsHistoryTable(Constants.DbConstants.MigrationsHistoryTableName, 
                    Constants.DbConstants.PublicSchema);
            });
            opts.ReplaceService<IHistoryRepository, CustomHistoryRepository>();
            opts.UseSnakeCaseNamingConvention();
            
#if DEBUG
            opts.EnableSensitiveDataLogging();
#endif

            opts.UseSeeding((context, _) =>
            {
                new DataSeed().Seed((AppDbContext)context);
                context.SaveChanges();
            });

            opts.UseAsyncSeeding(async (context, _, token) => 
            {
                new DataSeed().Seed((AppDbContext)context);
                await context.SaveChangesAsync(token);
            });
        });
        
        // SnakeCaseNameRewriter requires CultureInfo
        services.AddSingleton(new System.Globalization.CultureInfo("ru-RU"));
        services.AddScoped<INameRewriter, SnakeCaseNameRewriter>();

        services.AddScoped<IMigrationService, MigrationService>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>));

        #endregion

        #region Keycloak

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddRefitClient<IKeycloakAdminApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(configuration["Keycloak:auth-server-url"]!);
            });

        services.AddSingleton<KeycloakTokenService>();
        services.AddSingleton<IKeycloakTokenService>(sp => sp.GetRequiredService<KeycloakTokenService>());
        services.AddHostedService(sp => sp.GetRequiredService<KeycloakTokenService>());
        
        services.Configure<SystemUserOptions>(configuration.GetSection("Keycloak:SystemUser"));
        services.AddSingleton<SystemUserService>();
        services.AddSingleton<ISystemUserService>(sp => sp.GetRequiredService<SystemUserService>());
        services.AddHostedService(sp => sp.GetRequiredService<SystemUserService>());

        services.AddScoped<IKeycloakService, KeycloakService>();

        #endregion

        #region Email

        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ModuleInstaller).Assembly);
        });

        #endregion

        #region GRPC

        services.AddCodeFirstGrpcClient<ITangerineGeneratorService>(x =>
        {
            x.Address = new Uri(configuration["GRPC:TangerineGeneratorService"]!);
        });

        #endregion
        
        #region MassTransit
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OnTangerineCreatedConsumer>();
            
            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(30);
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                
                o.UsePostgres();
                o.UseBusOutbox();
            });
            
            x.SetKebabCaseEndpointNameFormatter();
            
            x.AddConfigureEndpointsCallback((context, _, cfg) =>
            {
                cfg.UseEntityFrameworkOutbox<AppDbContext>(context);
            });
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], h =>
                {
                    h.Username(configuration["RabbitMQ:User"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });
                
                cfg.UseMessageRetry(r => r.Exponential(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(5)));
                cfg.ConfigureEndpoints(context);
            });
        });
        
        #endregion
    }
    
}