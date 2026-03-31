using Ardalis.Specification;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.UseCases.Auctions.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions.Specifications;

internal class AuctionResponseSpecification : Specification<Auction, AuctionResponse>
{

    public AuctionResponseSpecification()
    {
        Query.Where(x => x.IsActual);
        
        Query.Select<Auction, AuctionResponse>(x => new AuctionResponse(
            x.Id,
            x.Name,
            x.CreatedOn,
            x.Tangerine.Name,
            x.Tangerine.Quality,
            x.Tangerine.FilePath,
            x.Bets
                .OrderByDescending(b => b.CreatedOn)
                .Select(b => b.Price)
                .First(),
            x.Bets
                .OrderByDescending(b => b.CreatedOn)
                .Select(b => b.CreatedBy)
                .First()));
    }
    
    public AuctionResponseSpecification WithSearchParams(AuctionSearchParams prms)
    {
        Query.Skip(prms.Skip).Take(prms.Take);
        
        if (!string.IsNullOrWhiteSpace(prms.Name))
            Query.Where(x => x.Name.Contains(prms.Name));
        
        return this;
    }
    
}