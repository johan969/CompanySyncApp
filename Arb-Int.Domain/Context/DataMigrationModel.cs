

using Arb_Int.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DataMigration : IEntity<Guid>, ICreateTracker
{
    public Guid Id { get; set; }
    public string Note { get; set; } = null!;
    public DateTimeOffset? CreateDateUtc { get; set; }
    public bool CompletedWithSuccess { get; set; }
}

public sealed class DataMigrationConfig : IEntityTypeConfiguration<DataMigration>
{
    public void Configure(EntityTypeBuilder<DataMigration> builder)
    {
        builder.HasKey(dm => dm.Id);
        builder.Property(dm => dm.Note).HasMaxLength(255);
    }
}