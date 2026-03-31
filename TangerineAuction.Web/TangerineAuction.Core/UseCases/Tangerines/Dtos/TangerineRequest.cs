using TangerineAuction.Shared;

namespace TangerineAuction.Core.UseCases.Tangerines.Dtos;

public class TangerineRequest
{
    /// <summary>
    /// Название
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Качество
    /// </summary>
    public required TangerineQuality Quality { get; set; }
    
    /// <summary>
    /// Начальная цена
    /// </summary>
    public required decimal StartPrice { get; set; }
    
    /// <summary>
    /// Путь к картинке
    /// </summary>
    public required string FilePath { get; set; }
}