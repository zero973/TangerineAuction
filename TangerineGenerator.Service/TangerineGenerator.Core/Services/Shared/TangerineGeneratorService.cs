using Microsoft.Extensions.Options;
using System.Reflection;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Shared;

namespace TangerineGenerator.Core.Services.Shared;

public class TangerineGeneratorService(IOptions<TangerineGeneratorOptions> options) : ITangerineGeneratorService
{
    
    public ValueTask<VersionInfo> GetVersion()
    {
        var file = new FileInfo(Assembly.GetEntryAssembly()!.Location);

        return new ValueTask<VersionInfo>(new VersionInfo
        {
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
            BuildDate = file.LastWriteTime
        });
    }

    public ValueTask DeleteImage(string filePath)
    {
        var file = Path.Combine(options.Value.PicturesOutputFolder, filePath);
        if (File.Exists(file))
            File.Delete(file);
        return ValueTask.CompletedTask;
    }
    
}