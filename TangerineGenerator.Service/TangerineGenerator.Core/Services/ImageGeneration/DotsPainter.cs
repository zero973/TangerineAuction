using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared.Enums;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class DotsPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Dots;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var draw = PaintHelper.Chance(
            context.Quality switch
            {
                TangerineQuality.Legendary => 90,
                TangerineQuality.Rare => 75,
                TangerineQuality.Uncommon => 60,
                TangerineQuality.Common => 45,
                _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
            });
        
        if (!draw) 
            return;
        
        var dots = context.Quality switch
        {
            TangerineQuality.Common => 10,
            TangerineQuality.Uncommon => 16,
            TangerineQuality.Rare => 26,
            TangerineQuality.Legendary => 38,
            _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
        };

        var dotColor = Color.FromRgba(255, 215, 140, 95);

        for (var i = 0; i < dots; i++)
        {
            var angle = Random.Shared.NextSingle() * MathF.PI * 2f;
            var distance = Random.Shared.NextSingle() * context.Radius * 0.78f;
            var x = context.Center.X + MathF.Cos(angle) * distance;
            var y = context.Center.Y + MathF.Sin(angle) * distance * 0.92f;

            ctx.Fill(dotColor, new EllipsePolygon(new PointF(x, y), 2.6f));
        }
    }
    
}