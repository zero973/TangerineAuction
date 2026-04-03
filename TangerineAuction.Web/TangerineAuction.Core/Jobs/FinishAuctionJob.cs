using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Specifications;
using TangerineAuction.Shared.Hangfire;

namespace TangerineAuction.Core.Jobs;

public class FinishAuctionJob(IReadRepository<Auction> repository, ISender sender, ILogger<FinishAuctionJob> logger) 
    : IRecurringJob
{
    
    public string Name => "Задача финиширования аукционов";
    
    public string Cron => "0 3 * * *";

    public async Task ExecuteAsync()
    {
        try
        {
            var auctions = await repository.ListAsync(new AuctionResponseSpecification().WithActual());
            foreach (var auction in auctions)
                await sender.Send(new FinishAuction.Command(auction.AuctionId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Name}. {ExMessage}", Name, ex.Message);
            throw;
        }
    }
    
}