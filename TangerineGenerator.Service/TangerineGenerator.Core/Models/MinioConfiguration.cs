namespace TangerineGenerator.Core.Models;

public class MinioConfiguration
{
    
    public string Endpoint { get; init; }
    
    public string AccessKey { get; init; }
    
    public string SecretKey { get; init; }
    
    public string BucketName { get; init; }
    
    public int ExpireTime { get; init; }
    
}