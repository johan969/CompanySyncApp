using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    /// <summary>
    /// Konfigurerar geografiska origin-poster för inventory lots.
    /// </summary>
    public sealed class InventoryLotOriginConfig : IEntityTypeConfiguration<InventoryLotOrigin>
    {
        /// <summary>
        /// Konfigurerar constraints, index, concurrency och relationer för lot-origins.
        /// </summary>
        public void Configure(EntityTypeBuilder<InventoryLotOrigin> builder)
        {
            builder.ToTable(
                "InventoryLotOrigins",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_InventoryLotOrigins_Proportion",
                        "\"Proportion\" IS NULL OR (\"Proportion\" >= 0 AND \"Proportion\" <= 1)");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.Proportion)
                .HasPrecision(18, 8);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => new
            {
                x.InventoryLotId,
                x.GeographicLocationId,
                x.Role
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.InventoryLotId,
                x.Role
            })
            .IsUnique()
            .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasIndex(x => x.GeographicLocationId);

            builder.HasOne(x => x.InventoryLot)
                .WithMany(x => x.Origins)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany()
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
