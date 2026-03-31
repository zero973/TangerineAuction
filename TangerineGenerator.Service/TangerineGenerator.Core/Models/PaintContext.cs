using SixLabors.ImageSharp;
using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Models;

public record PaintContext(TangerineQuality Quality, float Radius, PointF Center);