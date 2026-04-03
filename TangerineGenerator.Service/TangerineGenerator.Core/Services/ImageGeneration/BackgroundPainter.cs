using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared.Enums;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class BackgroundPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Background;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var background = context.Quality switch
        {
            TangerineQuality.Legendary => PaintHelper.RandomTint(
                [Color.FromRgb(12, 12, 18), Color.FromRgb(20, 18, 30), Color.FromRgb(30, 18, 12), Color.FromRgb(10, 24, 20)], 16),
            TangerineQuality.Rare => PaintHelper.RandomTint(
                [Color.FromRgb(30, 55, 120), Color.FromRgb(42, 72, 150), Color.FromRgb(60, 85, 170)], 18),
            TangerineQuality.Uncommon => PaintHelper.RandomTint(
                [Color.FromRgb(245, 240, 230), Color.FromRgb(255, 248, 235), Color.FromRgb(244, 236, 220)], 22),
            TangerineQuality.Common => PaintHelper.RandomTint(
                [Color.White, Color.FromRgb(248, 248, 245), Color.FromRgb(255, 250, 240)], 18),
            _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
        };
        ctx.Fill(background);
    }
    
}