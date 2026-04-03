using TangerineAuction.Core.Repository;

namespace TangerineAuction.Infrastructure.Data.Repositories;

internal class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    
    public async Task RunAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            state: 0,
            operation: async (_, _, ct) =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

                try
                {
                    await action(ct);
                    await dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return 0;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            },
            verifySucceeded: null,
            cancellationToken: ct);
    }

    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(
            state: 0,
            operation: async (_, _, ct) =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

                try
                {
                    var result = await action(ct);
                    await dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            },
            verifySucceeded: null,
            cancellationToken: ct);

        return result;
    }
    
}