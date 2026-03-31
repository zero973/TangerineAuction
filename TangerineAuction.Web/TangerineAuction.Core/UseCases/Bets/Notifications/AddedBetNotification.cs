using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TangerineAuction.Core.Helpers;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Bets.Specifications;
using TangerineAuction.Core.UseCases.Email;

namespace TangerineAuction.Core.UseCases.Bets.Notifications;

public record AddedBetNotification(Guid AuctionId) : INotification;

internal class AddedBetNotificationHandler(
    HybridCache cache,
    IReadRepository<Auction> auctionRepository,
    IReadRepository<Bet> betRepository,
    IKeycloakService keycloakService,
    ISystemUserService systemUserService,
    ISender sender)
    : INotificationHandler<AddedBetNotification>
{
    public async Task Handle(AddedBetNotification notification, CancellationToken ct)
    {
        await cache.RemoveAsync(CacheKeys.GetAuctionKey(notification.AuctionId), ct);

        var auction = (await auctionRepository.GetByIdAsync(notification.AuctionId, ct))!;
        var bet = await betRepository.FirstOrDefaultAsync(new BetSpecification(notification.AuctionId).GetSecondLastBet(), ct);

        if (bet == null || bet.CreatedBy == systemUserService.UserId)
            return;
        
        var mailAddress = await keycloakService.GetUserEmail(bet.CreatedBy.ToString(), ct);
        await sender.Send(new SendEmail.Command(
            mailAddress, 
            "Ваша ставка была перебита", 
            $"Ваша ставка на аукционе '{auction.Name}' была перебита ! Текущая ставка: {bet.Price:C2}"), 
            ct);
    }
}