using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierMaterialOfferingBiologicalSourceConfig : IEntityTypeConfiguration<SupplierMaterialOfferingBiologicalSource>
    {
        public void Configure(EntityTypeBuilder<SupplierMaterialOfferingBiologicalSource> builder)
        {
            builder.ToTable(
                "SupplierMaterialOfferingBiologicalSources",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierOfferingSources_Proportion",
                        """
                    "ProportionPercent" IS NULL
                    OR
                    (
                        "ProportionPercent" >= 0
                        AND "ProportionPercent" <= 100
                    )
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    SupplierOfferingBiologicalSourceRole.Unknown);

            builder.Property(x => x.ProportionPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.BusinessRelationshipBiologicalSourceClaimId
            })
            .IsUnique();

            builder.HasIndex(
                x => x.BusinessRelationshipBiologicalSourceClaimId);

            /*
             * Erbjudandet måste höra till samma BusinessRelationship
             * som denna länk anger.
             */
            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => new
                {
                    x.SupplierMaterialOfferingId,
                    x.BusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * Claim måste också höra till samma BusinessRelationship.
             *
             * Därmed kan ett erbjudande från leverantör A inte av misstag
             * länkas till en claim från leverantör B.
             */
            builder.HasOne(x => x.BiologicalSourceClaim)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BusinessRelationshipBiologicalSourceClaimId,
                    x.BusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}