using TangerineAuction.Shared;

namespace TangerineGenerator.Core.Services.Generators.Impl;

internal class PriceGenerator : IPriceGenerator
{
    public decimal Generate(TangerineQuality quality)
    {
        var basePrice = quality switch
        {
            TangerineQuality.Common => 1m,
            TangerineQuality.Uncommon => 250_000m,
            TangerineQuality.Rare => 500_000m,
            TangerineQuality.Legendary => 750_000m,
            _ => throw new NotImplementedException($"Не обработан вариант {quality}")
        };

        var jitter = Random.Shared.Next(0, 250_000);
        return basePrice + jitter;
    }
}