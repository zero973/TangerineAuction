using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TangerineGenerator.Core.Helpers;

internal static class PaintHelper
{
    
    public static bool Chance(int percent) => Random.Shared.Next(0, 100) < percent;
    
    public static Color RandomTint(Color[] palette, int jitter)
    {
        var baseColor = palette[Random.Shared.Next(palette.Length)];
        return Jitter(baseColor, jitter);
    }

    private static Color Jitter(Color color, int maxDelta)
    {
        var pixel = color.ToPixel<Rgba32>();

        var r = Clamp(pixel.R + Random.Shared.Next(-maxDelta, maxDelta + 1));
        var g = Clamp(pixel.G + Random.Shared.Next(-maxDelta, maxDelta + 1));
        var b = Clamp(pixel.B + Random.Shared.Next(-maxDelta, maxDelta + 1));

        return Color.FromRgb(r, g, b);
    }

    private static byte Clamp(int value) => (byte)Math.Clamp(value, 0, 255);
    
}