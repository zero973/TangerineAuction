using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TangerineAuction.Core.Behaviors;
using TangerineAuction.Core.Jobs;
using TangerineAuction.Shared.Hangfire;

namespace TangerineAuction.Core.Modules;

public class ModuleInstaller : IModuleInstaller
{
    
    public byte Order => 2;

    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<ModuleInstaller>(includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ModuleInstaller).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IRecurringJob, FinishAuctionJob>();
        services.AddScoped<IRecurringJob, CreateTangerineJob>();
    }
    
}