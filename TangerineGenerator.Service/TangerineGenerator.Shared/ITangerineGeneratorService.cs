using System.ServiceModel;
using TangerineAuction.Shared.Models;

namespace TangerineGenerator.Shared;

[ServiceContract]
public interface ITangerineGeneratorService
{
    
    /// <summary>
    /// Получить версию сервиса
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    ValueTask<VersionInfo> GetVersion();
    
    /// <summary>
    /// Удалить файл из хранилища
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [OperationContract]
    Task DeleteImage(string fileName, CancellationToken ct = default);
    
    /// <summary>
    /// Получить Url к картинкам мандаринок. Key - название файла, Value - Url.
    /// </summary>
    [OperationContract]
    Task<Dictionary<string, string>> GetPhotoUrls(List<string> tangerineFileNames, CancellationToken ct = default);
    
}