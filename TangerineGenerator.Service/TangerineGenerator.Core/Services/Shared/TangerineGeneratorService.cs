using TangerineGenerator.Shared;
using System.Reflection;
using TangerineAuction.Shared.Models;
using TangerineGenerator.Core.Services.FileStorages;

namespace TangerineGenerator.Core.Services.Shared;

public class TangerineGeneratorService(IFileStorage storage) : ITangerineGeneratorService
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

    public Task DeleteImage(string fileName, CancellationToken ct) => storage.Delete(fileName, ct);
    
    public async Task<Dictionary<string, string>> GetPhotoUrls(List<string> tangerineFileNames, CancellationToken ct)
    {
        var result = new Dictionary<string, string>();
        foreach (var fileName in tangerineFileNames)
        {
            if (ct.IsCancellationRequested)
                return result;
            
            var url = await storage.GetFileUrl(fileName);
            result.Add(fileName, url);
        }
        return result;
    }
    
}