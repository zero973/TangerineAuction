using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class HaloPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Halo;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var draw = context.Quality switch
        {
            TangerineQuality.Legendary => true,
            TangerineQuality.Rare => PaintHelper.Chance(25),
            TangerineQuality.Uncommon => false,
            TangerineQuality.Common => false,
            _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
        };
        
        if (!draw) 
            return;
        
        ctx.Draw(Color.FromRgba(255, 215, 0, 180), 6, new EllipsePolygon(context.Center, context.Radius + 18));
    }
    
}