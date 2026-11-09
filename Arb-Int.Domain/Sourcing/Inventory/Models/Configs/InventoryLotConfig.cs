using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class InventoryLotConfig : IEntityTypeConfiguration<InventoryLot>
    {
        public void Configure(EntityTypeBuilder<InventoryLot> builder)
        {
            builder.ToTable(
                "InventoryLots",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_InventoryLots_Quantities",
                        """
                    "TotalQuantity" >= 0
                    AND "AvailableQuantity" >= 0
                    AND "ReservedQuantity" >= 0
                    AND "AvailableQuantity" + "ReservedQuantity" <= "TotalQuantity"
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplierBusinessRelationshipId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.LotNumber)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.SupplierLotNumber)
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.TotalQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.AvailableQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.ReservedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.WarehouseReference)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.LotNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.SupplierBusinessRelationshipId,
                x.SupplierLotNumber
            });

            builder.HasIndex(x => new
            {
                x.Status,
                x.BestBeforeUtc
            });

            builder.HasOne(x => x.SupplierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.SupplierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.InventoryLots)
                .HasForeignKey(x => new
                {
                    x.SupplierMaterialOfferingId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CurrentLocation)
                .WithMany()
                .HasForeignKey(x => x.CurrentLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Origins)
                .WithOne(x => x.InventoryLot)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
