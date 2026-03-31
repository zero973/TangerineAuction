using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Specifications;

namespace TangerineAuction.Core.Services.BackgroundServices;

internal class DeleteAuctionService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DeleteAuctionService> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IReadRepository<Auction>>();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                
                var auctions = await repository.ListAsync(new AuctionResponseSpecification(), ct);
                foreach (var auction in auctions)
                    await sender.Send(new FinishAuction.Command(auction.AuctionId), ct);
                
                await timer.WaitForNextTickAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(DeleteAuctionService)}. {ex.Message}");
            }
        }
    }
}