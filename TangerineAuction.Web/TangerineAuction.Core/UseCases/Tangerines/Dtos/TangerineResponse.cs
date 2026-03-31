using TangerineAuction.Core.Models;
using TangerineAuction.Shared;

namespace TangerineAuction.Core.UseCases.Tangerines.Dtos;

public class TangerineResponse
{
    
    public Guid Id { get; set; }
    
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

    public TangerineResponse(Tangerine tangerine)
    {
        Id = tangerine.Id;
        Name = tangerine.Name;
        Quality = tangerine.Quality;
        StartPrice = tangerine.StartPrice;
        FilePath = tangerine.FilePath;
        CreatedOn = tangerine.CreatedOn;
    }
    
}