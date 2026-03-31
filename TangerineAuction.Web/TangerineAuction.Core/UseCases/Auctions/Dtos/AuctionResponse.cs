using TangerineAuction.Shared;

namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionResponse
{
    
    /// <summary>
    /// Id аукциона
    /// </summary>
    public Guid AuctionId { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string AuctionName { get; set; }

    /// <summary>
    /// Дата создания аукциона
    /// </summary>
    public DateTime AuctionCreatedOn { get; set; }
    
    /// <summary>
    /// Название мандарина
    /// </summary>
    public string TangerineName { get; set; }
    
    /// <summary>
    /// Качество мандарина
    /// </summary>
    public TangerineQuality TangerineQuality { get; set; }
    
    /// <summary>
    /// Путь к картинке мандарина
    /// </summary>
    public string FilePath { get; set; }
    
    /// <summary>
    /// Последняя ставка
    /// </summary>
    public decimal LastBet { get; private set; }
    
    /// <summary>
    /// Id последнего пользователя сделавшего ставку
    /// </summary>
    public Guid LastUserBetId { get; private set; }
    
    public AuctionResponse(Guid auctionId, string auctionName, DateTime auctionCreatedOn, string tangerineName, 
        TangerineQuality tangerineQuality, string filePath, decimal lastBet, Guid lastUserBetId)
    {
        AuctionId = auctionId;
        AuctionName = auctionName;
        AuctionCreatedOn = auctionCreatedOn;
        TangerineName = tangerineName;
        TangerineQuality = tangerineQuality;
        FilePath = filePath;
        LastBet = lastBet;
        LastUserBetId = lastUserBetId;
    }
    
}