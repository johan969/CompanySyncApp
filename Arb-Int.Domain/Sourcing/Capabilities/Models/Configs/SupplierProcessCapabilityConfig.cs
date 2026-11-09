using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierProcessCapabilityConfig : IEntityTypeConfiguration<SupplierProcessCapability>
    {
        public void Configure(EntityTypeBuilder<SupplierProcessCapability> builder)
        {
            builder.ToTable(
                "SupplierProcessCapabilities",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierProcessCapabilities_ObservedDates",
                        """
                    "LastObservedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierProcessCapabilities_VerifiedDate",
                        """
                    "LastVerifiedUtc" IS NULL
                    OR "LastVerifiedUtc" >= "FirstObservedUtc"
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierProcessCapabilities_Confidence",
                        """
                    "MappingConfidence" IS NULL
                    OR
                    (
                        "MappingConfidence" >= 0
                        AND "MappingConfidence" <= 1
                    )
                    """);

                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierProcessCapabilities_ConfirmedMapping",
                        """
                    "MappingStatus" <> 'Confirmed'
                    OR "ProcessTypeId" IS NOT NULL
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BusinessRelationshipId
            });

            builder.Property(x => x.CapabilityKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.DeclaredProcessName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.NormalizedDeclaredProcessName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.MappingStatus)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SourcingMappingStatus.Unmapped);

            builder.Property(x => x.MappingConfidence)
                .HasPrecision(5, 4);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SupplierProcessCapabilityStatus.Observed)
                .HasSentinel(SupplierProcessCapabilityStatus.Unknown);

            builder.Property(x => x.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(64)
                .HasDefaultValue(
                    SourcingVerificationStatus.Unverified);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.CapabilityKey
            })
            .IsUnique();

            builder.HasIndex(x => x.ProcessTypeId);

            builder.HasIndex(x => x.InputMaterialTypeId);

            builder.HasIndex(x => x.OutputMaterialTypeId);

            builder.HasIndex(x => new
            {
                x.Status,
                x.VerificationStatus
            });

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProcessType)
                .WithMany()
                .HasForeignKey(x => x.ProcessTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InputMaterialType)
                .WithMany()
                .HasForeignKey(x => x.InputMaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OutputMaterialType)
                .WithMany()
                .HasForeignKey(x => x.OutputMaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}