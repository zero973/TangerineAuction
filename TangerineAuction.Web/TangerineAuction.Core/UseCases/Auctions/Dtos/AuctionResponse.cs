using TangerineAuction.Shared.Enums;

namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionResponse
{
    
    /// <summary>
    /// Id аукциона
    /// </summary>
    public Guid AuctionId { get; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string AuctionName { get; }
    
    /// <summary>
    /// Активность аукциона.
    /// true - принимает ставки. false - определён победитель
    /// </summary>
    public bool IsActual { get; }

    /// <summary>
    /// Дата создания аукциона
    /// </summary>
    public DateTime AuctionCreatedOn { get; }
    
    /// <summary>
    /// Название мандарина
    /// </summary>
    public string TangerineName { get; }
    
    /// <summary>
    /// Качество мандарина
    /// </summary>
    public TangerineQuality TangerineQuality { get; }
    
    /// <summary>
    /// Название файла-картинки в хранилище
    /// </summary>
    public string TangerineFileName { get; }
    
    /// <summary>
    /// Url картинки
    /// </summary>
    public string ImageUrl { get; set; }
    
    /// <summary>
    /// Последняя ставка
    /// </summary>
    public decimal LastBet { get; }
    
    /// <summary>
    /// Id последнего пользователя сделавшего ставку
    /// </summary>
    public Guid LastUserBetId { get; }
    
    public AuctionResponse(Guid auctionId, string auctionName, bool isActual, DateTime auctionCreatedOn, 
        string tangerineName, TangerineQuality tangerineQuality, string tangerineFileName, decimal lastBet, Guid lastUserBetId)
    {
        AuctionId = auctionId;
        AuctionName = auctionName;
        IsActual = isActual;
        AuctionCreatedOn = auctionCreatedOn;
        TangerineName = tangerineName;
        TangerineQuality = tangerineQuality;
        TangerineFileName = tangerineFileName;
        LastBet = lastBet;
        LastUserBetId = lastUserBetId;
    }
    
}