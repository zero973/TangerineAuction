using TangerineAuction.Core.Models;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionFullResponse
{
    
    /// <summary>
    /// Id аукциона
    /// </summary>
    public Guid AuctionId { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Активность аукциона.
    /// true - принимает ставки. false - определён победитель
    /// </summary>
    public bool IsActual { get; set; }

    /// <summary>
    /// Мандарин
    /// </summary>
    public TangerineResponse Tangerine { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; set; }
    
    public List<BetResponse> Bets { get; set; }

    public AuctionFullResponse(Auction auction, TangerineResponse tangerine, List<BetResponse> bets)
    {
        AuctionId = auction.Id;
        Name = auction.Name;
        IsActual = auction.IsActual;
        Tangerine = tangerine;
        CreatedOn = auction.CreatedOn;
        Bets = bets;
    }
    
}