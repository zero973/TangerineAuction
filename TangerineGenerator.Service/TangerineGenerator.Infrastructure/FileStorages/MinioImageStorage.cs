using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.FileStorages;

namespace TangerineGenerator.Infrastructure.FileStorages;

internal class MinioImageStorage : IFileStorage, IHostedService
{
    
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IMinioClient _minio;
    private readonly HybridCache _cache;
    private readonly MinioConfiguration _configuration;
    private readonly ILogger<MinioImageStorage> _logger;

    public MinioImageStorage(HybridCache cache, IOptions<MinioConfiguration> configuration, ILogger<MinioImageStorage> logger)
    {
        _cache = cache;
        _minio = new MinioClient()
            .WithEndpoint(configuration.Value.Endpoint)
            .WithCredentials(configuration.Value.AccessKey, configuration.Value.SecretKey)
            .Build();
        _configuration = configuration.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        if (!await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_configuration.BucketName), ct)) 
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_configuration.BucketName), ct);
        _lock.Release();
        _logger.LogInformation("Minio ready");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task<string> GetFileUrl(string fileName)
    {
        return await _cache.GetOrCreateAsync<string>(
            fileName, 
            async _ => await _minio.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(_configuration.BucketName)
                    .WithObject(fileName)
                    .WithExpiry(_configuration.ExpireTime)), 
            new HybridCacheEntryOptions() { Expiration = TimeSpan.FromDays(1) });
    }

    public async Task<string> Save(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var result = await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_configuration.BucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType), ct);
        return result.ObjectName;
    }

    public async Task Delete(string fileName, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(fileName, ct);
        await _minio.RemoveObjectAsync(
            new RemoveObjectArgs().WithBucket(_configuration.BucketName).WithObject(fileName), ct);
    }
}