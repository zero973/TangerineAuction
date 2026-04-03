using System.Runtime.Serialization;

namespace TangerineAuction.Shared.Models;

[DataContract]
public class VersionInfo
{
    
    [DataMember(Order = 1)]
    public required string AppVersion { get; set; }

    [DataMember(Order = 2)]
    public DateTime BuildDate { get; set; }

    public override string ToString()
    {
        return $"{AppVersion} {BuildDate}";
    }
    
}