namespace TangerineAuction.Core.Models;

/// <summary>
/// Аукцион
/// </summary>
public class Auction : BaseEntity, IAggregateRoot
{
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Id мандарина
    /// </summary>
    public Guid TangerineId { get; private set; }
    
    /// <summary>
    /// Мандарин
    /// </summary>
    public Tangerine Tangerine { get; private set; } = null!;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; private set; }

    /// <summary>
    /// Активность аукциона.
    /// true - принимает ставки. false - определён победитель
    /// </summary>
    public bool IsActual { get; private set; } = true;
    
    private readonly List<Bet> _bets = [];

    /// <summary>
    /// Ставки
    /// </summary>
    public IReadOnlyCollection<Bet> Bets => _bets.AsReadOnly();

#pragma warning disable CS8618 // Required by Entity Framework
    // ReSharper disable once UnusedMember.Local
    private Auction() {}

    private Auction(string name, Guid tangerineId)
    {
        Name = name;
        TangerineId = tangerineId;
        CreatedOn = DateTime.UtcNow;
    }
    
    public static Auction Create(string name, Guid tangerineId)
    {
        return new Auction(name, tangerineId);
    }

    public void Disable()
    {
        IsActual = false;
    }

    public bool AddBet(Bet bet)
    {
        if (_bets.Contains(bet))
            return false;
        
        _bets.Add(bet);
        return true;
    }
    
}