using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.UseCases.Bets.Dtos;

public class BetResponse
{
    
    public Guid Id { get; set; }
    
    // <summary>
    /// Сумма ставки
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Кто сделал ставку
    /// </summary>
    public Guid CreatedBy { get; set; }

    public BetResponse(Bet bet)
    {
        Id = bet.Id;
        Price = bet.Price;
        CreatedOn = bet.CreatedOn;
        CreatedBy = bet.CreatedBy;
    }
    
}