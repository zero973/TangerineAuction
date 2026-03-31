namespace TangerineGenerator.Core.Models;

public class TangerineGeneratorOptions
{
    
    /// <summary>
    /// Папка, в которую будут сохраняться картинки
    /// </summary>
    public string PicturesOutputFolder { get; init; } = string.Empty;
    
    /// <summary>
    /// Задержка между генерацией мандаринок (в минутах)
    /// </summary>
    public int Delay { get; init; } = 5;
    
}