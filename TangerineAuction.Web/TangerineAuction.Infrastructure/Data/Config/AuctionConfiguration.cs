using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TangerineAuction.Core.Models;
using TangerineAuction.Infrastructure.Constants;

namespace TangerineAuction.Infrastructure.Data.Config;

internal class AuctionConfiguration() : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.Metadata.SetSchema(DbConstants.DataSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedOn)
            .IsRequired();
        
        builder.Property(x => x.IsActual)
            .IsRequired();
        
        builder.HasIndex(e => e.Name).HasFilter("is_actual = true");
        
        builder.HasIndex(e => e.CreatedOn).HasFilter("is_actual = true");

        builder.HasIndex(e => e.IsActual);
            
        builder.HasOne(x => x.Tangerine)
            .WithMany()
            .HasForeignKey(x => x.TangerineId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Bets)
            .WithOne(x => x.Auction)
            .HasForeignKey(x => x.AuctionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Bets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}