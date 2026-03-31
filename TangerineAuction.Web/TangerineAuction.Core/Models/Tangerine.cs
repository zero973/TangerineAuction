using TangerineAuction.Shared;

namespace TangerineAuction.Core.Models;

/// <summary>
/// Мандарин
/// </summary>
public class Tangerine : BaseEntity, IAggregateRoot
{
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// Качество
    /// </summary>
    public TangerineQuality Quality { get; private set; }
    
    /// <summary>
    /// Начальная цена
    /// </summary>
    public decimal StartPrice { get; private set; }
    
    /// <summary>
    /// Путь к картинке
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
#pragma warning disable CS8618 // Required by Entity Framework
    // ReSharper disable once UnusedMember.Local
    private Tangerine() {}

    private Tangerine(string name, TangerineQuality quality, decimal startPrice, string filePath)
    {
        Name = name;
        Quality = quality;
        StartPrice = startPrice;
        FilePath = filePath;
        CreatedOn = DateTime.UtcNow;
    }
    
    public static Tangerine Create(string name, TangerineQuality quality, decimal startPrice, string filePath)
    {
        return new Tangerine(name, quality, startPrice, filePath);
    }
    
}