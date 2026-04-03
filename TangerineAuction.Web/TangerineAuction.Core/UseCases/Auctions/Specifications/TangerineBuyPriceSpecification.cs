using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.UseCases.Auctions.Specifications;

internal class TangerineBuyPriceSpecification : Specification<Auction, decimal>
{
    public TangerineBuyPriceSpecification(Guid auctionId)
    {
        Query.Where(x => x.Id == auctionId).Select(x => x.Tangerine.BuyPrice);
    }
}