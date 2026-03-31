using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Models;
using TangerineGenerator.Core.Services.Generators.Impl;
using TangerineGenerator.Core.Services.ImageGeneration;

namespace TangerineGenerator.Tests;

public class UnitTests
{
    
    /// <summary>
    /// Проверка создания изображений
    /// </summary>
    [Fact]
    public async Task CheckImageGeneration()
    {
        var folder = "D:\\TestPics";
        var options = Options.Create(new TangerineGeneratorOptions { PicturesOutputFolder = folder });
        
        var service = new PictureGenerator(GetPainters().Values, options);

        foreach (var type in Enum.GetValues(typeof(TangerineQuality)))
        {
            var filePath = await service.Generate((TangerineQuality)type);
            Assert.True(File.Exists(filePath));
        }
    }

    /// <summary>
    /// Проверка, что у всех реализаций <see cref="IPainter"/> разные <see cref="IPainter.Object"/>
    /// (в методе GetPainters деламем "ToDictionary")
    /// </summary>
    [Fact]
    public void AreAllPaintersUnique()
    {
        Assert.True(GetPainters().Count != 0);
    }

    private Dictionary<PaintObject, IPainter> GetPainters()
        => typeof(IPainter).Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } && typeof(IPainter).IsAssignableFrom(x))
            .Select(x => (IPainter)Activator.CreateInstance(x)!)
            .ToDictionary(x => x.Object, x => x);
}