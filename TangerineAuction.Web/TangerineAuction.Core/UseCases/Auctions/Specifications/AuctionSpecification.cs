using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.UseCases.Auctions.Specifications;

internal class AuctionSpecification : Specification<Auction>
{
    public AuctionSpecification WithId(Guid id)
    {
        Query.Where(x => x.Id == id);
        return this;
    }
}