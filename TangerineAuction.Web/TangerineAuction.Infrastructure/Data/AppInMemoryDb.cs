using Microsoft.EntityFrameworkCore;

namespace TangerineAuction.Infrastructure.Data;

internal class AppInMemoryDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("InMemoryDb");
    }
}