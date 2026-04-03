using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Server;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Modules;
using TangerineGenerator.Core.Services.FileStorages;
using TangerineGenerator.Infrastructure.Consumers;
using TangerineGenerator.Infrastructure.Data;
using TangerineGenerator.Infrastructure.FileStorages;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TangerineGenerator.Infrastructure;

internal class ModuleInstaller : IModuleInstaller
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
        
        #region Minio

        services.Configure<MinioConfiguration>(configuration.GetSection("Minio"));
        services.AddSingleton<MinioImageStorage>();
        services.AddSingleton<IFileStorage>(sp => sp.GetRequiredService<MinioImageStorage>());
        services.AddHostedService(sp => sp.GetRequiredService<MinioImageStorage>());

        #endregion

        #region Databese

        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("Postgres"));
            opts.UseSnakeCaseNamingConvention();
        });

        #endregion

        #region MassTransit

        services.AddMassTransit(x =>
        {
            x.AddConsumer<GenerateTangerineRequestConsumer>();
            
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
                
                cfg.ConfigureEndpoints(context);
            });
        });

        #endregion
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ModuleInstaller).Assembly);
        });
        
        services.AddCodeFirstGrpc();
        services.AddRouting();
    }
    
}