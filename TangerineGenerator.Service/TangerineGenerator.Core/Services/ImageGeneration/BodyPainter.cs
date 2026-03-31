using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class BodyPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Body;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var bodyColor = GetBodyColor(context.Quality);
        ctx.Fill(bodyColor, new EllipsePolygon(context.Center, context.Radius));
    }
    
    private Color GetBodyColor(TangerineQuality quality)
    {
        return quality switch
        {
            TangerineQuality.Legendary => PaintHelper.RandomTint(
                [
                    Color.FromRgb(235, 205, 95),
                    Color.FromRgb(245, 225, 150),
                    Color.FromRgb(225, 190, 80),
                    Color.FromRgb(250, 245, 235)
                ],
                12),
            TangerineQuality.Rare => PaintHelper.RandomTint(
                [
                    Color.FromRgb(210, 60, 70),
                    Color.FromRgb(190, 55, 95),
                    Color.FromRgb(220, 80, 60)
                ],
                16),
            TangerineQuality.Uncommon => PaintHelper.RandomTint(
                [
                    Color.FromRgb(245, 195, 55),
                    Color.FromRgb(255, 210, 70),
                    Color.FromRgb(235, 180, 40)
                ],
                14),
            TangerineQuality.Common => PaintHelper.RandomTint(
                [
                    Color.FromRgb(255, 145, 35),
                    Color.FromRgb(255, 155, 45),
                    Color.FromRgb(255, 138, 25)
                ],
                14),
            _ => throw new NotImplementedException($"Не обработан вариант {quality}")
        };
    }
    
}