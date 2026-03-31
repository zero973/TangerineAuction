using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;

namespace TangerineAuction.Infrastructure.Data.Repositories;

internal class EfRepository<T>(AppDbContext context) : RepositoryBase<T>(context), IRepository<T>
    where T : class, IAggregateRoot
{
    public override async Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await ApplySpecification(specification).ExecuteDeleteAsync(ct);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var a = DbContext.ChangeTracker.Entries().ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        a = DbContext.ChangeTracker.Entries().ToList();
        return result;
    }
}