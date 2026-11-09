using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierProcessCapabilityEvidenceConfig : IEntityTypeConfiguration<SupplierProcessCapabilityEvidence>
    {
        public void Configure(EntityTypeBuilder<SupplierProcessCapabilityEvidence> builder)
        {
            builder.ToTable(
                "SupplierProcessCapabilityEvidence",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_SupplierProcessCapabilityEvidence_ObservedDates",
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

            builder.HasIndex(x => new
            {
                x.SupplierProcessCapabilityId,
                x.SourceSystem,
                x.SourceRecordKey
            })
            .IsUnique();

            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany(x => x.Evidence)
                .HasForeignKey(x => x.SupplierProcessCapabilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}