using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TangerineAuction.Core.Models;

namespace TangerineAuction.Infrastructure.Data;

/// <summary>
/// Application EF Core <see cref="DbContext"/> containing domain DbSets and model configuration.
/// </summary>
internal class AppDbContext : DbContext
{
    
    public DbSet<Tangerine> Tangerines { get; set; }

    public DbSet<Auction> Auctions { get; set; }

    public DbSet<Bet> Bets { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // dotnet tool update --global dotnet-ef
        // dotnet ef migrations add Initialize -p TangerineAuction.Infrastructure -s TangerineAuction.Server --context AppDbContext
        // dotnet ef database update -p TangerineAuction.Infrastructure -s TangerineAuction.Server
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }

    /// <summary>
    /// Returns the raw database connection used by this context.
    /// </summary>
    public IDbConnection GetConnection() => Database.GetDbConnection();
    
}