using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TangerineAuction.Core.Models;
using TangerineAuction.Infrastructure.Constants;

namespace TangerineAuction.Infrastructure.Data.Config;

internal class TangerineConfiguration : IEntityTypeConfiguration<Tangerine>
{
    public void Configure(EntityTypeBuilder<Tangerine> builder)
    {
        builder.Metadata.SetSchema(DbConstants.DataSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Quality)
            .IsRequired();

        builder.Property(x => x.StartPrice)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .HasColumnType("text")
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.HasIndex(e => e.Quality);
    }
}