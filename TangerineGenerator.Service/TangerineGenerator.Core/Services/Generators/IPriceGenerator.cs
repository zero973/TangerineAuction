using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Services.Generators;

public interface IPriceGenerator
{
    decimal Generate(TangerineQuality quality);
}