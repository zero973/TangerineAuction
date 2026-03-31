using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TangerineGenerator.Core.Modules;
using TangerineGenerator.Core.Services.Shared;

namespace TangerineGenerator.Service;

internal class Startup(IConfiguration configuration)
{
    
    public void ConfigureServices(IServiceCollection services)
    {
        LoadAssemblies();

        AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic)
            .SelectMany(a => a.GetTypes().Where(x => typeof(IModuleInstaller).IsAssignableFrom(x)))
            .Where(x => !x.IsAbstract)
            .Select(x => (IModuleInstaller)Activator.CreateInstance(x)!)
            .OrderBy(x => x.Order)
            .ToList()
            .ForEach(x => x.Install(services, configuration));
    }
    
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<TangerineGeneratorService>();
        });
    }

    private void LoadAssemblies()
    {
        Console.WriteLine("Загрузка сборок модулей");
        var binFolder = $"{AppDomain.CurrentDomain.BaseDirectory}";
    
        var libraries = new DirectoryInfo(binFolder)
            .GetFiles("*.dll")
            .Where(x => x.Name.Contains("TangerineGenerator"))
            .Select(x => AssemblyName.GetAssemblyName(x.FullName));

        foreach (var asmName in libraries)
        {
            try
            {
                Assembly.Load(asmName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка загрузки сборки: {asmName}", e);
                throw;
            }
        }
    }
    
}