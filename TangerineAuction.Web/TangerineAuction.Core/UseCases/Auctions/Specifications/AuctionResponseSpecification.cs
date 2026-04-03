using Ardalis.Specification;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.UseCases.Auctions.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions.Specifications;

internal class AuctionResponseSpecification : Specification<Auction, AuctionResponse>
{

    public AuctionResponseSpecification()
    {
        Query.Select<Auction, AuctionResponse>(x => new AuctionResponse(
            x.Id,
            x.Name,
            x.IsActual,
            x.CreatedOn,
            x.Tangerine.Name,
            x.Tangerine.Quality,
            x.Tangerine.FileName,
            x.Bets.OrderByDescending(b => b.CreatedOn).First().Price,
            x.Bets.OrderByDescending(b => b.CreatedOn).First().CreatedBy));
    }

    public AuctionResponseSpecification WithActual()
    {
        Query.Where(x => x.IsActual);
        return this;
    }
    
    public AuctionResponseSpecification WithSearchParams(AuctionSearchParams prms, Guid? currentUserId = null)
    {
        Query.Skip(prms.Skip).Take(prms.Take);
        
        if (!string.IsNullOrWhiteSpace(prms.AuctionName))
            Query.Where(x => x.Name.Contains(prms.AuctionName));
        
        if (!string.IsNullOrWhiteSpace(prms.TangerineName))
            Query.Where(x => x.Tangerine.Name.Contains(prms.TangerineName));
        
        if (prms.TangerineQuality != null)
            Query.Where(x => x.Tangerine.Quality == prms.TangerineQuality);
        
        if (!prms.ShowFinishedAuctions)
            Query.Where(x => x.IsActual);
        
        if (prms.IsCurrentUserWinner && currentUserId.HasValue && currentUserId != Guid.Empty)
            Query.Where(x => x.Bets.OrderByDescending(b => b.CreatedOn).First().CreatedBy == currentUserId);
        
        return this;
    }
    
}