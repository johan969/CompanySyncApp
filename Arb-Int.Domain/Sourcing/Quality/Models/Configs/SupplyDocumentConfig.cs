using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplyDocumentConfig : IEntityTypeConfiguration<SupplyDocument>
    {
        public void Configure(EntityTypeBuilder<SupplyDocument> builder)
        {
            builder.ToTable(
                "SupplyDocuments",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplyDocuments_Subject",
                        """
                    num_nonnulls("SubjectBusinessRelationshipId", "SupplierMaterialOfferingId",
                        "SupplierProcessCapabilityId", "InventoryLotId", "ShipmentId") >= 1
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplyDocuments_Validity",
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

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.StorageProvider)
                .HasMaxLength(100);

            builder.Property(x => x.StorageKey)
                .HasMaxLength(2048);

            builder.Property(x => x.FileName)
                .HasMaxLength(512);

            builder.Property(x => x.ContentType)
                .HasMaxLength(255);

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            builder.Property(x => x.ExternalDocumentId)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.DocumentTypeId,
                x.Status,
                x.ValidToUtc
            });

            builder.HasIndex(x => x.ContentHash);

            builder.HasIndex(x => new
            {
                x.SubjectBusinessRelationshipId,
                x.SupplierMaterialOfferingId,
                x.SupplierProcessCapabilityId
            });

            builder.HasOne(x => x.DocumentType)
                .WithMany()
                .HasForeignKey(x => x.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SubjectBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.SubjectBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.SupplierProcessCapabilityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReferenceEntry)
                .WithMany()
                .HasForeignKey(x => x.ReferenceEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
