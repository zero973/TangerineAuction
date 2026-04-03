namespace TangerineAuction.Core.UseCases.Bets.Dtos;

public class BetRequest
{
    
    /// <summary>
    /// Id аукциона
    /// </summary>
    public Guid AuctionId { get; set; }
    
    /// <summary>
    /// Сумма ставки
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Кто сделал ставку
    /// </summary>
    public Guid? CreatedBy { get; set; }

    public BetRequest(Guid auctionId, decimal price)
    {
        AuctionId = auctionId;
        Price = price;
    }
    
}