using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TangerineAuction.Core.Models;
using TangerineAuction.Infrastructure.Constants;

namespace TangerineAuction.Infrastructure.Data.Config;

internal class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.Metadata.SetSchema(DbConstants.DataSchema);

        builder.HasKey(nameof(Bet.Id));

        builder.Property(r => r.Price)
            .IsRequired();

        builder.Property(r => r.CreatedOn)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.HasIndex(x => x.AuctionId);
    }
}