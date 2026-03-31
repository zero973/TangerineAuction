using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;
using System.Reflection;
using System.Text;
using TangerineAuction.Infrastructure.Constants;

namespace TangerineAuction.Infrastructure.Data.Repositories;

/// <summary>
/// Customizes the EF Core migrations history storage for PostgreSQL.
/// </summary>
/// <remarks>
/// Stores migration id, product version and execution timestamp in a custom table/columns.
/// </remarks>
#pragma warning disable EF1001
internal class CustomHistoryRepository(HistoryRepositoryDependencies dependencies)
    : NpgsqlHistoryRepository(dependencies)
{
    
    /// <summary>
    /// Name of the execution date column in the history table.
    /// </summary>
    private const string ExecutionDateColumnName = "execution_date";

    /// <summary>
    /// Configures the schema for the migrations history table (names, keys and columns).
    /// </summary>
    /// <param name="builder">Entity type builder for the history row.</param>
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> builder)
    {
        builder.ToTable(DbConstants.MigrationsHistoryTableName, DbConstants.PublicSchema);

        builder.HasKey(h => h.MigrationId);

        builder.Property(h => h.MigrationId)
            .HasMaxLength(150)
            .HasColumnName("id");

        builder.Property(h => h.ProductVersion)
            .HasColumnName("product_version")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property<DateTime>(ExecutionDateColumnName).IsRequired();
    }
    
    /// <summary>
    /// Generates a custom SQL insert statement for the migrations history row,
    /// writing the migration id, product version and current timestamp.
    /// </summary>
    /// <param name="row">The history row to insert.</param>
    /// <returns>SQL script that inserts the history row.</returns>
    public override string GetInsertScript(HistoryRow row)
    {
        var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

        var version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        return new StringBuilder().Append("INSERT INTO ")
            .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
            .Append(" (")
                .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
                .Append(", ")
                .Append(SqlGenerationHelper.DelimitIdentifier(ProductVersionColumnName))
                .Append(", ")
                .Append(SqlGenerationHelper.DelimitIdentifier(ExecutionDateColumnName))
            .AppendLine(")")
            .Append("VALUES (")
                .Append(stringTypeMapping.GenerateSqlLiteral(row.MigrationId))
                .Append(", ")
                .Append(stringTypeMapping.GenerateSqlLiteral(version))
                .Append(", ")
                .Append("now()")
            .Append(')')
            .AppendLine(SqlGenerationHelper.StatementTerminator)
            .ToString();
    }

}