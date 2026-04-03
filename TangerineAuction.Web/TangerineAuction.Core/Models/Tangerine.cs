using TangerineAuction.Shared.Enums;

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
    /// Цена выкупа
    /// </summary>
    public decimal BuyPrice { get; private set; }
    
    /// <summary>
    /// Название файла-картинки в хранилище
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
#pragma warning disable CS8618 // Required by Entity Framework
    // ReSharper disable once UnusedMember.Local
    private Tangerine() {}

    private Tangerine(string name, TangerineQuality quality, decimal startPrice, decimal buyPrice, string fileName)
    {
        Name = name;
        Quality = quality;
        StartPrice = startPrice;
        BuyPrice = buyPrice;
        FileName = fileName;
        CreatedOn = DateTime.UtcNow;
    }
    
    public static Tangerine Create(string name, TangerineQuality quality, decimal startPrice, decimal buyPrice, string fileName)
    {
        return new Tangerine(name, quality, startPrice, buyPrice, fileName);
    }
    
}