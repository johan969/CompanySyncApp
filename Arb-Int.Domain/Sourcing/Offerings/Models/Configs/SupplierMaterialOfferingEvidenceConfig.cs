using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierMaterialOfferingEvidenceConfig : IEntityTypeConfiguration<SupplierMaterialOfferingEvidence>
    {
        public void Configure(EntityTypeBuilder<SupplierMaterialOfferingEvidence> builder)
        {
            builder.ToTable(
                "SupplierMaterialOfferingEvidence",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierOfferingEvidence_ObservedDates",
                        """
                    "LastObservedUtc" >= "FirstObservedUtc"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EvidenceType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SourcingEvidenceType.Unknown);

            builder.Property(x => x.SourceSystem)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SourceRecordKey)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.SourceUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            /*
             * Samma importerade post kan återköras utan att skapa
             * ytterligare evidensrader.
             */
            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.SourceSystem,
                x.SourceRecordKey
            })
            .IsUnique();

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Evidence)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}