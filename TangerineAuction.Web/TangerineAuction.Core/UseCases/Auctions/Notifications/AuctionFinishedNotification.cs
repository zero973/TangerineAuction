using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TangerineAuction.Core.Helpers;
using TangerineAuction.Core.Services;

namespace TangerineAuction.Core.UseCases.Auctions.Notifications;

public record AuctionFinishedNotification(Guid AuctionId) : INotification;

internal class AuctionDeletedNotificationHandler(HybridCache cache)
    : INotificationHandler<AuctionFinishedNotification>
{
    public Task Handle(AuctionFinishedNotification notification, CancellationToken ct)
        => cache.RemoveAsync(CacheKeys.GetAuctionKey(notification.AuctionId), ct).AsTask();
}