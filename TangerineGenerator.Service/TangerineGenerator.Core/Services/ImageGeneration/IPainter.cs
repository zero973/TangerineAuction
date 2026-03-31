using SixLabors.ImageSharp.Processing;
using TangerineGenerator.Core.Enums;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.ImageGeneration;

public interface IPainter
{
    
    PaintObject Object { get; }
    
    void Paint(IImageProcessingContext ctx, PaintContext context);
    
}