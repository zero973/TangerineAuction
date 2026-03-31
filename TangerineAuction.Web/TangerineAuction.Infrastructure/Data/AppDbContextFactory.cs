using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TangerineAuction.Infrastructure.Constants;
using TangerineAuction.Infrastructure.Data.Repositories;

namespace TangerineAuction.Infrastructure.Data;

/// <summary>
/// Design-time factory to create <see cref="AppDbContext"/> for EF Core tools.
/// </summary>
internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(config.GetConnectionString("Postgres"), opts =>
        {
            opts.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            opts.MigrationsHistoryTable(DbConstants.MigrationsHistoryTableName, DbConstants.PublicSchema);
        });

        optionsBuilder.ReplaceService<IHistoryRepository, CustomHistoryRepository>();
        optionsBuilder.UseSnakeCaseNamingConvention();

        return new AppDbContext(optionsBuilder.Options);
    }
}