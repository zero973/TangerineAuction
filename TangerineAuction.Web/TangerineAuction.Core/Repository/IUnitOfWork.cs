namespace TangerineAuction.Core.Repository;

public interface IUnitOfWork
{
    
    Task RunAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
    
    Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
    
}