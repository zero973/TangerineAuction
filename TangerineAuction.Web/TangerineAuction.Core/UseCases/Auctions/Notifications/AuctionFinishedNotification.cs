using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TangerineAuction.Core.Enums;
using TangerineAuction.Core.Helpers;
using TangerineAuction.Core.UseCases.SignalR;

namespace TangerineAuction.Core.UseCases.Auctions.Notifications;

public class AuctionFinished
{
    
    public record Notification(Guid AuctionId) : INotification;

    internal class AuctionDeletedNotificationHandler(HybridCache cache, ISender sender) : INotificationHandler<Notification>
    {
        public async Task Handle(Notification notification, CancellationToken ct)
        {
            await cache.RemoveAsync(CacheKeys.GetAuctionKey(notification.AuctionId), ct);
            await sender.Send(new SendMessage.Command(HubMessageType.AuctionFinished, notification.AuctionId), ct);
        }
    }
    
}