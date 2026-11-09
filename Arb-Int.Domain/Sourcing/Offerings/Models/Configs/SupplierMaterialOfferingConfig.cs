using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierMaterialOfferingConfig : IEntityTypeConfiguration<SupplierMaterialOffering>
    {
        public void Configure(EntityTypeBuilder<SupplierMaterialOffering> builder)
        {
            builder.ToTable(
                "SupplierMaterialOfferings",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierMaterialOfferings_ObservedDates",
                        """
                    "LastObservedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierMaterialOfferings_VerifiedDate",
                        """
                    "LastVerifiedUtc" IS NULL
                    OR "LastVerifiedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierMaterialOfferings_MappingConfidence",
                        """
                    "MappingConfidence" IS NULL
                    OR
                    (
                        "MappingConfidence" >= 0
                        AND "MappingConfidence" <= 1
                    )
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierMaterialOfferings_ConfirmedMaterial",
                        """
                    "MappingStatus" <> 'Confirmed'
                    OR "MappedMaterialId" IS NOT NULL
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            /*
             * Används av SupplierMaterialOfferingBiologicalSource för
             * att säkerställa att erbjudandet och claimen hör till samma
             * BusinessRelationship.
             */
            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BusinessRelationshipId
            });

            builder.Property(x => x.OfferingKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.SupplierSku)
                .HasMaxLength(255);

            builder.Property(x => x.NormalizedSupplierSku)
                .HasMaxLength(255);

            builder.Property(x => x.DeclaredFormat)
                .HasMaxLength(512);

            builder.Property(x => x.MappingStatus)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SourcingMappingStatus.Unmapped);

            builder.Property(x => x.MappingConfidence)
                .HasPrecision(5, 4);

            builder.Property(x => x.MappingNotes)
                .HasMaxLength(4096);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SupplierOfferingStatus.Observed)
                .HasSentinel(SupplierOfferingStatus.Unknown);

            /*
             * Den kanoniska importnyckeln inom leverantörsrelationen.
             */
            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.OfferingKey
            })
            .IsUnique();

            /*
             * Leverantörens SKU behöver endast vara unik inom den
             * specifika supplier-relationen och när ett SKU finns.
             */
            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.NormalizedSupplierSku
            })
            .IsUnique()
            .HasFilter("\"NormalizedSupplierSku\" IS NOT NULL");

            builder.HasIndex(x => x.MappedMaterialId);

            builder.HasIndex(x => x.OfferedMaterialTypeId);

            builder.HasIndex(x => new
            {
                x.Status,
                x.LastObservedUtc
            });

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OfferedMaterialType)
                .WithMany()
                .HasForeignKey(x => x.OfferedMaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MappedMaterial)
                .WithMany()
                .HasForeignKey(x => x.MappedMaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}