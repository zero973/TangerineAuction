using Ardalis.Result;
using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
    /// <summary>
    /// Check the database availability 
    /// </summary>
    /// <returns></returns>
    Task<Result> CanConnect(CancellationToken ct = default);
}