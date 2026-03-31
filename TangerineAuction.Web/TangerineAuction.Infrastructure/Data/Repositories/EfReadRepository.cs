using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using TangerineAuction.Core.Repository;

namespace TangerineAuction.Infrastructure.Data.Repositories;

internal class EfReadRepository<T>(AppDbContext context) : RepositoryBase<T>(context), IReadRepository<T>
    where T : class
{
    public async Task<Result> CanConnect(CancellationToken ct = default)
    {
        try
        {
            var result = await context.Database.CanConnectAsync(ct);
            return result ? Result.Success() : Result.Error("Database connection failed");
        }
        catch (OperationCanceledException e)
        {
            return Result.Error("Operation canceled");
        }
    }
}