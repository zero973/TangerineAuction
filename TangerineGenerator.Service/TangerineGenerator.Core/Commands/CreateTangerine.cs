using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Shared.Enums;
using TangerineAuction.Shared.Models;
using TangerineGenerator.Core.Services.Generators;

namespace TangerineGenerator.Core.Commands;

public class CreateTangerine
{
    
    public record Command() : IRequest<TangerineInfo>;
    
    internal class RequestHandler(
        INameGenerator nameGenerator, 
        IPriceGenerator priceGenerator, 
        IPictureGenerator pictureGenerator,
        ILogger<RequestHandler> logger) 
        : IRequestHandler<Command, TangerineInfo>
    {
        public async Task<TangerineInfo> Handle(Command request, CancellationToken ct)
        {
            var quality = Random.Shared.Next(0, 100) switch
            {
                < 60 => TangerineQuality.Common,
                < 85 => TangerineQuality.Uncommon,
                < 95 => TangerineQuality.Rare,
                _ => TangerineQuality.Legendary
            };
        
            var name = nameGenerator.Generate(quality);
            var (startPrice, buyPrice) = priceGenerator.Generate(quality);
            var fileName = await pictureGenerator.Generate(quality, ct);

            var tangerine = new TangerineInfo(name, quality, startPrice, buyPrice, fileName, DateTime.UtcNow);

            logger.LogInformation("Created tangerine: {TangerineInfo}", tangerine);
        
            return tangerine;
        }
    }
    
}