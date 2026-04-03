using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TangerineAuction.Shared.Enums;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.Generators.Impl;
using TangerineGenerator.Core.Services.ImageGeneration;
using TangerineGenerator.Infrastructure.FileStorages;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TangerineGenerator.Tests;

public class Tests
{
    
    /// <summary>
    /// Проверка создания изображений
    /// </summary>
    [Fact]
public async Task CheckImageGeneration()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    var services = new ServiceCollection();

    services.AddSingleton<IConfiguration>(configuration);

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

    services.AddLogging();

    var provider = services.BuildServiceProvider();

    var hybridCache = provider.GetRequiredService<HybridCache>();

    var options = Options.Create(
        configuration.GetSection("Minio").Get<MinioConfiguration>()
        ?? throw new InvalidOperationException("Minio configuration is missing"));

    var logger = NullLogger<MinioImageStorage>.Instance;

    var fileStorage = new MinioImageStorage(hybridCache, options, logger);
    await fileStorage.StartAsync(CancellationToken.None);

    var service = new PictureGenerator(fileStorage, GetPainters().Values);

    var fileNames = new List<string>();

    foreach (var type in Enum.GetValues<TangerineQuality>())
    {
        var fileName = await service.Generate(type);
        var url = await fileStorage.GetFileUrl(fileName);

        fileNames.Add(fileName);

        Assert.False(string.IsNullOrWhiteSpace(fileName));
        Assert.False(string.IsNullOrWhiteSpace(url));
        Assert.Equal(fileName, Path.GetFileName(new Uri(url).LocalPath));
    }

    foreach (var fileName in fileNames)
        await fileStorage.Delete(fileName, CancellationToken.None);
}

    /// <summary>
    /// Проверка, что у всех реализаций <see cref="IPainter"/> разные <see cref="IPainter.Object"/>
    /// (в методе GetPainters делаем ToDictionary)
    /// </summary>
    [Fact]
    public void AreAllPaintersUnique()
    {
        Assert.NotEmpty(GetPainters());
    }

    private static Dictionary<PaintObject, IPainter> GetPainters()
        => typeof(IPainter).Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } && typeof(IPainter).IsAssignableFrom(x))
            .Select(x => (IPainter)Activator.CreateInstance(x)!)
            .ToDictionary(x => x.Object, x => x);
}