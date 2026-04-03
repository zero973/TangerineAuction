using TangerineAuction.Shared.Enums;

namespace TangerineGenerator.Core.Services.Generators;

public interface IPriceGenerator
{
    (decimal StartPrice, decimal BuyPrice) Generate(TangerineQuality quality);
}