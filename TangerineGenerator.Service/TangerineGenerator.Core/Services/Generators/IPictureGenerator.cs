using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Services.Generators;

public interface IPictureGenerator
{
    Task<string> Generate(TangerineQuality quality, CancellationToken ct = default);
}