using TangerineAuction.Core.Models;
using TangerineAuction.Shared.Enums;

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
    /// Цена выкупа
    /// </summary>
    public decimal BuyPrice { get; set; }
    
    /// <summary>
    /// Название файла-картинки в хранилище
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// Url картинки
    /// </summary>
    public string ImageUrl { get; set; }

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
        BuyPrice = tangerine.BuyPrice;
        FileName = tangerine.FileName;
        CreatedOn = tangerine.CreatedOn;
    }
    
}