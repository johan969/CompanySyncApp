using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqLineConfig : IEntityTypeConfiguration<BuyerRfqLine>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqLine> builder)
        {
            builder.ToTable(
                "BuyerRfqLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqLines_RequiredQuantity",
                        """
                    "RequiredQuantity" > 0
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerRfqLines_PoolingRules",
                        """
                    NOT ("AllowMultipleSuppliers" AND "RequireSingleSupplier")
                    AND NOT ("AllowMultipleOrigins" AND "RequireSingleOrigin")
                    AND NOT ("AllowMultipleLots" AND "RequireSingleLot")
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerRfqLines_Tolerances",
                        """
                    ("LowerQuantityTolerancePercent" IS NULL
                        OR ("LowerQuantityTolerancePercent" >= 0 AND "LowerQuantityTolerancePercent" <= 100))
                    AND ("UpperQuantityTolerancePercent" IS NULL
                        OR ("UpperQuantityTolerancePercent" >= 0 AND "UpperQuantityTolerancePercent" <= 100))
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerRfqLines_MaximumSupplierCount",
                        """
                    "MaximumSupplierCount" IS NULL
                    OR "MaximumSupplierCount" > 0
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerRfqRevisionId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.BuyerLineReference)
                .HasMaxLength(255);

            builder.Property(x => x.BuyerOriginalText)
                .HasMaxLength(8000);

            builder.Property(x => x.RequiredQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.LowerQuantityTolerancePercent)
                .HasPrecision(9, 6);

            builder.Property(x => x.UpperQuantityTolerancePercent)
                .HasPrecision(9, 6);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BuyerRfqRevisionId,
                x.LineNumber
            })
                .IsUnique();

            builder.HasIndex(x => x.MaterialId);

            builder.HasIndex(x => x.MaterialTypeId);

            builder.HasOne(x => x.BuyerDemandScope)
                .WithMany()
                .HasForeignKey(x => x.BuyerDemandScopeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MaterialType)
                .WithMany()
                .HasForeignKey(x => x.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IntendedUseCharacteristic)
                .WithMany()
                .HasForeignKey(x => x.IntendedUseCharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.MaterialOptions)
                .WithOne(x => x.BuyerRfqLine)
                .HasForeignKey(x => x.BuyerRfqLineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
