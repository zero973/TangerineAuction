using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Helpers;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class LeafPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Leaf;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        var draw = context.Quality switch
        {
            TangerineQuality.Legendary => true,
            TangerineQuality.Rare => PaintHelper.Chance(80),
            TangerineQuality.Uncommon => PaintHelper.Chance(50),
            TangerineQuality.Common => PaintHelper.Chance(35),
            _ => throw new NotImplementedException($"Не обработан вариант {context.Quality}")
        };
        
        if (!draw) 
            return;
        
        var leafColor = context.Quality switch
        {
            TangerineQuality.Common => Color.ForestGreen,
            TangerineQuality.Uncommon => Color.SeaGreen,
            TangerineQuality.Rare => Color.DarkGreen,
            TangerineQuality.Legendary => Color.MediumSeaGreen,
            _ => Color.ForestGreen
        };

        var leafWidth = context.Radius * 0.34f;
        var leafHeight = context.Radius * 0.22f;

        // Координата прикрепления листика к верхушке мандаринки
        var attachX = context.Center.X + Random.Shared.NextSingle() * context.Radius * 0.18f - context.Radius * 0.09f;
        var attachY = context.Center.Y - context.Radius * 0.92f;

        // Лист чуть выступает вверх и немного смещается в сторону
        var leafCenter = new PointF(
            attachX + Random.Shared.NextSingle() * context.Radius * 0.08f - context.Radius * 0.04f,
            attachY - leafHeight * 0.55f);

        var tilt = Random.Shared.NextSingle() * 0.5f - 0.25f;

        var points = new[]
        {
            Rotate(new PointF(leafCenter.X - leafWidth * 0.55f, leafCenter.Y + leafHeight * 0.10f), leafCenter, tilt),
            Rotate(new PointF(leafCenter.X - leafWidth * 0.10f, leafCenter.Y - leafHeight * 0.55f), leafCenter, tilt),
            Rotate(new PointF(leafCenter.X + leafWidth * 0.58f, leafCenter.Y - leafHeight * 0.15f), leafCenter, tilt),
            Rotate(new PointF(leafCenter.X + leafWidth * 0.18f, leafCenter.Y + leafHeight * 0.52f), leafCenter, tilt)
        };
        
        // Небольшой "черешок"
        var stemStart = new PointF(attachX, attachY + 2f);
        var stemEnd = new PointF(leafCenter.X, leafCenter.Y + leafHeight * 0.10f);
        ctx.DrawLine(Color.FromRgba(90, 60, 20, 180), 4, stemStart, stemEnd);
        
        ctx.Fill(leafColor, new Polygon(points));
        
        // Лёгкий блик на листике
        var vein = new[]
        {
            Rotate(new PointF(leafCenter.X - leafWidth * 0.18f, leafCenter.Y - leafHeight * 0.10f), leafCenter, tilt),
            Rotate(new PointF(leafCenter.X + leafWidth * 0.26f, leafCenter.Y - leafHeight * 0.02f), leafCenter, tilt)
        };
        
        ctx.DrawLine(Color.FromRgba(255, 255, 255, 60), 2, vein[0], vein[1]);
    }

    private PointF Rotate(PointF p, PointF center, float radians)
    {
        var sin = MathF.Sin(radians);
        var cos = MathF.Cos(radians);

        var dx = p.X - center.X;
        var dy = p.Y - center.Y;

        return new PointF(center.X + dx * cos - dy * sin, center.Y + dx * sin + dy * cos);
    }
    
}