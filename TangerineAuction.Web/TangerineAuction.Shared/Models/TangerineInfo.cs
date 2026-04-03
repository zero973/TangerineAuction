using TangerineAuction.Shared.Enums;

namespace TangerineAuction.Shared.Models;

public class TangerineInfo
{
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Качество
    /// </summary>
    public TangerineQuality Quality { get; }
    
    /// <summary>
    /// Начальная цена
    /// </summary>
    public decimal StartPrice { get; }
    
    /// <summary>
    /// Цена выкупа
    /// </summary>
    public decimal BuyPrice { get; }
    
    /// <summary>
    /// Название файла-картинки в хранилище
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; }

    public TangerineInfo(
        string name,
        TangerineQuality quality,
        decimal startPrice,
        decimal buyPrice,
        string fileName,
        DateTime createdOn)
    {
        Name = name;
        Quality = quality;
        StartPrice = startPrice;
        BuyPrice = buyPrice;
        FileName = fileName;
        CreatedOn = createdOn;
    }

    public override string ToString() => $"{Quality} ({StartPrice}): {Name}. Файл: {FileName}";
    
}