using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TangerineAuction.Core.Modules;

/// <summary>
/// Module dependency installer
/// </summary>
public interface IModuleInstaller
{

    /// <summary>
    /// Initialization order
    /// </summary>
    byte Order { get; }

    /// <summary>
    /// Register dependencies
    /// </summary>
    void Install(IServiceCollection services, IConfiguration configuration);

}