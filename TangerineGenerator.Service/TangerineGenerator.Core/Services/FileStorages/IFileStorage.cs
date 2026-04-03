namespace TangerineGenerator.Core.Services.FileStorages;

public interface IFileStorage
{

    /// <summary>
    /// Получить ссылку на файл
    /// </summary>
    /// <param name="fileName">Название файла</param>
    /// <returns></returns>
    Task<string> GetFileUrl(string fileName);
    
    /// <summary>
    /// Сохранить файл в хранилище
    /// </summary>
    /// <param name="stream">Стрим</param>
    /// <param name="fileName">Название файла</param>
    /// <param name="contentType">Тип файла</param>
    /// <param name="ct"></param>
    /// <returns>Название сохранённого файла</returns>
    Task<string> Save(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    
    /// <summary>
    /// Удалить файл из хранилища
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="ct"></param>
    Task Delete(string fileName, CancellationToken ct = default);
    
}