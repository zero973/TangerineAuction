using TangerineAuction.Shared.Enums;

namespace TangerineGenerator.Core.Services.Generators;

public interface INameGenerator
{
    string Generate(TangerineQuality quality);
}