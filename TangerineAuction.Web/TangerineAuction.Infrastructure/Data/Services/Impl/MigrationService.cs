using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TangerineAuction.Infrastructure.Data.Services.Impl;

/// <summary>
/// Applies pending EF Core migrations and logs progress.
/// </summary>
/// <param name="context">Application database context used to run migrations.</param>
/// <param name="logger">Logger for migration-related messages.</param>
internal class MigrationService(AppDbContext context, ILogger<MigrationService> logger) : IMigrationService
{
    public async Task<Result> Migrate(CancellationToken ct = default)
    {
        try
        {
            var migrations = (await context.Database.GetPendingMigrationsAsync(ct)).ToList();

            if (!migrations.Any())
                return Result.Success();

            logger.LogInformation("Unfulfilled migrations detected:"
                                  + Environment.NewLine + string.Join(Environment.NewLine, migrations));

            logger.LogInformation("Launching migrations...");
            await context.Database.MigrateAsync(ct);
            logger.LogInformation("Migrations completed");

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when performing migrations");
            throw;
        }
    }
}