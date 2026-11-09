using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierOfferingAvailabilityConfig : IEntityTypeConfiguration<SupplierOfferingAvailability>
    {
        public void Configure(EntityTypeBuilder<SupplierOfferingAvailability> builder)
        {
            builder.ToTable(
                "SupplierOfferingAvailabilities",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierOfferingAvailabilities_Quantities",
                        """
                    "AvailableQuantity" >= 0
                    AND "ReservedQuantity" >= 0
                    AND "ReservedQuantity" <= "AvailableQuantity"
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierOfferingAvailabilities_Window",
                        """
                    "AvailableToUtc" IS NULL
                    OR "AvailableToUtc" >= "AvailableFromUtc"
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierOfferingAvailabilities_MinimumOrder",
                        """
                    ("MinimumOrderQuantity" IS NULL AND "MinimumOrderUnitId" IS NULL)
                    OR ("MinimumOrderQuantity" > 0 AND "MinimumOrderUnitId" IS NOT NULL)
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplierMaterialOfferingId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.AvailableQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.ReservedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.MinimumOrderQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.SourceReference)
                .HasMaxLength(1000);

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.Status,
                x.AvailableFromUtc
            });

            builder.HasIndex(x => x.InventoryLotId);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Availabilities)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany(x => x.Availabilities)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MinimumOrderUnit)
                .WithMany()
                .HasForeignKey(x => x.MinimumOrderUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AvailabilityLocation)
                .WithMany()
                .HasForeignKey(x => x.AvailabilityLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
