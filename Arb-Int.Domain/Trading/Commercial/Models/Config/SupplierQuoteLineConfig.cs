using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierQuoteLineConfig : IEntityTypeConfiguration<SupplierQuoteLine>
    {
        public void Configure(EntityTypeBuilder<SupplierQuoteLine> builder)
        {
            builder.ToTable(
                "SupplierQuoteLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierQuoteLines_Values",
                        """
                    "OfferedQuantity" > 0
                    AND "UnitPrice" >= 0
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierQuoteLines_Target",
                        """
                    num_nonnulls("SupplierMaterialOfferingId", "SupplierProcessCapabilityId",
                        "SupplySolutionStepId", "InventoryLotId") >= 1
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierQuoteLines_Availability",
                        """
                        "AvailableToUtc" IS NULL
                        OR "AvailableFromUtc" IS NULL
                        OR "AvailableToUtc" >= "AvailableFromUtc"
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

            builder.Property(x => x.OfferedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(20, 6);

            builder.Property(x => x.FixedFee)
                .HasPrecision(20, 6);

            builder.Property(x => x.MinimumOrderQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.NamedPlace)
                .HasMaxLength(512);

            builder.Property(x => x.PriceBasis)
                .HasMaxLength(1000);

            builder.Property(x => x.SupplierLineReference)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierQuoteRevisionId,
                x.LineNumber
            })
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.SupplierProcessCapabilityId,
                x.SupplySolutionStepId
            });

            builder.HasOne(x => x.BuyerRfqLine)
                .WithMany()
                .HasForeignKey(x => x.BuyerRfqLineId)
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

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
