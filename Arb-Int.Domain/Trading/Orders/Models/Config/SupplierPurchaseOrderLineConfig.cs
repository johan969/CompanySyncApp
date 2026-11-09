using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierPurchaseOrderLineConfig : IEntityTypeConfiguration<SupplierPurchaseOrderLine>
    {
        public void Configure(EntityTypeBuilder<SupplierPurchaseOrderLine> builder)
        {
            builder.ToTable(
                "SupplierPurchaseOrderLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierPurchaseOrderLines_Values",
                        """
                    "OrderedQuantity" > 0
                    AND "FulfilledQuantity" >= 0
                    AND "FulfilledQuantity" <= "OrderedQuantity"
                    AND "UnitPrice" >= 0
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierPurchaseOrderLines_Target",
                        """
                    num_nonnulls("SupplierMaterialOfferingId", "SupplierProcessCapabilityId",
                        "SupplySolutionStepId", "InventoryLotId") >= 1
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

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.OrderedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.FulfilledQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(20, 6);

            builder.Property(x => x.NetAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TaxPercent)
                .HasPrecision(9, 4);

            builder.Property(x => x.TaxAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.GrossAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.SupplierLineReference)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.SupplierPurchaseOrderId,
                x.LineNumber
            })
                .IsUnique();

            builder.HasOne(x => x.SupplierQuoteLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplierQuoteLineId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany()
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

            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplierProcessCapabilityId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplySolutionStep)
                .WithMany()
                .HasForeignKey(x => x.SupplySolutionStepId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.InventoryLotId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
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

            builder.HasMany(x => x.SalesAllocations)
                .WithOne(x => x.SupplierPurchaseOrderLine)
                .HasForeignKey(x => x.SupplierPurchaseOrderLineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
