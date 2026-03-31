using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.BackgroundServices;
using TangerineGenerator.Core.Services.Generators;
using TangerineGenerator.Core.Services.Generators.Impl;
using TangerineGenerator.Core.Services.ImageGeneration;

namespace TangerineGenerator.Core.Modules;

internal class ModuleInstaller : IModuleInstaller
{
    
    public byte Order => 2;

    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TangerineGeneratorOptions>(configuration.GetSection("TangerineGenerationOptions"));
        
        var painterInterface = typeof(IPainter);
        var painterTypes = painterInterface.Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } && typeof(IPainter).IsAssignableFrom(x));
        foreach (var painterType in painterTypes)
            services.AddScoped(painterInterface, painterType);
        
        services.AddScoped<INameGenerator, NameGenerator>();
        services.AddScoped<IPriceGenerator, PriceGenerator>();
        services.AddScoped<IPictureGenerator, PictureGenerator>();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ModuleInstaller).Assembly);
        });
        
        services.AddHostedService<TangerineCreationService>();
    }
    
}