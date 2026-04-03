using TangerineAuction.Shared.Enums;

namespace TangerineGenerator.Core.Services.Generators.Impl;

internal class PriceGenerator : IPriceGenerator
{
    
    private const decimal CommonBuyPrice = 250_000m;
    private const decimal UncommonBuyPrice = 500_000m;
    private const decimal RareBuyPrice = 750_000m;
    private const decimal LegendaryBuyPrice = 1_000_000m;
    
    public (decimal StartPrice, decimal BuyPrice) Generate(TangerineQuality quality)
    {
        var buyPrice = quality switch
        {
            TangerineQuality.Common => CommonBuyPrice,
            TangerineQuality.Uncommon => UncommonBuyPrice,
            TangerineQuality.Rare => RareBuyPrice,
            TangerineQuality.Legendary => LegendaryBuyPrice,
            _ => throw new NotImplementedException($"Не обработан вариант {quality}")
        };
        
        var basePrice = quality switch
        {
            TangerineQuality.Common => 1m,
            TangerineQuality.Uncommon => CommonBuyPrice,
            TangerineQuality.Rare => UncommonBuyPrice,
            TangerineQuality.Legendary => RareBuyPrice,
            _ => throw new NotImplementedException($"Не обработан вариант {quality}")
        };

        var jitter = Random.Shared.Next(0, 250_000);
        
        return (basePrice + jitter, buyPrice * 2);
    }
    
}