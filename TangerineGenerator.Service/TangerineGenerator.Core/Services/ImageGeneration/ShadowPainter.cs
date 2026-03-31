using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class ShadowPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Shadow;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var shadowCenter = new PointF(
            context.Center.X + context.Radius * 0.40f,
            context.Center.Y + context.Radius * 0.38f);

        ctx.Fill(Color.FromRgba(0, 0, 0, 45), new EllipsePolygon(shadowCenter, context.Radius * 0.92f));
    }
    
}