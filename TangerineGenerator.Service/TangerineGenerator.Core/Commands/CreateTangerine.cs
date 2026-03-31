using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Shared;
using TangerineGenerator.Core.Notifications;
using TangerineGenerator.Core.Services.Generators;

namespace TangerineGenerator.Core.Commands;

public class CreateTangerine
{
    
    public record Command() : IRequest<TangerineInfo>;
    
    internal class RequestHandler(
        INameGenerator nameGenerator, 
        IPriceGenerator priceGenerator, 
        IPictureGenerator pictureGenerator,
        IPublisher publisher,
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
            var price = priceGenerator.Generate(quality);
            var picturePath = await pictureGenerator.Generate(quality, ct);

            var tangerine = new TangerineInfo(name, quality, price, Path.GetFileName(picturePath),DateTime.UtcNow);

            logger.LogInformation($"Created tangerine: {tangerine}");
            
            await publisher.Publish(new OnTangerineGeneratedNotification(tangerine), ct);
        
            return tangerine;
        }
    }
    
}