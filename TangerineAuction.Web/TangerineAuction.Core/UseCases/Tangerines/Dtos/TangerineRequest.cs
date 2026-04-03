using TangerineAuction.Shared.Enums;
using TangerineAuction.Shared.Models;

namespace TangerineAuction.Core.UseCases.Tangerines.Dtos;

public class TangerineRequest
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
    /// Название файла-картинки
    /// </summary>
    public string FileName { get; }

    public TangerineRequest(TangerineInfo tangerine)
    {
        Name = tangerine.Name;
        Quality = tangerine.Quality;
        StartPrice = tangerine.StartPrice;
        BuyPrice = tangerine.BuyPrice;
        FileName = tangerine.FileName;
    }
    
}