using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Server;
using TangerineGenerator.Core.Modules;
using TangerineGenerator.Infrastructure.Consumers;
using TangerineGenerator.Infrastructure.Data;

namespace TangerineGenerator.Infrastructure;

internal class ModuleInstaller : IModuleInstaller
{

    public byte Order => 1;
    
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("Postgres"));
            opts.UseSnakeCaseNamingConvention();
        });
        
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
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ModuleInstaller).Assembly);
        });
        
        services.AddCodeFirstGrpc();
        services.AddRouting();
    }
    
}