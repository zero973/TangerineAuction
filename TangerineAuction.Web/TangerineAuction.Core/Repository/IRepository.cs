using Ardalis.Specification;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Core.Repository;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{ }