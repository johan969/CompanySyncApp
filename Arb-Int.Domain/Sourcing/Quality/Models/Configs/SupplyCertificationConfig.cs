using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplyCertificationConfig : IEntityTypeConfiguration<SupplyCertification>
    {
        public void Configure(EntityTypeBuilder<SupplyCertification> builder)
        {
            builder.ToTable(
                "SupplyCertifications",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplyCertifications_Subject",
                        """
                    num_nonnulls("CertifiedBusinessRelationshipId", "SupplierMaterialOfferingId",
                        "SupplierProcessCapabilityId", "InventoryLotId", "ShipmentId") = 1
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplyCertifications_Validity",
                        """
                    "ValidToUtc" IS NULL
                    OR "ValidFromUtc" IS NULL
                    OR "ValidToUtc" >= "ValidFromUtc"
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.Scope)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.CertificateNumber)
                .HasMaxLength(255);
            builder.Property(x => x.CertificateHolderName)
                .HasMaxLength(255);
            builder.Property(x => x.IssuerName)
                .HasMaxLength(255);
            builder.Property(x => x.CertifiedScopeText)
                .HasMaxLength(4096);
            builder.Property(x => x.VerificationNotes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.CertificationSchemeId,
                x.Status,
                x.ValidToUtc
            });

            builder.HasIndex(x => new
            {
                x.CertifiedBusinessRelationshipId,
                x.SupplierMaterialOfferingId,
                x.SupplierProcessCapabilityId
            });

            builder.HasIndex(x => new
            {
                x.InventoryLotId,
                x.ShipmentId
            });

            builder.HasOne(x => x.CertificationScheme)
                .WithMany()
                .HasForeignKey(x => x.CertificationSchemeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificationSchemeJurisdiction)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.CertificationSchemeJurisdictionId,
                    x.CertificationSchemeId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.CertificationSchemeId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertifiedBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.CertifiedBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Certifications)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany(x => x.Certifications)
                .HasForeignKey(x => x.SupplierProcessCapabilityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany(x => x.Certifications)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertifiedSiteLocation)
                .WithMany()
                .HasForeignKey(x => x.CertifiedSiteLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IssuerBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.IssuerBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplyDocument)
                .WithMany()
                .HasForeignKey(x => x.SupplyDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
