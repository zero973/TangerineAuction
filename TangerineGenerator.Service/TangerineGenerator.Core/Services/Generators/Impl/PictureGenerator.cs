using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared.Enums;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.FileStorages;
using TangerineGenerator.Core.Services.ImageGeneration;

namespace TangerineGenerator.Core.Services.Generators.Impl;

internal class PictureGenerator : IPictureGenerator
{
    
    private readonly IFileStorage _fileStorage;
    private readonly List<IPainter> _painters;

    public PictureGenerator(IFileStorage fileStorage, IEnumerable<IPainter> painters)
    {
        _fileStorage = fileStorage;
        _painters = painters.OrderBy(x => x.Object).ToList();
    }

    public async Task<string> Generate(TangerineQuality quality, CancellationToken ct = default)
    {
        var fileName = $"{Guid.NewGuid():N}.png";

        using var image = new Image<Rgba32>(512, 512);

        image.Mutate(ctx =>
        {
            var baseRadius = quality switch
            {
                TangerineQuality.Common => 100f,
                TangerineQuality.Uncommon => 125f,
                TangerineQuality.Rare => 150f,
                TangerineQuality.Legendary => 175f,
                _ => 110f
            };

            var radius = baseRadius + Random.Shared.Next(0, 25);

            var center = new PointF(
                256f + Random.Shared.NextSingle() * radius * 0.12f - radius * 0.06f,
                282f + Random.Shared.NextSingle() * radius * 0.08f - radius * 0.04f);
            
            var paintContext = new PaintContext(quality, radius, center);
            _painters.ForEach(x => x.Paint(ctx, paintContext));
        });

        await using var stream = new MemoryStream();
        await image.SaveAsPngAsync(stream, ct);
        stream.Position = 0;

        await _fileStorage.Save(stream, fileName, "image/png", ct);

        return fileName;
    }
    
}