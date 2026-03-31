namespace TangerineAuction.Core.Models;

/// <summary>
/// Ставка
/// </summary>
public class Bet : BaseEntity
{
    
    /// <summary>
    /// Id аукциона
    /// </summary>
    public Guid AuctionId { get; private set; }
    
    /// <summary>
    /// Аукцион
    /// </summary>
    public Auction Auction { get; private set; } = null!;
    
    /// <summary>
    /// Сумма ставки
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; private set; }

    /// <summary>
    /// Кто сделал ставку
    /// </summary>
    public Guid CreatedBy { get; private set; }
    
#pragma warning disable CS8618 // Required by Entity Framework
    // ReSharper disable once UnusedMember.Local
    private Bet() {}

    private Bet(Guid auctionId, decimal price, Guid createdBy)
    {
        AuctionId = auctionId;
        Price = price;
        CreatedBy = createdBy;
        CreatedOn = DateTime.UtcNow;
    }
    
    public static Bet Create(Guid auctionId, decimal price, Guid createdBy)
    {
        return new Bet(auctionId, price, createdBy);
    }
    
}