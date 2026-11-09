using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplyAllocationConfig : IEntityTypeConfiguration<SupplyAllocation>
    {
        public void Configure(EntityTypeBuilder<SupplyAllocation> builder)
        {
            builder.ToTable(
                "SupplyAllocations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplyAllocations_Quantity",
                        """
                    "AllocatedQuantity" > 0
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplyAllocations_PriceCurrency",
                        """
                    ("UnitPurchasePrice" IS NULL AND "PurchasePriceCurrencyId" IS NULL)
                    OR ("UnitPurchasePrice" >= 0 AND "PurchasePriceCurrencyId" IS NOT NULL)
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplySolutionLineId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.AllocatedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.UnitPurchasePrice)
                .HasPrecision(20, 6);

            builder.Property(x => x.EstimatedAdditionalCost)
                .HasPrecision(20, 6);

            builder.Property(x => x.AvailabilityVerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.QualityVerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplySolutionLineId,
                x.AllocationSequence
            })
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.SupplierBusinessRelationshipId,
                x.SupplierMaterialOfferingId
            });

            builder.HasOne(x => x.SupplierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.SupplierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Jag tar med supplier-relationen i foreign keyn så att en allokering
             * aldrig kan kombinera ett offering med fel supplier.
             */
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

            /*
             * Jag binder tillgänglighetsposten till samma offering som allokeringen.
             */
            builder.HasOne(x => x.SupplierOfferingAvailability)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplierOfferingAvailabilityId,
                    x.SupplierMaterialOfferingId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierMaterialOfferingId
                })
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Jag binder även en vald lot till samma supplier-relation.
             */
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

            /*
            * En quote line som används som kommersiellt underlag måste komma
            * från samma supplier-relation som allokeringen.
            */
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

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PurchasePriceCurrency)
                .WithMany()
                .HasForeignKey(x => x.PurchasePriceCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
