using Ardalis.Specification;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions.Specifications;

internal class AuctionFullResponseSpecification : Specification<Auction, AuctionFullResponse>
{

    public AuctionFullResponseSpecification()
    {
        Query.Select<Auction, AuctionFullResponse>(auction => new AuctionFullResponse(
            auction, 
            new TangerineResponse(auction.Tangerine), 
            auction.Bets.Select(bet => new BetResponse(bet)).ToList())
        );
    }
    
    public AuctionFullResponseSpecification WithId(Guid id)
    {
        Query.Where(auction => auction.Id == id);
        return this;
    }
    
}