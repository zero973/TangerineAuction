namespace TangerineAuction.Core.Services;

public interface ISystemUserService
{
    
    Guid UserId { get; }
    
    Task EnsureInitialized(CancellationToken ct = default);
    
}