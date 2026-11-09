using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipBiologicalSourceClaimConfig : IEntityTypeConfiguration<BusinessRelationshipBiologicalSourceClaim>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipBiologicalSourceClaim> builder)
        {
            builder.ToTable(
                "BusinessRelationshipBiologicalSourceClaims",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_BusinessRelationshipSourceClaims_ObservedDates",
                        """
                    "LastObservedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_BusinessRelationshipSourceClaims_VerifiedDate",
                        """
                    "LastVerifiedUtc" IS NULL
                    OR "LastVerifiedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_BusinessRelationshipSourceClaims_Confidence",
                        """
                    "MappingConfidence" IS NULL
                    OR
                    (
                        "MappingConfidence" >= 0
                        AND "MappingConfidence" <= 1
                    )
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_BusinessRelationshipSourceClaims_ConfirmedMapping",
                        """
                    "MappingStatus" <> 'Confirmed'
                    OR "BiologicalSourceId" IS NOT NULL
                    """);
                });

            builder.HasKey(x => x.Id);

            /*
             * Denna alternate key används senare av sourcingmodellen.
             * Den gör det möjligt att säkerställa att ett erbjudande och
             * dess biologiska claim tillhör samma BusinessRelationship.
             */
            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BusinessRelationshipId
            });

            builder.Property(x => x.DeclaredName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.NormalizedDeclaredName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.RelationshipKind)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    BiologicalSourceRelationshipKind.Unknown);

            builder.Property(x => x.MappingStatus)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(ClaimMappingStatus.Unmapped);

            builder.Property(x => x.MappingConfidence)
                .HasPrecision(5, 4);

            builder.Property(x => x.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(64)
                .HasDefaultValue(
                    ClaimVerificationStatus.Unverified);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            /*
             * En normaliserad claim.
             *
             * Samma affärsrelation kan ha både Cultivates och Supplies
             * för samma BiologicalSource eftersom RelationshipKind ingår.
             */
            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.RelationshipKind,
                x.BiologicalSourceId
            })
            .IsUnique()
            .HasFilter("\"BiologicalSourceId\" IS NOT NULL");

            /*
             * En ännu omappad claim.
             */
            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.RelationshipKind,
                x.NormalizedDeclaredName
            })
            .IsUnique()
            .HasFilter("\"BiologicalSourceId\" IS NULL");

            builder.HasIndex(x => x.BiologicalSourceId);

            builder.HasIndex(x => new
            {
                x.VerificationStatus,
                x.LastObservedUtc
            });

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany(x => x.BiologicalSourceClaims)
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * Ingen reverse-navigation på BiologicalSource behövs.
             * Articles-modulen behöver därmed inte referera tillbaka
             * till CRM.
             */
            builder.HasOne(x => x.BiologicalSource)
                .WithMany()
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}