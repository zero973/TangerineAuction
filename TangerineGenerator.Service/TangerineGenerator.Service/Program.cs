using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace TangerineGenerator.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
    
        LogManager.Setup().LoadConfigurationFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog.config"));

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.UseNLog(); 
    
        builder.ConfigureWebHostDefaults(webHostBuilder =>
        {
            webHostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>();
        });
        return builder;
    }
}