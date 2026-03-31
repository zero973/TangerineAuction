using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

internal class StarsPainter : IPainter
{
    
    public PaintObject Object => PaintObject.Stars;

    public void Paint(IImageProcessingContext ctx, PaintContext context)
    {
        if (context.Quality != TangerineQuality.Legendary)
            return;
        
        var starCount = Random.Shared.Next(30, 50);

        for (var i = 0; i < starCount; i++)
        {
            var center = new PointF(
                Random.Shared.NextSingle() * 512f,
                Random.Shared.NextSingle() * 512f);

            var outerRadius = Random.Shared.NextSingle() * 3.5f + 1.5f;
            var innerRadius = outerRadius * 0.45f;

            var rotation = Random.Shared.NextSingle() * MathF.PI * 2f;

            var starColor = Random.Shared.Next(0, 3) switch
            {
                0 => Color.FromRgba(255, 244, 180, 180),
                1 => Color.FromRgba(255, 215, 120, 170),
                _ => Color.FromRgba(255, 255, 255, 140)
            };

            var star = CreateStarPolygon(center, outerRadius, innerRadius, rotation);
            ctx.Fill(starColor, star);
        }
    }
    
    private Polygon CreateStarPolygon(PointF center, float outerRadius, float innerRadius, float rotation)
    {
        var points = new PointF[10];

        for (var i = 0; i < 10; i++)
        {
            var angle = rotation - MathF.PI / 2f + i * MathF.PI / 5f;
            var radius = i % 2 == 0 ? outerRadius : innerRadius;

            points[i] = new PointF(
                center.X + MathF.Cos(angle) * radius,
                center.Y + MathF.Sin(angle) * radius);
        }

        return new Polygon(points);
    }
    
}