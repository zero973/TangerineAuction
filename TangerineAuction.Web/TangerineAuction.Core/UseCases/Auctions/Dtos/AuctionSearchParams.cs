using TangerineAuction.Shared.Enums;

namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionSearchParams
{
    
    public int Skip { get; set; }
    
    public int Take { get; set; }

    /// <summary>
    /// Название аукциона
    /// </summary>
    public string? AuctionName { get; set; }

    /// <summary>
    /// Название аукциона
    /// </summary>
    public string? TangerineName { get; set; }

    /// <summary>
    /// Качество мандарина
    /// </summary>
    public TangerineQuality? TangerineQuality { get; set; }

    /// <summary>
    /// Включить отображение завершённых аукционов
    /// </summary>
    public bool ShowFinishedAuctions { get; set; }

    /// <summary>
    /// Отображать только те аукционы, где текущий пользователь победитель
    /// </summary>
    public bool IsCurrentUserWinner { get; set; }
    
}