using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BuyerDemandScopeConfig : IEntityTypeConfiguration<BuyerDemandScope>
    {
        public void Configure(EntityTypeBuilder<BuyerDemandScope> builder)
        {
            builder.ToTable(
                "BuyerDemandScopes",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerDemandScopes_Target",
                        """
                    num_nonnulls("BiologicalSourceClaimId", "MaterialId", "MaterialTypeId", "DeclaredFormat",
                        "IntendedUseCharacteristicId", "IntendedUseText") >= 1
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerDemandScopes_Quantity",
                        """
                    ("TypicalQuantity" IS NULL AND "TypicalQuantityUnitId" IS NULL)
                    OR ("TypicalQuantity" > 0 AND "TypicalQuantityUnitId" IS NOT NULL)
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.DeclaredFormat)
                .HasMaxLength(255);
            builder.Property(x => x.IntendedUseText)
                .HasMaxLength(2048);
            builder.Property(x => x.TypicalQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.RecurrenceDescription)
                .HasMaxLength(1000);
            builder.HasIndex(x => new
            {
                x.BuyerDemandId,
                x.SortOrder
            });

            /*
             * Jag binder claimen till samma buyer-relation som demanden.
             */
            builder.HasOne(x => x.BiologicalSourceClaim)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BiologicalSourceClaimId,
                    x.BusinessRelationshipId
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

            builder.HasOne(x => x.MaterialType)
                .WithMany()
                .HasForeignKey(x => x.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IntendedUseCharacteristic)
                .WithMany()
                .HasForeignKey(x => x.IntendedUseCharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TypicalQuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.TypicalQuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
