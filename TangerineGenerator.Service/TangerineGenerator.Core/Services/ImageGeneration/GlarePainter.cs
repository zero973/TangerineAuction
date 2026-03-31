using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class GlarePainter : IPainter
{
    
    public PaintObject Object => PaintObject.Glare;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var draw = PaintHelper.Chance(
            context.Quality switch
            {
                TangerineQuality.Legendary => 50,
                TangerineQuality.Rare => 25,
                TangerineQuality.Uncommon => 10,
                TangerineQuality.Common => 5,
                _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
            });
        
        if (!draw) 
            return;
        
        var glareCenter = new PointF(
            context.Center.X - context.Radius * 0.30f,
            context.Center.Y - context.Radius * 0.30f);

        ctx.Fill(Color.FromRgba(255, 255, 255, 95), new EllipsePolygon(glareCenter, context.Radius * 0.34f));
    }
    
}