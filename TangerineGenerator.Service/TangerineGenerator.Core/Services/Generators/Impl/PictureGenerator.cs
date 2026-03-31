using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.ImageGeneration;
using Path = System.IO.Path;

namespace TangerineGenerator.Core.Services.Generators.Impl;

internal class PictureGenerator : IPictureGenerator
{
    
    private readonly string _outputDirectory;
    private readonly List<IPainter> _painters;

    public PictureGenerator(IEnumerable<IPainter> painters, IOptions<TangerineGeneratorOptions> options)
    {
        _painters = painters.OrderBy(x => x.Object).ToList();
        _outputDirectory = options.Value.PicturesOutputFolder;
        Directory.CreateDirectory(_outputDirectory);
    }

    public async Task<string> Generate(TangerineQuality quality, CancellationToken ct = default)
    {
        var fileName = $"{Guid.NewGuid():N}.png";
        var filePath = Path.Combine(_outputDirectory, fileName);

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

        await image.SaveAsync(filePath, new PngEncoder(), ct);
        return filePath;
    }
    
}