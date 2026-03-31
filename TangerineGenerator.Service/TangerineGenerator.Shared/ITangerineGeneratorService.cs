using System.ServiceModel;
using TangerineAuction.Shared;

namespace TangerineGenerator.Shared;

[ServiceContract]
public interface ITangerineGeneratorService
{
    
    [OperationContract]
    ValueTask<VersionInfo> GetVersion();
    
    [OperationContract]
    ValueTask DeleteImage(string filePath);
    
}