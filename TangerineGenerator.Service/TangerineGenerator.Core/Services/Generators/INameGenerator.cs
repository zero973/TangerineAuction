using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Services.Generators;

public interface INameGenerator
{
    string Generate(TangerineQuality quality);
}