using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.UseCases.Bets.Specifications;

internal class BetSpecification : Specification<Bet>
{

    public BetSpecification(Guid auctionId)
    {
        Query.Where(x => x.AuctionId == auctionId);
    }
    
    /// <summary>
    /// Получить последнюю ставку в аукционе
    /// </summary>
    public BetSpecification GetLastBet()
    {
        Query.OrderByDescending(b => b.CreatedOn);
        Query.Take(1);
        return this;
    }
    
    /// <summary>
    /// Получить предпоследнюю ставку в аукционе
    /// </summary>
    public BetSpecification GetSecondLastBet()
    {
        Query.OrderByDescending(b => b.CreatedOn);
        Query.Skip(1).Take(1);
        return this;
    }
    
}