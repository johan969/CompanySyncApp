using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplyAnalysisConfig : IEntityTypeConfiguration<SupplyAnalysis>
    {
        public void Configure(EntityTypeBuilder<SupplyAnalysis> builder)
        {
            builder.ToTable(
                "SupplyAnalyses",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplyAnalyses_Subject",
                        """
                    num_nonnulls("SupplierMaterialOfferingId", "InventoryLotId") >= 1
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
            builder.Property(x => x.AnalysisNumber)
                .HasMaxLength(255);
            builder.Property(x => x.SampleIdentifier)
                .HasMaxLength(255);
            builder.Property(x => x.LaboratoryName)
                .HasMaxLength(255);
            builder.Property(x => x.AccreditationReference)
                .HasMaxLength(1000);
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.InventoryLotId,
                x.Status,
                x.ReportedAtUtc
            });

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.Status
            });

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Analyses)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany(x => x.Analyses)
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplyDocument)
                .WithMany()
                .HasForeignKey(x => x.SupplyDocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LaboratoryBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.LaboratoryBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Results)
                .WithOne(x => x.SupplyAnalysis)
                .HasForeignKey(x => x.SupplyAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
