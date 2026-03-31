namespace TangerineAuction.Shared;

public class TangerineInfo
{
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Качество
    /// </summary>
    public TangerineQuality Quality { get; set; }
    
    /// <summary>
    /// Начальная цена
    /// </summary>
    public decimal StartPrice { get; set; }
    
    /// <summary>
    /// Путь к картинке
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; set; }

    public TangerineInfo(string name, TangerineQuality quality, decimal startPrice, string filePath, DateTime createdOn)
    {
        Name = name;
        Quality = quality;
        StartPrice = startPrice;
        FilePath = filePath;
        CreatedOn = createdOn;
    }

    public override string ToString() => $"{Quality} ({StartPrice}): {Name}. Файл: {FilePath}";
    
}