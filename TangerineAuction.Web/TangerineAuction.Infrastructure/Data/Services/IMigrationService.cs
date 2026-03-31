using Ardalis.Result;

namespace TangerineAuction.Infrastructure.Data.Services;

/// <summary>
/// Runs pending database migrations.
/// </summary>
public interface IMigrationService
{
    /// <summary>
    /// Detects and applies any pending migrations; logs start, details and completion.
    /// </summary>
    Task<Result> Migrate(CancellationToken ct = default);
}